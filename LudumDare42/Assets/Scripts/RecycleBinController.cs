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
    private int _emptiedFileCount; // TODO: Split into virus/antivirus counts?

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
        _emptiedFileCount += _recycledFiles.Count;
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
