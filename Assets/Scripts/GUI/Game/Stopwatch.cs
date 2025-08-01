using System;
using TMPro;
using UnityEngine;

public class Stopwatch : MonoBehaviour
{
    private bool _isEnabled = true;
    private TMP_Text _text;
    public float TimeElapsed { get; set; }

    private void Awake()
    {
        _text = GetComponent<TMP_Text>();
    }

    private void Update()
    {
        if (!_isEnabled) return;
        TimeElapsed += Time.deltaTime;
        _text.text = TimeToString(TimeElapsed);
    }

    public string TimeToString(float time)
    {
        var timeSpan = TimeSpan.FromSeconds(TimeElapsed);
        var minutes = timeSpan.Minutes > 0 ? timeSpan.Minutes + ":" : "";
        return $"{minutes}{timeSpan.Seconds}:{timeSpan.Milliseconds}";
    }

    //Call these two when pausing or when a level is finished
    public void Enable()
    {
        _isEnabled = true;
    }

    public void Disable()
    {
        _isEnabled = false;
    }
}