using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZipBombManager : MonoBehaviour {

    public static ZipBombManager ZBM;

    public static EventHandler OnZipBombExecuted;
    public static EventHandler OnZipBombKilled;
    
    [SerializeField] private DifficultyData[] _difficultyData;

    private bool _isVirusActive = false;
    private float _activeDuration = 0;

    private void Awake() {
        ZBM = this;
    }

    private void Update() {
        if (_isVirusActive) {
            _activeDuration += Time.deltaTime;
        }
    }

    public void executeVirus() {
        _isVirusActive = true;
        _activeDuration = 0;

        if (OnZipBombExecuted != null) {
            OnZipBombExecuted.Invoke(this, EventArgs.Empty);
        }

        AntivirusManager.AM.startScanning();
        StartCoroutine(handleVirus());
    }

    public void killVirus() {
        if (OnZipBombKilled != null) {
            OnZipBombKilled.Invoke(this, EventArgs.Empty);
        }

        _isVirusActive = false;
        StopAllCoroutines();
    }

    private IEnumerator handleVirus() {
        int difficulty = Mathf.Clamp(DataManager.getDifficulty(), 0, _difficultyData.Length - 1);

        float time = 0;
        while (true) {
            FileController newFile = DesktopSystemManager.DSM.getFileOfType(FileController.FileType.Virus);
            if (newFile != null) {
                DesktopSystemManager.DSM.randomlyAddFile(newFile);
            }

            float waitTime = _difficultyData[difficulty].getWaitTime(time);
            time += waitTime;
            yield return new WaitForSeconds(waitTime);
        }
    }

    public bool isVirusActive {
        get {
            return _isVirusActive;
        }
    }

    public float activeDuration {
        get {
            return _activeDuration;
        }
    }

}
