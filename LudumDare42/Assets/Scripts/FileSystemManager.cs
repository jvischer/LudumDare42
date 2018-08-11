using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FileSystemManager : MonoBehaviour {

    public static FileSystemManager FSM;

    // TODO: Convert to directories which contain files if I decide that files will add to the game
    private HashSet<FileController> _managedFiles = new HashSet<FileController>();

    private void Awake() {
        FSM = this;
    }
    
    public void registerFile(FileController file) {
        _managedFiles.Add(file);
    }

    public void deregisterFile(FileController file) {
        _managedFiles.Remove(file);
    }

    public List<FileController> getAllSelectedFiles(float minX, float maxX, float minY, float maxY) {
        List<FileController> selectedFiles = new List<FileController>();

        foreach (FileController file in _managedFiles) {
            Vector3 filePos = file.transform.position;
            float deltaX = file.getWidth() / 2;
            float deltaY = file.getHeight() / 2;

            float fileMinX = filePos.x - deltaX;
            float fileMaxX = filePos.x + deltaX;
            float fileMinY = filePos.y - deltaY;
            float fileMaxY = filePos.y + deltaY;

            bool isXContained = minX < fileMaxX && fileMinX < maxX;
            bool isYContained = minY < fileMaxY && fileMinY < maxY;
            if (isXContained && isYContained) {
                selectedFiles.Add(file);
            }
        }

        return selectedFiles;
    }

    public void deselectAllFiles() {
        foreach (FileController file in _managedFiles) {
            file.tryDeselectFile();
        }
    }

}
