using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class DesktopSystemManager : MonoBehaviour {

    [SerializeField] private RectTransform[] _defaultIcons;

    [Space]

    [SerializeField] private FileOption[] _desktopOptions;
    [SerializeField] private Sprite[] _desktopBGOptions;

    [Space]

    [SerializeField] private RectTransform[] _virusIconPool;
    [SerializeField] private RectTransform[] _antiVirusIconPool;

    private Image _backgroundImage;
    private RectTransform[] _desktopIconPlaceholders;
    private HashSet<int> _availableIndices = new HashSet<int>();
    private int _currentDesktopBG = 0;

    private void Awake() {
        _backgroundImage = gameObject.GetComponent<Image>();
    }

    private IEnumerator Start() {
        yield return null;
        
        _desktopIconPlaceholders = new RectTransform[transform.childCount];
        for (int i = 0; i < _desktopIconPlaceholders.Length; i++) {
            _desktopIconPlaceholders[i] = transform.GetChild(i).GetComponent<RectTransform>();

            _availableIndices.Add(i);
        }

        for (int i = 0; i < _defaultIcons.Length; i++) {
            _availableIndices.Remove(i);

            _defaultIcons[i].position = _desktopIconPlaceholders[i].position;
            _defaultIcons[i].gameObject.SetActive(true);
        }
    }

    public void clickDesktop(BaseEventData baseEventData) {
        PointerEventData pointerEventData = baseEventData as PointerEventData;

        if (pointerEventData.button == PointerEventData.InputButton.Right) {
            // Open right click options for the desktop
            DropdownController.DC.displayDropdownForDesktop(this);
        }
    }

    public void rotateDesktopBackground() {
        _currentDesktopBG++;
        _currentDesktopBG %= _desktopBGOptions.Length;
        _backgroundImage.sprite = _desktopBGOptions[_currentDesktopBG];
    }

    //private void Update() {
    //    if (Input.GetKeyDown(KeyCode.LeftWindows) || Input.GetKeyDown(KeyCode.RightWindows)) {
    //        // TODO: Alternate way to click the start button IF necessary
    //    }
    //}

    public FileOption[] desktopOptions {
        get {
            return _desktopOptions;
        }
    }

}
