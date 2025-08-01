using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;

public class MusicManager : MonoBehaviour
{
    private const string HighpassWet = "HighpassWet";
    private const string LowpassWet = "LowpassWet";
    [FormerlySerializedAs("microwaveSfx")] public AudioSource microwaveBeepSfx;
    public AudioSource microwaveSfx;
    public AudioClip explosionSfx;
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
        PlayMicrowaveSfx();
    }

    public void EnableHighpass()
    {
        //audioMixer.SetFloat(_highpassWet, 0f);
        audioMixer.GetFloat(HighpassWet, out var currentValue);
        StartCoroutine(SmoothTransition(HighpassWet, currentValue, 0));
    }

    public void DisableHighpass()
    {
        //audioMixer.SetFloat(_highpassWet, -80f);
        audioMixer.GetFloat(HighpassWet, out var currentValue);
        StartCoroutine(SmoothTransition(HighpassWet, currentValue, -80));
    }

    public void DisableHighpassInstant()
    {
        audioMixer.SetFloat(HighpassWet, -80);
    }

    public void EnableLowpass()
    {
        audioMixer.GetFloat(LowpassWet, out var currentValue);
        StartCoroutine(SmoothTransition(LowpassWet, currentValue, 0));
    }

    public void DisableLowpass()
    {
        audioMixer.GetFloat(LowpassWet, out var currentValue);
        StartCoroutine(SmoothTransition(LowpassWet, currentValue, -80));
    }

    public void DisableLowpassInstant()
    {
        audioMixer.SetFloat(LowpassWet, -80);
    }

    private IEnumerator SmoothTransition(string value, float start, float end)
    {
        var elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            audioMixer.SetFloat(value, InterpolateDecibels(start, end, elapsed / duration));
            yield return null;
        }

        audioMixer.SetFloat(value, end);
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

    public void PlayMicrowaveBeepSfx()
    {
        //stop the microwave drone noise
        microwaveSfx.Stop();
        microwaveBeepSfx.Play();
    }

    public void PlayMicrowaveSfx()
    {
        microwaveSfx.Play();
    }

    public void PlayExplosion()
    {
        microwaveBeepSfx.PlayOneShot(explosionSfx);
    }
}