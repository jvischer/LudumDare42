using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZipBombManager : MonoBehaviour {

    public static ZipBombManager ZBM;

    [SerializeField] private float _spawnRampUpDuration;
    [SerializeField] private float _initialMinSpawnDelay;
    [SerializeField] private float _initialMaxSpawnDelay;
    [SerializeField] private float _endMinSpawnDelay;
    [SerializeField] private float _endMaxSpawnDelay;

    private void Awake() {
        ZBM = this;
    }

    public void executeVirus() {
        StartCoroutine(handleVirus());
    }

    public void killVirus() {
        StopAllCoroutines();
    }

    private IEnumerator handleVirus() {
        float time = 0;
        while (true) {
            FileController newFile = DesktopSystemManager.DSM.getFileOfType(FileController.FileType.Virus);
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

}
