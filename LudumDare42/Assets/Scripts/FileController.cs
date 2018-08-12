using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform), typeof(Animator))]
public class FileController : MonoBehaviour {

    public static int LastClickedFile = AppConsts.MISSING_FILE_ID;

    [SerializeField] protected RectTransform _clickBoxRectTransform;
    [SerializeField] protected FileOption[] _fileOptions;
    [SerializeField] protected Text _fileNameText;

    [Space]

    public FileType fileType = FileType.Default;
    public int fileID = AppConsts.DEFAULT_FILE_ID;
    public int desktopPositionIndex = -1;

    private const string SELECTED_TRIGGER = "Selected";
    private const string DESELECTED_TRIGGER = "Deselected";

    private Animator _animator;
    private FileAction _queuedAction = FileAction.Idle;
    private bool _isSelected = false;

    private bool _isFollowingMouse = false;
    private Vector3 _posBeforeFollowingMouse;

    protected virtual void Awake() {
        _animator = gameObject.GetComponent<Animator>();
    }

    private void OnEnable() {
        FileSystemManager.FSM.registerFile(this);
    }

    private void OnDisable() {
        FileSystemManager.FSM.deregisterFile(this);
    }

    private void Update() {
        if (_isFollowingMouse) {
            transform.position = Input.mousePosition;
        }

        switch (_queuedAction) {
            case FileAction.Select:
                if (!_isSelected) {
                    _isSelected = true;

                    _animator.SetTrigger(SELECTED_TRIGGER);
                }
                _queuedAction = FileAction.Idle;
                break;
            case FileAction.Deselect:
                if (_isSelected) {
                    _isSelected = false;

                    _animator.SetTrigger(DESELECTED_TRIGGER);
                }
                _queuedAction = FileAction.Idle;
                break;
        }
    }

    public void trySelectFile() {
        _queuedAction = FileAction.Select;
    }

    public void tryDeselectFile() {
        _queuedAction = FileAction.Deselect;
    }

    public void tryUnzip() {
        StartCoroutine(handleUnzip());
    }

    private IEnumerator handleUnzip() {
        UnzipController.UC.displayForFile(this);
        yield return new WaitForSeconds(AppConsts.FILE_UNZIP_DURATION);

        if (!ZipBombManager.ZBM.isVirusActive) {
            ZipBombManager.ZBM.executeVirus();
            tryDelete();
        } else {
            FileController newFile = DesktopSystemManager.DSM.getFileOfType(FileType.Virus);
            if (newFile != null) {
                DesktopSystemManager.DSM.randomlyAddFile(newFile);
            }
            tryDelete();
        }
    }

    public virtual void tryRestoreContents() {
        
    }

    public virtual void tryEmptyRecycleBin() {
        
    }

    public void tryDelete() {
        if (!wasFileDeleted) {
            RecycleBinController.RBC.tryDeleteFile(this);
        }
    }

    public float getWidth() {
        return _clickBoxRectTransform.rect.width;
    }

    public float getHeight() {
        return _clickBoxRectTransform.rect.height;
    }

    public void setName(string name) {
        gameObject.name = name;
        _fileNameText.text = name;
    }

    private bool _wasSelectedBeforeClick = false;

    public void clickFile(BaseEventData baseEventData) {
        PointerEventData pointerEventData = baseEventData as PointerEventData;

        DropdownController.DC.hideDropdown();
        if (pointerEventData.button == PointerEventData.InputButton.Left) {
            Debug.Log("Last clicked file " + LastClickedFile + " w/ " + fileID);
            // Handle lmb
            _wasSelectedBeforeClick = _isSelected;
            if (!_isSelected) {
                // Replace the last clicked file
                FileSystemManager.FSM.deselectLastClickedFile();
                LastClickedFile = fileID;

                DragController.DC.clearCache();
                //DragController.DC.updateSelectedFileCache();
            }

            //if (LastClickedFile == fileID) {
            //    // Handle double click on item
            //    onDoubleClick();
            //} else {
            //    // Replace the last clicked file
            //    FileSystemManager.FSM.deselectLastClickedFile();
            //    LastClickedFile = fileID;

            //    DragController.DC.clearCache();
            //    //DragController.DC.updateSelectedFileCache();
            //}

            transform.SetAsLastSibling();
            DragController.DC.startSelectionFollowingMouse();
        } else if (pointerEventData.button == PointerEventData.InputButton.Right) {
            // Handle rmb
            if (LastClickedFile != fileID) {
                // Replace the last clicked file
                FileSystemManager.FSM.deselectLastClickedFile();
                LastClickedFile = fileID;
            }

            // Open right click options relative to the file based on the file's data
            DropdownController.DC.displayDropdownForFile(this);
        }
    }

