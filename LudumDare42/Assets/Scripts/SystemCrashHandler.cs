using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SystemCrashHandler : MonoBehaviour {

    public static SystemCrashHandler SCH;

    [SerializeField] private GameObject _BSODPanel;
    [SerializeField] private float _BSODDumpDuration = 5.0F;

    private void Awake() {
        SCH = this;
    }

    public void crashSystem() {
        _BSODPanel.SetActive(true);
        // TODO: Play SFX
        StartCoroutine(killSystemIn(_BSODDumpDuration));
    }

    private IEnumerator killSystemIn(float duration) {
        yield return new WaitForSeconds(duration);
        SceneManager.LoadScene(0);
    }

}
