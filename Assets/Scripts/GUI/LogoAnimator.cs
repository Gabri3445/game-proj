using System.Collections;
using UnityEngine;

namespace GUI
{
    public class LogoAnimator : MonoBehaviour
    {
        public Vector3 targetScale = new(1.2f, 1.2f, 1);
        public float duration = 2.0f;
        private Vector3 _originalScale;
        private RectTransform _rectTransform;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {
            _rectTransform = gameObject.GetComponent<RectTransform>();
            _originalScale = _rectTransform.localScale;
            StartCoroutine(Pulse());
        }

        private IEnumerator Pulse()
        {
            while (true)
            {
                yield return StartCoroutine(ScaleLerp(_originalScale, targetScale, duration));
                yield return StartCoroutine(ScaleLerp(targetScale, _originalScale, duration));
            }
            // ReSharper disable once IteratorNeverReturns
        }

        private IEnumerator ScaleLerp(Vector3 fromScale, Vector3 toScale, float durationLerp)
        {
            var elapsed = 0f;
            while (elapsed < durationLerp)
            {
                _rectTransform.localScale = Vector3.Lerp(fromScale, toScale, elapsed / durationLerp);
                elapsed += Time.deltaTime;
                yield return null;
            }
        }
    }
}