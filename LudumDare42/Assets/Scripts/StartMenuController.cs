using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class StartMenuController : MonoBehaviour {

    public static StartMenuController SMC;

    [SerializeField] private Transform _recentScoreListingRoot;
    [SerializeField] private ScoreListing _recentScoreListingPrefab;
    [SerializeField] private Text _userNameText;

    [SerializeField] private string _fadeInTrigger;
    [SerializeField] private string _fadeOutTrigger;

    private Animator _animator;
    private bool _isOpen = false;

    private void Awake() {
        SMC = this;

        _animator = gameObject.GetComponent<Animator>();

        _userNameText.text = String.Format(AppConsts.ADMIN_FMT, AppConsts.DIFFICULTY_TO_TEXT[Mathf.Clamp(DataManager.getDifficulty(), 0, AppConsts.DIFFICULTY_TO_TEXT.Length - 1)]);
        //populateRecentScores();
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

    //public void clearAndRepopulateRecentScores() {
    //    for (int i = 0; i < _recentScoreListingRoot.childCount; i++) {
    //        Destroy(_recentScoreListingRoot.GetChild(i));
    //    }

    //    populateRecentScores();
    //}

    //private void populateRecentScores() {
    //    List<string> highScores = DataManager.getRecentScores();
    //    for (int i = 0; i < highScores.Count; i++) {
    //        ScoreListing newListing = Instantiate<ScoreListing>(_recentScoreListingPrefab, _recentScoreListingRoot);
    //        newListing._scoreText.text = highScores[i];
    //    }
    //}

}
