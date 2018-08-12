using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RecycleBinController : FileController {

    public static RecycleBinController RBC;

    public static EventHandler OnRecycleBinFilled;
    public static EventHandler OnRecycleBinDestroyed;

    [SerializeField] private Image _fullnessBar;
    [SerializeField] private Color _emptyColor;
    [SerializeField] private Color _fullColor;

    private List<FileController> _recycledFiles = new List<FileController>();
    private int _emptiedVirusFileCount;
    private int _emptiedAntivirusFileCount;

    private int _maxCapacity = 1;

    protected override void Awake() {
        base.Awake();
        RBC = this;

        _maxCapacity = AppConsts.RECYCLE_BIN_LIMIT_BY_DIFFICULTY[Mathf.Clamp(DataManager.getDifficulty(), 0, AppConsts.RECYCLE_BIN_LIMIT_BY_DIFFICULTY.Length - 1)];
    }

    protected override void Update() {
        base.Update();

        _fullnessBar.fillAmount = fullnessMeterPercent;
        _fullnessBar.color = _emptyColor + (_fullColor - _emptyColor) * fullnessMeterPercent;
    }

    public override void tryRestoreContents() {
        for (int i = 0; i < _recycledFiles.Count; i++) {
            DesktopSystemManager.DSM.neatlyAddFile(_recycledFiles[i]);
        }

        _recycledFiles.Clear();
    }

    public override void tryEmptyRecycleBin() {
        for (int i = 0; i < _recycledFiles.Count; i++) {
            switch (_recycledFiles[i].fileType) {
                case FileType.Virus:
                    _emptiedVirusFileCount++;
                    break;
                case FileType.Antivirus:
                    _emptiedAntivirusFileCount++;
                    break;
                case FileType.ZipBomb:
                    if (!ZipBombManager.ZBM.isVirusActive) {
                        AntivirusManager.AM.prepareToReAddZipBomb();
                    }
                    break;
            }
        }
        _recycledFiles.Clear();
    }

    public void resetCounters() {
        _emptiedVirusFileCount = 0;
        _emptiedAntivirusFileCount = 0;
    }

    public void tryDeleteFile(FileController file) {
        if (canDeleteItems) {
            // If it's an antivirus file and the zip bomb is no longer active, don't keep the file around
            if (!(file.fileType == FileType.Antivirus && !ZipBombManager.ZBM.isVirusActive)) {
                _recycledFiles.Add(file);
            }

            file.gameObject.SetActive(false);
            DesktopSystemManager.DSM.freeUpFile(file);

            // If the recycle bin is being deleted, empty it manually
            if (file.fileType == FileType.RecycleBin) {
                tryEmptyRecycleBin();

                if (OnRecycleBinDestroyed != null) {
                    OnRecycleBinDestroyed.Invoke(this, EventArgs.Empty);
                }
            }

            if (_recycledFiles.Count >= _maxCapacity) {
                if (OnRecycleBinFilled != null) {
                    OnRecycleBinFilled.Invoke(this, EventArgs.Empty);
                }
            }
        }
    }

    public void tryUnzipFile(FileController file) {
        file.gameObject.SetActive(false);
        DesktopSystemManager.DSM.freeUpFile(file);
    }

    public float fullnessMeterPercent {
        get {
            return Mathf.Clamp01((float) _recycledFiles.Count / _maxCapacity);
        }
    }

    public bool isEmpty {
        get {
            return _recycledFiles.Count <= 0;
        }
    }

    public bool canDeleteItems {
        get {
            return gameObject.activeSelf && _recycledFiles.Count < _maxCapacity;
        }
    }

    public int emptiedVirusFileCount {
        get {
            return _emptiedVirusFileCount;
        }
    }

    public int emptiedAntivirusFileCount {
        get {
            return _emptiedAntivirusFileCount;
        }
    }

    public bool isMouseHovering {
        get {
            Vector3 centerPos = _clickBoxRectTransform.position;
            float deltaX = _clickBoxRectTransform.rect.width / 2;
            float deltaY = _clickBoxRectTransform.rect.height / 2;
            return centerPos.x - deltaX <= Input.mousePosition.x && Input.mousePosition.x <= centerPos.x + deltaX &&
                   centerPos.y - deltaY <= Input.mousePosition.y && Input.mousePosition.y <= centerPos.y + deltaY;
        }
    }

}
