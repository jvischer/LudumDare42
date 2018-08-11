﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform), typeof(Animator))]
public class FileController : MonoBehaviour {

    public static int lastClickedFile = AppConsts.MISSING_FILE_ID;

    [SerializeField] private RectTransform _clickBoxRectTransform;
    [SerializeField] private FileOption[] _fileOptions;

    [Space]

    public int fileID = AppConsts.DEFAULT_FILE_ID;

    private const string SELECTED_TRIGGER = "Selected";
    private const string DESELECTED_TRIGGER = "Deselected";

    private Animator _animator;
    private FileAction _queuedAction = FileAction.Idle;
    private bool _isSelected = false;

    private bool _isFollowingMouse = false;
    private Vector3 _posBeforeFollowingMouse;

    private void Awake() {
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
        Debug.Log("Tried to unzip file " + fileID);
    }

    public void tryRestoreContents() {
        Debug.Log("Tried to restore contents");
    }

    public void tryEmptyRecycleBin() {
        Debug.Log("Tried to empty the recycle bin");
    }

    public void tryDelete() {
        gameObject.SetActive(false);
    }

    public float getWidth() {
        return _clickBoxRectTransform.rect.width;
    }

    public float getHeight() {
        return _clickBoxRectTransform.rect.height;
    }

    public void clickFile(BaseEventData baseEventData) {
        PointerEventData pointerEventData = baseEventData as PointerEventData;

        DropdownController.DC.hideDropdown();
        if (pointerEventData.button == PointerEventData.InputButton.Left) {
            // Handle lmb
            if (lastClickedFile == fileID) {
                // Handle double click on item
                Debug.Log("DOUBLE CLICK ON FILE " + fileID);
            } else {
                // Replace the last clicked file
                FileSystemManager.FSM.deselectLastClickedFile();
                lastClickedFile = fileID;
            }

            DragController.DC.startSelectionFollowingMouse();
        } else if (pointerEventData.button == PointerEventData.InputButton.Right) {
            // Handle rmb
            if (lastClickedFile != fileID) {
                // Replace the last clicked file
                FileSystemManager.FSM.deselectLastClickedFile();
                lastClickedFile = fileID;
            }

            // Open right click options relative to the file based on the file's data
            DropdownController.DC.displayDropdownForFile(this);
        }
    }

    public void releaseFile(BaseEventData baseEventData) {
        PointerEventData pointerEventData = baseEventData as PointerEventData;

        if (pointerEventData.button == PointerEventData.InputButton.Left) {
            Vector3 pointerDelta = pointerEventData.position - pointerEventData.pressPosition;
            Debug.Log("Delta is " + pointerDelta + " w/ len " + pointerDelta.sqrMagnitude);

            DragController.DC.stopSelectionFollowingMouse();
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
                Debug.Log("Find the new location for the file!");
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

    public bool wasFileClicked {
        get {
            return lastClickedFile == fileID;
        }
    }

    public FileOption[] fileOptions {
        get {
            return _fileOptions;
        }
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
