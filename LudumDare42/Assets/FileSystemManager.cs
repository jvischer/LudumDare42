using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FileSystemManager : MonoBehaviour {

    public static FileSystemManager FSM;

    // TODO: Convert to directories which contain files if I decide that files will add to the game
    private HashSet<FileController> _managedFiles;

    private void Awake() {
        FSM = this;
    }

    private void Update() {
        tryReleaseFiles();
    }

    public void registerFile(FileController file) {
        _managedFiles.Add(file);
    }

    public void deregisterFile(FileController file) {
        _managedFiles.Remove(file);
    }

    private void tryReleaseFiles() {
        if (Input.GetMouseButtonUp(0)) {
            foreach (FileController file in _managedFiles) {
                file.tryDeselectFile();
            }
        }
    }

}
