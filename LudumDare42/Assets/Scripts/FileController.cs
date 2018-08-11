using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform), typeof(Animator))]
public class FileController : MonoBehaviour {

    private const string SELECTED_TRIGGER = "Selected";
    private const string DESELECTED_TRIGGER = "Deselected";

    private RectTransform _rectTransform;
    private Animator _animator;
    private FileAction _queuedAction = FileAction.Idle;
    private bool _isSelected = false;

    private void Awake() {
        _rectTransform = gameObject.GetComponent<RectTransform>();
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
        return _rectTransform.rect.width;
    }

    public float getHeight() {
        return _rectTransform.rect.height;
    }

    private enum FileAction {
        Idle, Select, Deselect,
    }

}
