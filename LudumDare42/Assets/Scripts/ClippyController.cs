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
        progressConversation(0);
    }

    private void progressConversation(int indexChange) {
        if (_currentConversation.tryContinueConversation(indexChange)) {
            _clippyText.text = _currentConversation.text;

            _okayButton.SetActive(_currentConversation.frameType == ClippyConversationFrame.ConversationFrameType.Okay);

            _yesButton.SetActive(_currentConversation.frameType == ClippyConversationFrame.ConversationFrameType.Unique);
            _noButton.SetActive(_currentConversation.frameType == ClippyConversationFrame.ConversationFrameType.Unique);

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

        progressConversation(_currentConversation.okayResponseIndexChange);
    }

    public void executeYesResponse() {
        if (_currentConversation.yesResponse != null) {
            _currentConversation.yesResponse.Invoke();
        }

        progressConversation(_currentConversation.yesResponseIndexChange);
    }

    public void executeNoResponse() {
        if (_currentConversation.noResponse != null) {
            _currentConversation.noResponse.Invoke();
        }

        progressConversation(_currentConversation.noResponseIndexChange);
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

    public int okayResponseIndexChange {
        get {
            return _conversationFrames[_currentConversationIndex].okayResponseIndexChange;
        }
    }

    public ClippyConversationFrame.ResponseDelegate yesResponse {
        get {
            return _conversationFrames[_currentConversationIndex].yesResponse;
        }
    }

    public int yesResponseIndexChange {
        get {
            return _conversationFrames[_currentConversationIndex].yesResponseIndexChange;
        }
    }

    public ClippyConversationFrame.ResponseDelegate noResponse {
        get {
            return _conversationFrames[_currentConversationIndex].noResponse;
        }
    }

    public int noResponseIndexChange {
        get {
            return _conversationFrames[_currentConversationIndex].noResponseIndexChange;
        }
    }

    public ClippyConversationFrame.ConversationFrameType frameType {
        get {
            return _conversationFrames[_currentConversationIndex].frameType;
        }
    }

    public bool tryContinueConversation(int indexChange) {
        _currentConversationIndex += (1 + indexChange);
        return _currentConversationIndex < _conversationFrames.Length;
    }

}

public class ClippyConversationFrame {

    public delegate void ResponseDelegate();

    public string text;
    public ResponseDelegate okayResponse;
    public int okayResponseIndexChange;
    public ResponseDelegate yesResponse;
    public int yesResponseIndexChange;
    public ResponseDelegate noResponse;
    public int noResponseIndexChange;

    public ConversationFrameType frameType;

    public ClippyConversationFrame(string text, ResponseDelegate okayResponse, int okayResponseIndexChange) {
        this.text = text;
        this.okayResponse = okayResponse;
        this.okayResponseIndexChange = okayResponseIndexChange;

        this.frameType = ConversationFrameType.Okay;
    }

    public ClippyConversationFrame(string text, ResponseDelegate yesResponse, int yesResponseIndexChange,
                                                ResponseDelegate noResponse, int noResponseIndexChange) {
        this.text = text;
        this.yesResponse = yesResponse;
        this.yesResponseIndexChange = yesResponseIndexChange;
        this.noResponse = noResponse;
        this.noResponseIndexChange = noResponseIndexChange;

        this.frameType = ConversationFrameType.Unique;
    }

    public enum ConversationFrameType {
        Okay, Unique,
    }

}
