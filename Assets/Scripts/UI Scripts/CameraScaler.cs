using UnityEngine;

public class CameraScaler : MonoBehaviour
{
    private Camera _camera;

    void Start()
    {
        _camera = Camera.main;
    }

    void Update()
    {
        float aspectRatio = (float)Screen.width / Screen.height;

        // Mobile Handling
        if (Application.isMobilePlatform)
        {
            if (Screen.orientation == ScreenOrientation.LandscapeLeft || Screen.orientation == ScreenOrientation.LandscapeRight)
            {
                _camera.orthographicSize = 4.9f;
            }
            else
            {
                _camera.orthographicSize = 21f;
            }
        }

        // Check for 16:10 or narrower (taller) ratios
        else if (aspectRatio <= 1.6f)
        {
            _camera.orthographicSize = 5.5f;
        }
        
        // Default desktop
        else
        {
            _camera.orthographicSize = 5.1f;
        }
    }
}