using UnityEngine;

// Attach this to your Camera
public class CameraScaler : MonoBehaviour
{
    public float targetAspect = 16f / 9f; // Your design aspect ratio
    public float orthographicSize = 5f;   // Default camera size at target aspect

    void Start()
    {
        float windowAspect = (float)Screen.width / (float)Screen.height;
        float scale = windowAspect / targetAspect;
        Camera.main.orthographicSize = orthographicSize / scale;
    }
}