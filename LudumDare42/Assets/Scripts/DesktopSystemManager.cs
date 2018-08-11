using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesktopSystemManager : MonoBehaviour {

    [SerializeField] private RectTransform[] _defaultIcons;

    [Space]

    [SerializeField] private RectTransform[] _virusIconPool;
    [SerializeField] private RectTransform[] _antiVirusIconPool;

    private RectTransform[] _desktopIconPlaceholders;
    private HashSet<int> _availableIndices = new HashSet<int>();

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

    //private void Update() {
    //    if (Input.GetKeyDown(KeyCode.LeftWindows) || Input.GetKeyDown(KeyCode.RightWindows)) {
    //        // TODO: Alternate way to click the start button IF necessary
    //    }
    //}

}