    public void releaseFile(BaseEventData baseEventData) {
        PointerEventData pointerEventData = baseEventData as PointerEventData;

        if (pointerEventData.button == PointerEventData.InputButton.Left) {
            DragController.DC.stopSelectionFollowingMouse();

            // If the file was already selected AND is now AND the mouse barely moved
            if (_wasSelectedBeforeClick && _isSelected &&
                (pointerEventData.position - pointerEventData.pressPosition).sqrMagnitude < AppConsts.DESKTOP_MOVEMENT_NULLABLE_SQR_DISTANCE) {
                // Handle double click on item
                onDoubleClick();
            }

            if (!_wasSelectedBeforeClick && _isSelected) {
                DragController.DC.clearCache();
            }

            // BLOCK WAS MOVED FROM CLICK FILE. INSTEAD ACT ON A MIN TIME FOR RELEASE
            //if ((pointerEventData.position - pointerEventData.pressPosition).sqrMagnitude < 50.0F) {
            //    // Replace the last clicked file
            //    FileSystemManager.FSM.deselectLastClickedFile();
            //    lastClickedFile = fileID;

            //    DragController.DC.clearCache();
            //    DragController.DC.updateSelectedFileCache();
            //}

            // Is it above the recycle bin and holding files that aren't the recycle bin
            if (RecycleBinController.RBC.isMouseHovering) {
                bool canDeleteFiles = true;
                List<FileController> selectedFiles = DragController.DC.selectedFiles;
                for (int i = 0; i < selectedFiles.Count; i++) {
                    if (selectedFiles[i].fileType == FileType.RecycleBin) {
                        canDeleteFiles = false;
                        break;
                    }
                }

                if (canDeleteFiles) {
                    for (int i = 0; i < selectedFiles.Count; i++) {
                        selectedFiles[i].tryDelete();
                    }
                }
            }
        }
    }

    private void onDoubleClick() {
        Debug.Log("DOUBLE CLICK ON FILE " + fileID);
        if (fileType == FileType.ZipBomb ||
            fileType == FileType.Virus) {
            tryUnzip();
        }
    }

    public void startFollowMouse() {
        if (!_isFollowingMouse) {
            _isFollowingMouse = true;

            _posBeforeFollowingMouse = transform.position;
        }
    }

    public void displaceBy(Vector3 delta) {
        transform.position = _posBeforeFollowingMouse + delta;
    }

    public void stopFollowMouse(bool trySnapToGrid) {
        if (_isFollowingMouse) {
            _isFollowingMouse = false;

            if (trySnapToGrid) {
                // Find a new nearest unoccupied location
                Debug.Log("TODO: Find the new location for the file!");
                // TODO: Remove snap below, it's just to prevent visual issues for now
                transform.position = _posBeforeFollowingMouse;
            } else {
                // Return to the previous location
                transform.position = _posBeforeFollowingMouse;
            }
        }
    }

    public void clickedEvent_Unzip() {
        tryUnzip();
    }

    public void clickedEvent_RestoreContents() {
        tryRestoreContents();
    }

    public void clickedEvent_EmptyRecycleBin() {
        tryEmptyRecycleBin();
    }

    public void clickedEvent_Delete() {
        tryDelete();
    }

    public bool wasFileDeleted {
        get {
            return !gameObject.activeSelf;
        }
    }

    public bool wasFileClicked {
        get {
            return LastClickedFile == fileID;
        }
    }

    public FileOption[] fileOptions {
        get {
            return _fileOptions;
        }
    }

    public enum FileType {
        Default, RecycleBin, ZipBomb, Virus, Antivirus,
    }

    private enum FileAction {
        Idle, Select, Deselect,
    }
    
}

[Serializable]
public class FileOption {

    public string optionText;
    public UnityEvent clickedEvent;

    public void fireClickedEvent() {
        if (clickedEvent != null) {
            clickedEvent.Invoke();
        }
    }

}
