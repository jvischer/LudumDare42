using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartupSceneController : MonoBehaviour {

    [SerializeField] private float _minLoadTime;
    [SerializeField] private float _maxLoadTime;
    [SerializeField] private GameObject _loadPanel;

    [Space]

    [SerializeField] private float _minLoginTime;
    [SerializeField] private float _maxLoginTime;
    [SerializeField] private GameObject _loginPanel;
    [SerializeField] private GameObject _userPanel;
    [SerializeField] private GameObject _loggingInPanel;

    private void Awake() {
        DataManager.verifyData();
    }

    private void Start() {
        StartCoroutine(handleStartupScene());
    }

    private IEnumerator handleStartupScene() {
        _loadPanel.SetActive(true);
        _loginPanel.SetActive(false);

        yield return new WaitForSeconds(UnityEngine.Random.Range(_minLoadTime, _maxLoadTime));
        _loadPanel.SetActive(false);
        _loginPanel.SetActive(true);

        _userPanel.SetActive(true);
        _loggingInPanel.SetActive(false);
    }

    public void onPickedLoginOption(int difficultyRating) {
        DataManager.setDifficulty(difficultyRating);

        _userPanel.SetActive(false);
        _loggingInPanel.SetActive(true);

        StartCoroutine(handleLoginOption());
    }

    private IEnumerator handleLoginOption() {
        yield return new WaitForSeconds(UnityEngine.Random.Range(_minLoginTime, _maxLoginTime));
        SceneManager.LoadScene(1);
    }

    public void powerOff() {
        Application.Quit();
    }

    public void restart() {
        SceneManager.LoadScene(0);
    }

}
