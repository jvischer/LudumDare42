using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Animator))]
public class StartMenuController : MonoBehaviour {

    public static StartMenuController SMC;

    [SerializeField] private string _fadeInTrigger;
    [SerializeField] private string _fadeOutTrigger;

    private Animator _animator;
    private bool _isOpen = false;

    private void Awake() {
        SMC = this;

        _animator = gameObject.GetComponent<Animator>();
    }

    public void toggleStartMenu() {
        _isOpen = !_isOpen;

        if (_isOpen) {
            _animator.SetTrigger(_fadeInTrigger);
        } else {
            _animator.SetTrigger(_fadeOutTrigger);
        }
    }

    public void checkStartMenuSearch(string text) {
        switch (text.ToUpper().Replace(" ", "")) {
            case "CUTTHEPOWERTOTHEBUILDING":
                powerOff();
                break;
        }
    }

    public void powerOff() {
        Application.Quit();
    }

    public void restart() {
        SceneManager.LoadScene(0);
    }

}
