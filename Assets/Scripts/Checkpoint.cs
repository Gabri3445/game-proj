using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class Checkpoint : MonoBehaviour
{
    public int checkpointNumber;
    [FormerlySerializedAs("Text")] public TMP_Text text;
    public float duration;

    private void Awake()
    {
        text.gameObject.SetActive(false);
    }

    public IEnumerator ShowCheckPointText()
    {
        text.gameObject.SetActive(true);
        yield return new WaitForSeconds(duration);
        text.gameObject.SetActive(false);
    }
}