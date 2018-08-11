using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeSystemManager : MonoBehaviour {

    [SerializeField] private Text _timeText;

    private const string AM_FORMAT = "{0}:{1} AM";
    private const string PM_FORMAT = "{0}:{1} PM";

    private int _lastDisplayedHour = -1;
    private int _lastDisplayedMinute = -1;

    private void Update() {
        DateTime now = DateTime.Now;
        if (_lastDisplayedHour != now.Hour ||
            _lastDisplayedMinute != now.Minute) {
            refreshDisplay(now);
        }
    }

    private void refreshDisplay(DateTime timeToShow) {
        _lastDisplayedHour = timeToShow.Hour;
        _lastDisplayedMinute = timeToShow.Minute;

        bool isAM = _lastDisplayedHour == 24 || _lastDisplayedHour < 12 ? true : false;
        _timeText.text = String.Format(isAM ? AM_FORMAT : PM_FORMAT, (_lastDisplayedHour > 12 ? _lastDisplayedHour - 12 : _lastDisplayedHour), _lastDisplayedMinute);
    }

}
