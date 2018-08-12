using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressBarLoopController : MonoBehaviour {

    [SerializeField] private RectTransform _fillStartPos;
    [SerializeField] private RectTransform _fillEndPos;
    [SerializeField] private RectTransform _fillRectTransform;

    [Space]

    [SerializeField] private float _cycleDuration = 2.0F;
    [SerializeField] private float _cycleCooldown = 0.5F;

    private void Start() {
        StartCoroutine(handleProgressBarLooping());
    }

    private IEnumerator handleProgressBarLooping() {
        _fillRectTransform.position = _fillStartPos.position;

        while (true) {
            // Complete a cycle
            float time = 0;
            Vector3 positionDelta = _fillEndPos.position - _fillStartPos.position;
            while (time < _cycleDuration) {
                float progress = Mathf.Clamp01(time / _cycleDuration);
                _fillRectTransform.position = _fillStartPos.position + progress * positionDelta;

                time += Time.deltaTime;
                yield return null;
            }
            _fillRectTransform.position = _fillStartPos.position;

            // Then wait the cooldown before doing a cycle
            yield return new WaitForSeconds(_cycleCooldown);
        }
    }
    
}
