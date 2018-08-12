using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class ClippyController : MonoBehaviour {

    public static ClippyController CC;

    [SerializeField] private GameObject _clippy;

    [Space]

    [SerializeField] private Text _clippyText;
    [SerializeField] private GameObject _okayButton;
    [SerializeField] private GameObject _yesButton;
    [SerializeField] private GameObject _noButton;

    [Space]

    [SerializeField] private string _fadeInTrigger;
    [SerializeField] private string _popTrigger;
    [SerializeField] private string _fadeOutTrigger;

    private Animator _animator;
    private ClippyConversation _currentConversation;
    private bool _isMidConversation = false;

    private void Awake() {
        CC = this;

        _animator = gameObject.GetComponent<Animator>();
    }

    public void startConversation(ClippyConversation conversation) {
        if (_isMidConversation) {
            return;
        }

        _currentConversation = conversation;
        progressConversation();
    }

    private void progressConversation() {
        if (_currentConversation.tryContinueConversation()) {
            _clippyText.text = _currentConversation.text;

            _okayButton.SetActive(_currentConversation.okayResponse != null);

            _yesButton.SetActive(_currentConversation.yesResponse != null);

            _noButton.SetActive(_currentConversation.noResponse != null);

            if (!_isMidConversation) {
                _isMidConversation = true;

                _animator.SetTrigger(_fadeInTrigger);
            } else {
                _animator.SetTrigger(_popTrigger);
            }
        } else {
            endConversation();
        }
    }

    private void endConversation() {
        _isMidConversation = false;

        _animator.SetTrigger(_fadeOutTrigger);
    }

    public void executeOkayResponse() {
        if (_currentConversation.okayResponse != null) {
            _currentConversation.okayResponse.Invoke();
        }

        progressConversation();
    }

    public void executeYesResponse() {
        if (_currentConversation.yesResponse != null) {
            _currentConversation.yesResponse.Invoke();
        }

        progressConversation();
    }

    public void executeNoResponse() {
        if (_currentConversation.noResponse != null) {
            _currentConversation.noResponse.Invoke();
        }

        progressConversation();
    }

}

public class ClippyConversation {

    private ClippyConversationFrame[] _conversationFrames;
    private int _currentConversationIndex;

    public ClippyConversation(params ClippyConversationFrame[] conversationFrames) {
        _conversationFrames = conversationFrames;
        _currentConversationIndex = -1;
    }

    public string text {
        get {
            return _conversationFrames[_currentConversationIndex].text;
        }
    }

    public ClippyConversationFrame.ResponseDelegate okayResponse {
        get {
            return _conversationFrames[_currentConversationIndex].okayResponse;
        }
    }

    public ClippyConversationFrame.ResponseDelegate yesResponse {
        get {
            return _conversationFrames[_currentConversationIndex].yesResponse;
        }
    }

    public ClippyConversationFrame.ResponseDelegate noResponse {
        get {
            return _conversationFrames[_currentConversationIndex].noResponse;
        }
    }

    public bool tryContinueConversation() {
        _currentConversationIndex++;
        return _currentConversationIndex < _conversationFrames.Length;
    }

}

public class ClippyConversationFrame {

    public delegate void ResponseDelegate();

    public string text;
    public ResponseDelegate okayResponse;
    public ResponseDelegate yesResponse;
    public ResponseDelegate noResponse;

    public ClippyConversationFrame(string text, ResponseDelegate okayResponse, ResponseDelegate yesResponse, ResponseDelegate noResponse) {
        this.text = text;
        this.okayResponse = okayResponse;
        this.yesResponse = yesResponse;
        this.noResponse = noResponse;
    }

}
