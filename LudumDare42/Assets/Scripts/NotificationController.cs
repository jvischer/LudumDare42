using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class NotificationController : MonoBehaviour {

    public static NotificationController NC;

    [SerializeField] private Text _notificationHeaderText;
    [SerializeField] private Text _notificationBodyText;

    [SerializeField] private string _fadeInTrigger;
    [SerializeField] private string _fadeOutTrigger;

    [SerializeField] private Transform _notificationNewParent;
    [SerializeField] private float _notificationDuration;

    private Animator _animator;

    private void Awake() {
        NC = this;

        _animator = gameObject.GetComponent<Animator>();
        transform.SetParent(_notificationNewParent);

        AntivirusManager.OnAntivirusExecuted += antivirusManager_OnAntivirusExecuted;
        ZipBombManager.OnZipBombKilled += zipBombManager_OnZipBombKilled;
        RecycleBinController.OnRecycleBinFilled += recycleBinController_OnRecycleBinFilled;
    }

    private void antivirusManager_OnAntivirusExecuted(object sender, EventArgs e) {
        _notificationHeaderText.text = AppConsts.NOTIFICATION_ON_ANTIVIRUS_EXECUTED_HEADER;
        _notificationBodyText.text = AppConsts.NOTIFICATION_ON_ANTIVIRUS_EXECUTED_BODY;

        StartCoroutine(handleNotificationDisplay());
    }

    private void zipBombManager_OnZipBombKilled(object sender, EventArgs e) {
        _notificationHeaderText.text = AppConsts.NOTIFICATION_ON_ZIP_BOMB_KILLED_HEADER;
        _notificationBodyText.text = AppConsts.NOTIFICATION_ON_ZIP_BOMB_KILLED_BODY;

        StartCoroutine(handleNotificationDisplay());
    }

    private void recycleBinController_OnRecycleBinFilled(object sender, EventArgs e) {
        _notificationHeaderText.text = AppConsts.NOTIFICATION_ON_RECYCLE_BIN_FILLED_HEADER;
        _notificationBodyText.text = AppConsts.NOTIFICATION_ON_RECYCLE_BIN_FILLED_BODY;

        StartCoroutine(handleNotificationDisplay());
    }

    private IEnumerator handleNotificationDisplay() {
        _animator.SetTrigger(_fadeInTrigger);

        yield return new WaitForSeconds(_notificationDuration);

        _animator.SetTrigger(_fadeOutTrigger);
    }

}
