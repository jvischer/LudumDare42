using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntivirusManager : MonoBehaviour {

    public static AntivirusManager AM;

    public static EventHandler OnAntivirusExecuted;

    [SerializeField] private float _scanningVirusDuration;

    [Space]

    [SerializeField] private DifficultyData[] _difficultyData;

    [Space]

    [SerializeField] private float _killDurationUponVirusKilled;
    [SerializeField] private float _delayToSpawnNewZipBomb;

    private bool _isAntivirusActive = false;

    private void Awake() {
        AM = this;
    }

    public void startScanning() {
        StartCoroutine(handleScanning());
    }

    private IEnumerator handleScanning() {
        yield return new WaitForSeconds(_scanningVirusDuration);

        executeAntivirus();
    }

    private void executeAntivirus() {
        _isAntivirusActive = true;

        if (OnAntivirusExecuted != null) {
            OnAntivirusExecuted.Invoke(this, EventArgs.Empty);
        }

        StartCoroutine(handleAntivirus());
    }

    public void killAntivirus() {
        StopAllCoroutines();
    }

    private IEnumerator handleAntivirus() {
        int difficulty = Mathf.Clamp(DataManager.getDifficulty(), 0, _difficultyData.Length - 1);

        float time = 0;
        while (true) {
            FileController newFile = DesktopSystemManager.DSM.getFileOfType(FileController.FileType.Antivirus);
            if (newFile != null) {
                DesktopSystemManager.DSM.randomlyAddFile(newFile);
            }

            // Check if the win condition has been met
            if (DesktopSystemManager.DSM.activeAntivirusFileCount >= DesktopSystemManager.DSM.desktopIconSpace * AppConsts.PERCENT_OF_FILES_AS_ANTIVIRUS_TO_KILL_THE_VIRUS) {
                ZipBombManager.ZBM.killVirus();
                //DataManager.logScore(ZipBombManager.ZBM.activeDuration);
                //StartMenuController.SMC.clearAndRepopulateRecentScores();

                DesktopSystemManager.DSM.killVirusFiles();
                RecycleBinController.RBC.tryEmptyRecycleBin();
                break;
            }

            float waitTime = _difficultyData[difficulty].getWaitTime(time);
            time += waitTime;
            yield return new WaitForSeconds(waitTime);
        }

        StartCoroutine(handleRemovingAntivirusFiles());
    }

    private IEnumerator handleRemovingAntivirusFiles() {
        WaitForSeconds wfs = new WaitForSeconds(_killDurationUponVirusKilled);
        while (true) {
            if (!DesktopSystemManager.DSM.tryKillRandomAntivirusFile()) {
                break;
            }

            yield return wfs;
        }

        prepareToReAddZipBomb();
    }

    public void prepareToReAddZipBomb() {
        StartCoroutine(handleReAddingZipBomb());
    }

    private IEnumerator handleReAddingZipBomb() {
        // Start countdown until next zip bomb file appears
        yield return new WaitForSeconds(_delayToSpawnNewZipBomb);
        DesktopSystemManager.DSM.tryReAddZipBomb();
    }

    public bool isAntivirusActive {
        get {
            return _isAntivirusActive;
        }
    }

}
