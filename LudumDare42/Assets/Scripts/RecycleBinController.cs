using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RecycleBinController : FileController {

    public static RecycleBinController RBC;

    private List<FileController> _recycledFiles = new List<FileController>();
    private int _emptiedVirusFileCount;
    private int _emptiedAntivirusFileCount;

    protected override void Awake() {
        base.Awake();
        RBC = this;
    }

    public override void tryRestoreContents() {
        Debug.Log("Tried to restore " + _recycledFiles.Count + " contents");
        for (int i = 0; i < _recycledFiles.Count; i++) {
            DesktopSystemManager.DSM.neatlyAddFile(_recycledFiles[i]);
        }

        _recycledFiles.Clear();
    }

    public override void tryEmptyRecycleBin() {
        Debug.Log("Tried to empty the recycle bin");
        for (int i = 0; i < _recycledFiles.Count; i++) {
            switch (_recycledFiles[i].fileType) {
                case FileType.Virus:
                    _emptiedVirusFileCount++;
                    break;
                case FileType.Antivirus:
                    _emptiedAntivirusFileCount++;
                    break;
            }
        }
        _recycledFiles.Clear();
    }

    public void tryDeleteFile(FileController file) {
        if (canDeleteItems) {
            _recycledFiles.Add(file);
            file.gameObject.SetActive(false);
            DesktopSystemManager.DSM.freeUpFile(file);
        }
    }

    public bool canDeleteItems {
        get {
            return gameObject.activeSelf;
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
