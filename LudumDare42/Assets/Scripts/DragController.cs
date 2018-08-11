using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragController : MonoBehaviour {

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

    private void Update() {
        handleInput();
        updateSelection();
    }

    private void updateSelection() {
        if (!_isDragging) {
            FileController file;
            if (FileSystemManager.FSM.tryGetFileWithID(FileController.lastClickedFile, out file)) {
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

            FileSystemManager.FSM.deselectAllFiles();
            _cachedSelectedFiles = FileSystemManager.FSM.getAllSelectedFiles(minX, maxX, minY, maxY);
            for (int i = 0; i < _cachedSelectedFiles.Count; i++) {
                _cachedSelectedFiles[i].trySelectFile();
            }
        }
    }

    public void startDesktopDrag() {
        _startPos = Input.mousePosition;
        _isDragging = true;

        _topDragLine.gameObject.SetActive(true);
        _botDragLine.gameObject.SetActive(true);
        _leftDragLine.gameObject.SetActive(true);
        _rightDragLine.gameObject.SetActive(true);

        FileController.lastClickedFile = AppConsts.MISSING_FILE_ID;
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
        if (Input.GetKeyDown(KeyCode.Delete)) {
            FileController file;
            if (FileSystemManager.FSM.tryGetFileWithID(FileController.lastClickedFile, out file)) {
                file.tryDelete();
            }

            for (int i = 0; i < _cachedSelectedFiles.Count; i++) {
                _cachedSelectedFiles[i].tryDelete();
            }
        }
    }

}
