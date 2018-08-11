using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class DesktopSystemManager : MonoBehaviour {

    public static DesktopSystemManager DSM;

    [SerializeField] private FileController[] _defaultIcons;
    [SerializeField] private Transform _activeFilePool;

    [Space]

    [SerializeField] private FileOption[] _desktopOptions;
    [SerializeField] private Sprite[] _desktopBGOptions;

    [Space]

    [SerializeField] private FileController[] _virusIconPool;
    [SerializeField] private FileController[] _antiVirusIconPool;

    private Image _backgroundImage;
    private RectTransform[] _desktopIconPlaceholders;

    private bool[] _indicesAvailability;
    private HashSet<int> _availableIndices = new HashSet<int>();

    private HashSet<FileController> _availableVirusFiles = new HashSet<FileController>();
    private HashSet<FileController> _availableAntivirusFiles = new HashSet<FileController>();

    private int _currentDesktopBG = 0;

    private void Awake() {
        DSM = this;

        _backgroundImage = gameObject.GetComponent<Image>();
    }

    private IEnumerator Start() {
        yield return null;
        
        _desktopIconPlaceholders = new RectTransform[transform.childCount];
        _indicesAvailability = new bool[transform.childCount];
        for (int i = 0; i < _desktopIconPlaceholders.Length; i++) {
            _desktopIconPlaceholders[i] = transform.GetChild(i).GetComponent<RectTransform>();
            _indicesAvailability[i] = true;

            _availableIndices.Add(i);
        }

        for (int i = 0; i < _defaultIcons.Length; i++) {
            _availableIndices.Remove(i);
            _indicesAvailability[i] = false;

            _defaultIcons[i].desktopPositionIndex = i;
            _defaultIcons[i].transform.position = _desktopIconPlaceholders[i].position;
            _defaultIcons[i].gameObject.SetActive(true);

            _defaultIcons[i].transform.SetParent(_activeFilePool);
            _defaultIcons[i].transform.SetAsLastSibling();
        }
    }

    public FileController getFileOfType(FileController.FileType fileType) {
        switch (fileType) {
            case FileController.FileType.Antivirus:
                if (_availableAntivirusFiles.Count > 0) {
                    return _availableAntivirusFiles.ElementAt(UnityEngine.Random.Range(0, _availableAntivirusFiles.Count));
                }
                break;
            case FileController.FileType.Virus:
                if (_availableVirusFiles.Count > 0) {
                    return _availableVirusFiles.ElementAt(UnityEngine.Random.Range(0, _availableVirusFiles.Count));
                }
                break;
        }
        return null;
    }

    public void neatlyAddFile(FileController file) {
        int firstAvailableIndex = -1;
        for (int i = 0; i < _indicesAvailability.Length; i++) {
            if (_indicesAvailability[i]) {
                firstAvailableIndex = i;
                break;
            }
        }

        if (firstAvailableIndex < 0) {
            Debug.Log("Desktop System Manager ran out of desktop space to place a file");
            SystemCrashHandler.SCH.crashSystem();
            return;
        }

        _availableIndices.Remove(firstAvailableIndex);
        _indicesAvailability[firstAvailableIndex] = false;

        file.desktopPositionIndex = firstAvailableIndex;
        file.transform.position = _desktopIconPlaceholders[firstAvailableIndex].position;
        file.gameObject.SetActive(true);

        file.transform.SetParent(_activeFilePool);
        file.transform.SetAsLastSibling();

        file.tryDeselectFile();

        switch (file.fileType) {
            case FileController.FileType.Antivirus:
                _availableAntivirusFiles.Remove(file);
                break;
            case FileController.FileType.Virus:
                _availableVirusFiles.Remove(file);
                break;
        }
    }

    public void freeUpFile(FileController file) {
        if (file.desktopPositionIndex < 0) {
        Debug.Log("Freeing up file with invalid " + file.desktopPositionIndex);
            return;
        }

        Debug.Log("Freeing up file " + file.desktopPositionIndex);
        _availableIndices.Add(file.desktopPositionIndex);
        _indicesAvailability[file.desktopPositionIndex] = true;

        switch (file.fileType) {
            case FileController.FileType.Antivirus:
                _availableAntivirusFiles.Add(file);
                break;
            case FileController.FileType.Virus:
                _availableVirusFiles.Add(file);
                break;
        }
    }

    public void clickDesktop(BaseEventData baseEventData) {
        PointerEventData pointerEventData = baseEventData as PointerEventData;

        if (pointerEventData.button == PointerEventData.InputButton.Right) {
            // Open right click options for the desktop
            DropdownController.DC.displayDropdownForDesktop(this);
        }
    }

    public void rotateDesktopBackground() {
        _currentDesktopBG++;
        _currentDesktopBG %= _desktopBGOptions.Length;
        _backgroundImage.sprite = _desktopBGOptions[_currentDesktopBG];
    }

    //private void Update() {
    //    if (Input.GetKeyDown(KeyCode.LeftWindows) || Input.GetKeyDown(KeyCode.RightWindows)) {
    //        // TODO: Alternate way to click the start button IF necessary
    //    }
    //}

    public FileOption[] desktopOptions {
        get {
            return _desktopOptions;
        }
    }

}
