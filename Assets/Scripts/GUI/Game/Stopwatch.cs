using System;
using TMPro;
using UnityEngine;

public class Stopwatch : MonoBehaviour
{
    private TMP_Text _text;
    private float _time; //TODO: store this in the savefile? maybe for each level?
    private bool _isEnabled = true;
    
    private void Awake()
    {
        _text =  GetComponent<TMP_Text>();
    }
    
    private void Update()
    {
        if (!_isEnabled) return;
        _time += Time.deltaTime;
        var timeSpan = TimeSpan.FromSeconds(_time);
        var minutes = timeSpan.Minutes > 0 ? timeSpan.Minutes.ToString() + ":" : "";
        _text.text = $"{minutes}{timeSpan.Seconds}:{timeSpan.Milliseconds}";
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
