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

    [Space]

    [SerializeField] private IconToggleManager _audioToggleManager;

    private Image _backgroundImage;
    private RectTransform[] _desktopIconPlaceholders;

    private bool[] _indexIsAvailable;
    private HashSet<int> _availableIndices = new HashSet<int>();

    private HashSet<FileController> _availableVirusFiles = new HashSet<FileController>();
    private HashSet<FileController> _activeVirusFiles = new HashSet<FileController>();
    private HashSet<FileController> _availableAntivirusFiles = new HashSet<FileController>();
    private HashSet<FileController> _activeAntivirusFiles = new HashSet<FileController>();

    private int _currentDesktopBG = 0;
    private int _availableVirusID = 0;
    private int _availableAntivirusID = 0;

    private void Awake() {
        DSM = this;

        _backgroundImage = gameObject.GetComponent<Image>();

        bool isAudioEnabled;
        if (DataManager.tryGetBool(AppConsts.DATA_AUDIO_KEY, out isAudioEnabled)) {
            _audioToggleManager.initialize(isAudioEnabled);
        }
    }

    private IEnumerator Start() {
        yield return null;
        
        _desktopIconPlaceholders = new RectTransform[transform.childCount];
        _indexIsAvailable = new bool[transform.childCount];
        for (int i = 0; i < _desktopIconPlaceholders.Length; i++) {
            _desktopIconPlaceholders[i] = transform.GetChild(i).GetComponent<RectTransform>();
            _indexIsAvailable[i] = true;

            _availableIndices.Add(i);
        }

        for (int i = 0; i < _defaultIcons.Length; i++) {
            _availableIndices.Remove(i);
            _indexIsAvailable[i] = false;

            _defaultIcons[i].desktopPositionIndex = i;
            _defaultIcons[i].transform.position = _desktopIconPlaceholders[i].position;
            _defaultIcons[i].gameObject.SetActive(true);

            _defaultIcons[i].transform.SetParent(_activeFilePool);
            _defaultIcons[i].transform.SetAsLastSibling();
        }

        for (int i = 0; i < _virusIconPool.Length; i++) {
            _availableVirusFiles.Add(_virusIconPool[i]);
        }

        for (int i = 0; i < _antiVirusIconPool.Length; i++) {
            _availableAntivirusFiles.Add(_antiVirusIconPool[i]);
        }
    }

    private void OnApplicationPause(bool pause) {
        serializeAndSaveData();
    }

    private void OnApplicationQuit() {
        serializeAndSaveData();
    }

    private void serializeAndSaveData() {
        DataManager.setBool(AppConsts.DATA_AUDIO_KEY, _audioToggleManager.status);

        DataManager.save();
    }

    public void resetCounters() {
        _availableVirusID = 0;
        _availableAntivirusID = 0;
    }

    public void tryReAddZipBomb() {
        if (_defaultIcons[1].wasFileDeleted) {
            StartCoroutine(keepTryingToReAddZipBomb());
        }
    }

    private IEnumerator keepTryingToReAddZipBomb() {
        while (true) {
            if (RecycleBinController.RBC.isEmpty) {
                neatlyAddFile(_defaultIcons[1]);
                break;
            }

            yield return null;
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
        for (int i = 0; i < _indexIsAvailable.Length; i++) {
            if (_indexIsAvailable[i]) {
                firstAvailableIndex = i;
                break;
            }
        }

        if (firstAvailableIndex < 0) {
            Debug.Log("Desktop System Manager ran out of desktop space to neatly place a file");
            SystemCrashHandler.SCH.crashSystem();
            return;
        }

        addFileAtIndex(file, firstAvailableIndex);
    }

    public void randomlyAddFile(FileController file) {
        if (_availableIndices.Count <= 0) {
            Debug.Log("Desktop System Manager ran out of desktop space to randomly place a file");
            SystemCrashHandler.SCH.crashSystem();
            return;
        }

        int chosenIndex = _availableIndices.ElementAt(UnityEngine.Random.Range(0, _availableIndices.Count));
        addFileAtIndex(file, chosenIndex);
    }

    private void addFileAtIndex(FileController file, int chosenIndex) {
        _availableIndices.Remove(chosenIndex);
        _indexIsAvailable[chosenIndex] = false;

        file.desktopPositionIndex = chosenIndex;
        file.transform.position = _desktopIconPlaceholders[chosenIndex].position;
        file.gameObject.SetActive(true);

        file.transform.SetParent(_activeFilePool);
        file.transform.SetAsFirstSibling();

        file.tryDeselectFile();

        switch (file.fileType) {
            case FileController.FileType.Virus:
                file.setName(String.Format(AppConsts.FILE_NAME_VIRUS_FMT, _availableVirusID++));

                _availableVirusFiles.Remove(file);
                _activeVirusFiles.Add(file);
                break;
            case FileController.FileType.Antivirus:
                file.setName(String.Format(AppConsts.FILE_NAME_ANTIVIRUS_FMT, _availableAntivirusID++));

                _availableAntivirusFiles.Remove(file);
                _activeAntivirusFiles.Add(file);
                break;
        }
    }

    public void freeUpFile(FileController file) {
        if (file.desktopPositionIndex < 0) {
            Debug.Log("Freeing up file with an invalid index: " + file.desktopPositionIndex);
            return;
        }

        _availableIndices.Add(file.desktopPositionIndex);
        _indexIsAvailable[file.desktopPositionIndex] = true;

        switch (file.fileType) {
            case FileController.FileType.Virus:
                _availableVirusFiles.Add(file);
                _activeVirusFiles.Remove(file);
                break;
            case FileController.FileType.Antivirus:
                _availableAntivirusFiles.Add(file);
                _activeAntivirusFiles.Remove(file);
                break;
        }
    }

    public bool trySnapToNearestPlaceholder(FileController file) {
        Vector3 filePos = file.transform.position;
        float closestPlaceholderSqrDistance = float.MaxValue;
        int desktopIconPlaceholderIndex = -1;
        for (int i = 0; i < _desktopIconPlaceholders.Length; i++) {
            // Pick the closest icon
            float currPlaceholderSqrDistance = (_desktopIconPlaceholders[i].position - filePos).sqrMagnitude;
            if (desktopIconPlaceholderIndex < 0 ||
                currPlaceholderSqrDistance < closestPlaceholderSqrDistance) {
                closestPlaceholderSqrDistance = currPlaceholderSqrDistance;
                desktopIconPlaceholderIndex = i;
            }
        }
        
        // If nothing was selected OR the index is not available
        if (desktopIconPlaceholderIndex < 0 ||
            !_indexIsAvailable[desktopIconPlaceholderIndex]) {
            return false;
        }

        // Update the position
        file.transform.position = _desktopIconPlaceholders[desktopIconPlaceholderIndex].position;

        // Free up the previous index
        _availableIndices.Add(file.desktopPositionIndex);
        _indexIsAvailable[file.desktopPositionIndex] = true;

        // Write over it and reserve the new index
        file.desktopPositionIndex = desktopIconPlaceholderIndex;
        _availableIndices.Remove(file.desktopPositionIndex);
        _indexIsAvailable[file.desktopPositionIndex] = false;
        return true;
    }

    public void killVirusFiles() {
        FileController[] filesToKill = _activeVirusFiles.ToArray<FileController>();
        for (int i = 0; i < filesToKill.Length; i++) {
            filesToKill[i].tryDelete();
        }
    }

    public bool tryKillRandomAntivirusFile() {
        if (_activeAntivirusFiles.Count <= 0) {
            return false;
        }

        FileController fileToKill = _activeAntivirusFiles.ElementAt(UnityEngine.Random.Range(0, _activeAntivirusFiles.Count));
        fileToKill.tryDelete();
        return true;
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

    public void toggleStartMenu() {
        StartMenuController.SMC.toggleStartMenu();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.LeftWindows) || Input.GetKeyDown(KeyCode.RightWindows)) {
            toggleStartMenu();
        }
    }

    public FileOption[] desktopOptions {
        get {
            return _desktopOptions;
        }
    }

    public int activeVirusFileCount {
        get {
            return _activeVirusFiles.Count;
        }
    }

    public int activeAntivirusFileCount {
        get {
            return _activeAntivirusFiles.Count;
        }
    }

    public int desktopIconSpace {
        get {
            return _desktopIconPlaceholders.Length;
        }
    }

}
