using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GUI
{
    public class ButtonOpacity : MonoBehaviour
    {
        public List<Button> buttons = new();
        public float delay;
        public float duration = 2.0f;
        public float delayAdd = 1.0f;

        private void Start()
        {
            var color = new Color(0.9949913f, 0.6761006f, 1.0f, 1.0f);
            foreach (var myButton in buttons.Select(t => new MyButton
                     {
                         Button = t,
                         Text = t.transform.GetChild(0).gameObject.GetComponent<TMP_Text>(),
                         Panel = t.transform.GetChild(1).gameObject.GetComponent<Image>()
                     }))
            {
                StartCoroutine(ChangeOpacity(color, duration, delay, myButton.Panel));
                StartCoroutine(ChangeOpacity(Color.white, duration, delay, myButton.Text));
                StartCoroutine(ChangeOpacity(Color.white, duration, delay, myButton.Button.GetComponent<Image>()));
                delay += delayAdd;
            }
        }

        private static IEnumerator ChangeOpacity(Color targetColor, float duration, float delay, Image image)
        {
            yield return new WaitForSeconds(delay);
            var elapsedTime = 0.0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                image.color = Color.Lerp(Color.clear, targetColor, elapsedTime / duration);
                yield return null;
            }

            image.color = targetColor;
        }

        private static IEnumerator ChangeOpacity(Color targetColor, float duration, float delay, TMP_Text text)
        {
            yield return new WaitForSeconds(delay);
            var elapsedTime = 0.0f;
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                text.color = Color.Lerp(Color.clear, targetColor, elapsedTime / duration);
                yield return null;
            }

            text.color = targetColor;
        }
    }

    internal struct MyButton
    {
        public Button Button { get; set; }
        public TMP_Text Text { get; set; }
        public Image Panel { get; set; }
    }
}