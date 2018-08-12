using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnzipController : MonoBehaviour {

    public static UnzipController UC;

    [SerializeField] private GameObject _panel;

    [SerializeField] private Text _zipFileNameText;
    [SerializeField] private Image _progressBar;

    private void Awake() {
        UC = this;

        _panel.SetActive(false);
    }

    public void displayForFile(FileController file) {
        _zipFileNameText.text = file.name;
        _progressBar.fillAmount = 0;

        _panel.SetActive(true);

        StartCoroutine(handleUnzip());
    }

    private IEnumerator handleUnzip() {
        float time = 0;
        while (time < AppConsts.FILE_UNZIP_DURATION) {
            _progressBar.fillAmount = Mathf.Clamp01(time / AppConsts.FILE_UNZIP_DURATION);

            time += Time.deltaTime;
            yield return null;
        }

        _panel.SetActive(false);
    }

}
