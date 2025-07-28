using UnityEngine;

// Attach this to your Camera
public class CameraScaler : MonoBehaviour
{

    private float sceneWidth;
    Camera _camera;

    void Start()
    {
        _camera = Camera.main;
    }

    void Update()
    {
        if (Application.isMobilePlatform)
        {
            if (Screen.orientation == ScreenOrientation.LandscapeLeft || Screen.orientation == ScreenOrientation.LandscapeRight)
            {
                sceneWidth = 7f;
            }
            else
            {
                sceneWidth = 20f;
            }
        }
        else
        {
            sceneWidth = 5f;
        }
        //float unitsPerPixel = sceneWidth / Screen.width;
        //float desiredHalfHeight = 0.25f * unitsPerPixel * Screen.height;
        _camera.orthographicSize = sceneWidth;
    }
}

