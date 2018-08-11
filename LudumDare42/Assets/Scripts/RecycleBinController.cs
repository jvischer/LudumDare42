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
    }

    public override void tryEmptyRecycleBin() {
        Debug.Log("Tried to empty the recycle bin");
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
            float halfWidth = _clickBoxRectTransform.rect.width / 2;
            float halfHeight = _clickBoxRectTransform.rect.height / 2;
            return centerPos.x - halfWidth <= Input.mousePosition.x && Input.mousePosition.x <= centerPos.x + halfWidth &&
                   centerPos.y - halfHeight <= Input.mousePosition.y && Input.mousePosition.y <= centerPos.y + halfHeight;
        }
    }

}
