using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour
{
    private const string HighpassWet = "HighpassWet";
    private const string LowpassWet = "LowpassWet";
    public AudioMixer audioMixer;
    public float duration;

    // ReSharper disable once MemberCanBePrivate.Global
    public static MusicManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void EnableHighpass()
    {
        //audioMixer.SetFloat(_highpassWet, 0f);
        audioMixer.GetFloat(HighpassWet, out var currentValue);
        StartCoroutine(SmoothTransition(currentValue, 0));
    }

    public void DisableHighpass()
    {
        //audioMixer.SetFloat(_highpassWet, -80f);
        audioMixer.GetFloat(HighpassWet, out var currentValue);
        StartCoroutine(SmoothTransition(currentValue, -80));
    }

    public void DisableHighpassInstant()
    {
        audioMixer.SetFloat(HighpassWet, -80);
    }

    public void EnableLowpass()
    {
        audioMixer.GetFloat(LowpassWet, out var currentValue);
        StartCoroutine(SmoothTransition(currentValue, 0));
    }

    public void DisableLowpassInstant()
    {
        audioMixer.SetFloat(LowpassWet, -80);
    }

    private IEnumerator SmoothTransition(float start, float end)
    {
        var elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            audioMixer.SetFloat(HighpassWet, InterpolateDecibels(start, end, elapsed / duration));
            yield return null;
        }

        audioMixer.SetFloat(HighpassWet, end);
    }

    private float InterpolateDecibels(float dB1, float dB2, float t)
    {
        var gain1 = Mathf.Pow(10f, dB1 / 20f);
        var gain2 = Mathf.Pow(10f, dB2 / 20f);

        var interpolatedGain = Mathf.Lerp(gain1, gain2, t);

        // Avoid log of zero or negative
        if (interpolatedGain <= 0.00001f)
            return -80f;

        return 20f * Mathf.Log10(interpolatedGain);
    }
}