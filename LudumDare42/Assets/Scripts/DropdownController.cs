using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropdownController : MonoBehaviour {

    public static DropdownController DC;

    [SerializeField] private GameObject _bottomBarCover;
    [SerializeField] private RectTransform _dropdownRoot;
    [SerializeField] private DropdownOptionController[] _dropdownOptionPool;

    private void Awake() {
        DC = this;
    }

    public void displayDropdownForFile(FileController file) {
        for (int i = 0; i < _dropdownOptionPool.Length; i++) {
            // First clear all listeners
            _dropdownOptionPool[i].optionButton.onClick.RemoveAllListeners();

            // If the index matches an option, display it
            if (i < file.fileOptions.Length) {
                _dropdownOptionPool[i].optionButton.onClick.AddListener(file.fileOptions[i].fireClickedEvent);
                _dropdownOptionPool[i].optionButton.onClick.AddListener(hideDropdown);

                _dropdownOptionPool[i].optionText.text = file.fileOptions[i].optionText;

                _dropdownOptionPool[i].gameObject.SetActive(true);
            }
            // Else disable the option
            else {
                _dropdownOptionPool[i].gameObject.SetActive(false);
            }
        }

        _bottomBarCover.SetActive(true);

        _dropdownRoot.position = Input.mousePosition;
        _dropdownRoot.gameObject.SetActive(true);
    }

    public void displayDropdownForDesktop(DesktopSystemManager desktopSystemManager) {
        for (int i = 0; i < _dropdownOptionPool.Length; i++) {
            // First clear all listeners
            _dropdownOptionPool[i].optionButton.onClick.RemoveAllListeners();

            // If the index matches an option, display it
            if (i < desktopSystemManager.desktopOptions.Length) {
                _dropdownOptionPool[i].optionButton.onClick.AddListener(desktopSystemManager.desktopOptions[i].fireClickedEvent);
                _dropdownOptionPool[i].optionButton.onClick.AddListener(hideDropdown);

                _dropdownOptionPool[i].optionText.text = desktopSystemManager.desktopOptions[i].optionText;

                _dropdownOptionPool[i].gameObject.SetActive(true);
            }
            // Else disable the option
            else {
                _dropdownOptionPool[i].gameObject.SetActive(false);
            }
        }

        _bottomBarCover.SetActive(true);

        _dropdownRoot.position = Input.mousePosition;
        _dropdownRoot.gameObject.SetActive(true);
    }

    public void hideDropdown() {
        _bottomBarCover.SetActive(false);

        _dropdownRoot.gameObject.SetActive(false);
    }

}
