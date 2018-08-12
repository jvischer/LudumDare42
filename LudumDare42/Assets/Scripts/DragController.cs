﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragController : MonoBehaviour {

    public static DragController DC;

    [SerializeField] private RectTransform _parentCanvas;
	[SerializeField] private RectTransform _topDragLine;
	[SerializeField] private RectTransform _botDragLine;
	[SerializeField] private RectTransform _leftDragLine;
	[SerializeField] private RectTransform _rightDragLine;

    [Space]

    [SerializeField] private float _lineThickness = 2;

    private List<FileController> _cachedSelectedFiles = new List<FileController>();
    private Vector3 _startPos;
    private bool _isDragging = false;
    private bool _isDraggingFiles = false;

    private void Awake() {
        DC = this;
    }

    private void Update() {
        handleInput();
        updateSelection();
        moveDraggedFiles();
        updateCacheVisuals();
    }

    private void updateSelection() {
        if (!_isDragging) {
            FileController file;
            if (FileSystemManager.FSM.tryGetFileWithID(FileController.LastClickedFile, out file)) {
                file.trySelectFile();
            }
        } else {
            float minX = Mathf.Min(Input.mousePosition.x, _startPos.x);
            float minY = Mathf.Min(Input.mousePosition.y, _startPos.y);
            float maxX = Mathf.Max(Input.mousePosition.x, _startPos.x);
            float maxY = Mathf.Max(Input.mousePosition.y, _startPos.y);

            Vector2 middle = (_startPos + Input.mousePosition) / 2;
            float width = Mathf.Abs(Input.mousePosition.x - _startPos.x);
            float height = Mathf.Abs(Input.mousePosition.y - _startPos.y);

            Vector2 horiz = new Vector2(width * (1 / _parentCanvas.localScale.x), _lineThickness);
            Vector2 vert = new Vector2(_lineThickness, height * (1 / _parentCanvas.localScale.y));

            Vector2 pos;
            pos = _topDragLine.position;
            pos.x = middle.x;
            pos.y = maxY;
            _topDragLine.position = pos;
            _topDragLine.sizeDelta = horiz;

            pos = _botDragLine.position;
            pos.x = middle.x;
            pos.y = minY;
            _botDragLine.position = pos;
            _botDragLine.sizeDelta = horiz;

            pos = _leftDragLine.position;
            pos.x = minX;
            pos.y = middle.y;
            _leftDragLine.position = pos;
            _leftDragLine.sizeDelta = vert;

            pos = _rightDragLine.position;
            pos.x = maxX;
            pos.y = middle.y;
            _rightDragLine.position = pos;
            _rightDragLine.sizeDelta = vert;

            updateSelectedFileCache();
        }
    }

    public void updateSelectedFileCache() {
        float minX = Mathf.Min(Input.mousePosition.x, _startPos.x);
        float minY = Mathf.Min(Input.mousePosition.y, _startPos.y);
        float maxX = Mathf.Max(Input.mousePosition.x, _startPos.x);
        float maxY = Mathf.Max(Input.mousePosition.y, _startPos.y);

        FileSystemManager.FSM.deselectAllFiles();
        _cachedSelectedFiles = FileSystemManager.FSM.getAllSelectedFiles(minX, maxX, minY, maxY);
        for (int i = 0; i < _cachedSelectedFiles.Count; i++) {
            _cachedSelectedFiles[i].trySelectFile();
        }
    }

    public void clearCache() {
        for (int i = 0; i < _cachedSelectedFiles.Count; i++) {
            _cachedSelectedFiles[i].tryDeselectFile();
        }

        FileController selectedFile = FileSystemManager.FSM.getLastClickedFile();
        if (selectedFile == null) {
            _cachedSelectedFiles.Clear();
        } else {
            _cachedSelectedFiles = new List<FileController>() { selectedFile };
            selectedFile.trySelectFile();
        }
    }

    public void startDesktopDrag() {
        _startPos = Input.mousePosition;
        _isDragging = true;

        _topDragLine.gameObject.SetActive(true);
        _botDragLine.gameObject.SetActive(true);
        _leftDragLine.gameObject.SetActive(true);
        _rightDragLine.gameObject.SetActive(true);

        FileController.LastClickedFile = AppConsts.MISSING_FILE_ID;
        FileSystemManager.FSM.deselectAllFiles();
        DropdownController.DC.hideDropdown();
    }

    public void stopDesktopDrag() {
        _isDragging = false;

        _topDragLine.gameObject.SetActive(false);
        _botDragLine.gameObject.SetActive(false);
        _leftDragLine.gameObject.SetActive(false);
        _rightDragLine.gameObject.SetActive(false);
    }

    private void handleInput() {
        if ((Input.GetKeyDown(KeyCode.Delete) || Input.GetKeyDown(KeyCode.Backspace)) && !_isDragging) {
            FileController file;
            if (FileSystemManager.FSM.tryGetFileWithID(FileController.LastClickedFile, out file)) {
                file.tryDelete();
            }

            for (int i = _cachedSelectedFiles.Count - 1; i >= 0; i--) {
                _cachedSelectedFiles[i].tryDelete();
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape)) {
            stopDesktopDrag();

            FileController.LastClickedFile = AppConsts.MISSING_FILE_ID;
            FileSystemManager.FSM.deselectAllFiles();
            DropdownController.DC.hideDropdown();
        }
    }

    public void startSelectionFollowingMouse() {
        _isDraggingFiles = true;
        _startPos = Input.mousePosition;

        //updateSelectedFileCache();

        for (int i = 0; i < _cachedSelectedFiles.Count; i++) {
            Debug.Log("Starting file following mouse " + _cachedSelectedFiles[i].name);
            _cachedSelectedFiles[i].startFollowMouse();
        }
    }

    private void moveDraggedFiles() {
        if (!_isDraggingFiles) {
            return;
        }

        Vector3 delta = Input.mousePosition - _startPos;
        for (int i = 0; i < _cachedSelectedFiles.Count; i++) {
            _cachedSelectedFiles[i].displaceBy(delta);
        }
    }

    public void stopSelectionFollowingMouse() {
        _isDraggingFiles = false;

        for (int i = 0; i < _cachedSelectedFiles.Count; i++) {
            // TODO: Set to true if released over the desktop or another file (use tags)
            _cachedSelectedFiles[i].stopFollowMouse(false);
        }
    }

    private void updateCacheVisuals() {
        for (int i = 0; i < _cachedSelectedFiles.Count; i++) {
            _cachedSelectedFiles[i].trySelectFile();
        }
    }

    public List<FileController> selectedFiles {
        get {
            return _cachedSelectedFiles;
        }
    }

}
