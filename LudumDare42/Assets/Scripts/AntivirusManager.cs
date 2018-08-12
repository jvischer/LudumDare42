using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntivirusManager : MonoBehaviour {

    public static AntivirusManager AM;

    public static EventHandler OnAntivirusExecuted;

    [SerializeField] private float _scanningVirusDuration;

    [Space]

    [SerializeField] private float _spawnRampUpDuration;
    [SerializeField] private float _initialMinSpawnDelay;
    [SerializeField] private float _initialMaxSpawnDelay;
    [SerializeField] private float _endMinSpawnDelay;
    [SerializeField] private float _endMaxSpawnDelay;

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
        float time = 0;
        while (true) {
            FileController newFile = DesktopSystemManager.DSM.getFileOfType(FileController.FileType.Antivirus);
            if (newFile != null) {
                DesktopSystemManager.DSM.randomlyAddFile(newFile);
            }

            float progress = Mathf.Clamp01(time / _spawnRampUpDuration);
            float minSpawnDelayDelta = _endMinSpawnDelay - _initialMinSpawnDelay;
            float maxSpawnDelayDelta = _endMaxSpawnDelay - _initialMaxSpawnDelay;
            float waitTime = UnityEngine.Random.Range(_initialMinSpawnDelay + progress * minSpawnDelayDelta, _initialMaxSpawnDelay + progress * maxSpawnDelayDelta);
            time += waitTime;
            yield return new WaitForSeconds(waitTime);
        }
    }

    public bool isAntivirusActive {
        get {
            return _isAntivirusActive;
        }
    }

}
