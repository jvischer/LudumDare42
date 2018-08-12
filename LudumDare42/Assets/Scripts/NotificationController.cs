using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class NotificationController : MonoBehaviour {

    public static NotificationController NC;

    [SerializeField] private string _fadeInTrigger;
    [SerializeField] private string _fadeOutTrigger;

    [SerializeField] private float _notificationDuration;

    private Animator _animator;

    private void Awake() {
        NC = this;

        _animator = gameObject.GetComponent<Animator>();

        AntivirusManager.OnAntivirusExecuted += antivirusManager_OnAntivirusExecuted;
    }

    private void antivirusManager_OnAntivirusExecuted(object sender, EventArgs e) {
        StartCoroutine(handleVirusFoundNotification());
    }

    private IEnumerator handleVirusFoundNotification() {
        _animator.SetTrigger(_fadeInTrigger);

        yield return new WaitForSeconds(_notificationDuration);

        _animator.SetTrigger(_fadeOutTrigger);
    }

}
