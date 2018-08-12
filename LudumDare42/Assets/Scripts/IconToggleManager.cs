using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class IconToggleManager : MonoBehaviour {

    [SerializeField] private Sprite _trueSprite;
    [SerializeField] private Sprite _falseSprite;

    private Image _image;
    private bool _initialized = false;
    private bool _status = false;

    private void Awake() {
        if (_image == null) {
            _image = gameObject.GetComponent<Image>();
        }
    }

    public void initialize(bool status) {
        if (_image == null) {
            _image = gameObject.GetComponent<Image>();
        }

        _initialized = true;
        _status = status;
        _image.sprite = status ? _trueSprite : _falseSprite;
    }

    public void toggle() {
        if (_initialized) {
            _status = !_status;
            _image.sprite = _status ? _trueSprite : _falseSprite;
        }
    }

    public bool status {
        get {
            return _status;
        }
    }

}
