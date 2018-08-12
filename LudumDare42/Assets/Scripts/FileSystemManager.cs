using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FileSystemManager : MonoBehaviour {

    public static FileSystemManager FSM;

    // TODO: Convert to directories which contain files if I decide that files will add to the game
    private Dictionary<int, FileController> _managedFiles = new Dictionary<int, FileController>();
    private int availableID = 0;

    private void Awake() {
        FSM = this;
    }
    
    public void registerFile(FileController file) {
        file.fileID = availableID++;

        _managedFiles.Add(file.fileID, file);
    }

    public void deregisterFile(FileController file) {
        _managedFiles.Remove(file.fileID);
    }

    public FileController getLastClickedFile() {
        FileController file;
        tryGetFileWithID(FileController.LastClickedFile, out file);
        return file;
    }

    public List<FileController> getAllSelectedFiles(float minX, float maxX, float minY, float maxY) {
        List<FileController> selectedFiles = new List<FileController>();

        foreach (FileController file in _managedFiles.Values) {
            if (file.wasFileClicked) {
                selectedFiles.Add(file);
                continue;
            }

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

    public void deselectLastClickedFile() {
        FileController file;
        if (FileSystemManager.FSM.tryGetFileWithID(FileController.LastClickedFile, out file)) {
            file.tryDeselectFile();
        }
    }

    public void deselectAllFiles() {
        foreach (FileController file in _managedFiles.Values) {
            file.tryDeselectFile();
        }
    }

    public bool tryGetFileWithID(int fileID, out FileController file) {
        return _managedFiles.TryGetValue(fileID, out file) && file != null;
    }

}
