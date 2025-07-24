using UnityEngine;

public class SkyboxRotate : MonoBehaviour
{
    private const string Property = "_Rotation";
    private static readonly int Rotation = Shader.PropertyToID(Property);
    [SerializeField] private float velocity = 3.0f;
    public Light directional;

    // Update is called once per frame
    private void Update()
    {
        RenderSettings.skybox.SetFloat(Rotation, Time.time * velocity);
        directional.transform.rotation = Quaternion.Euler(27.6f, -(Time.time * velocity) + 168.75f, 0.0f);
    }

    private void OnEnable()
    {
        if (!directional) directional = GameObject.FindWithTag("Light").GetComponent<Light>();
    }

    private void OnDisable()
    {
        directional = null;
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