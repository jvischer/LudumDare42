using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform), typeof(Animator))]
public class FileController : MonoBehaviour {

    public static int lastClickedFile = AppConsts.MISSING_FILE_ID;

    [SerializeField] private RectTransform _clickBoxRectTransform;

    public int fileID = AppConsts.DEFAULT_FILE_ID;

    private const string SELECTED_TRIGGER = "Selected";
    private const string DESELECTED_TRIGGER = "Deselected";

    private Animator _animator;
    private FileAction _queuedAction = FileAction.Idle;
    private bool _isSelected = false;

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

    public void tryDelete() {
        gameObject.SetActive(false);
    }

    public float getWidth() {
        return _clickBoxRectTransform.rect.width;
    }

    public float getHeight() {
        return _clickBoxRectTransform.rect.height;
    }

    public void clickFile() {
        if (lastClickedFile == fileID) {
            // Handle double click on item
            Debug.Log("DOUBLE CLICK ON FILE " + fileID);
        } else {
            // Replace the last clicked file
            FileSystemManager.FSM.deselectLastClickedFile();
            lastClickedFile = fileID;
        }
    }

    public bool wasFileClicked {
        get {
            return lastClickedFile == fileID;
        }
    }

    private enum FileAction {
        Idle, Select, Deselect,
    }

}
