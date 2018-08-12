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

    private void Start() {
        StartCoroutine(handleStartupScene());
    }

    private IEnumerator handleStartupScene() {
        _loadPanel.SetActive(true);
        _loginPanel.SetActive(false);

        yield return new WaitForSeconds(UnityEngine.Random.Range(_minLoadTime, _maxLoadTime));
        _loadPanel.SetActive(false);
        _loginPanel.SetActive(true);

        yield return new WaitForSeconds(UnityEngine.Random.Range(_minLoginTime, _maxLoginTime));
        SceneManager.LoadScene(1);
    }

}
