using UnityEngine;

public class SkyboxRotate : MonoBehaviour
{
    private const string Property = "_Rotation";
    private static readonly int Rotation = Shader.PropertyToID(Property);
    public Light directional;
    [SerializeField] private float velocity = 3.0f;
    public bool isEnabled = true;


    // Update is called once per frame
    private void Update()
    {
        if (!isEnabled) return;
        RenderSettings.skybox.SetFloat(Rotation, Time.time * velocity);
        directional.transform.rotation = Quaternion.Euler(27.6f, -(Time.time * velocity) + 168.75f, 0.0f);
    }

    private void OnDisable()
    {
        ResetRotation();
    }

    public void ResetRotation()
    {
        directional.transform.rotation = Quaternion.Euler(27.6f, 168.75f, 0.0f);
        RenderSettings.skybox.SetFloat(Rotation, 0);
    }
}