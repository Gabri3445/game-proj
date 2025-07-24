using UnityEngine;

public class SkyboxRotate : MonoBehaviour
{
    private const string Property = "_Rotation";
    private static readonly int Rotation = Shader.PropertyToID(Property);
    [SerializeField] private float velocity = 3.0f;
    private Light _directional;

    // Update is called once per frame
    private void Update()
    {
        RenderSettings.skybox.SetFloat(Rotation, Time.time * velocity);
        _directional.transform.rotation = Quaternion.Euler(27.6f, -(Time.time * velocity) + 168.75f, 0.0f);
    }

    private void OnEnable()
    {
        if (!_directional) _directional = GameObject.FindWithTag("Light").GetComponent<Light>();
    }
/*
    private void OnDestroy()
    {

    }

    public void ResetRotation()
    {
        directional.transform.rotation = Quaternion.Euler(27.6f, 168.75f, 0.0f);
        RenderSettings.skybox.SetFloat(Rotation, 0);
    }
    */
}