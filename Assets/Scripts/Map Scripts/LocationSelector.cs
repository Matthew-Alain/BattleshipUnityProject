using UnityEngine;
using Mapbox.Unity.Map;
using Mapbox.Examples;
using System.Collections;

public class LocationSelector : MonoBehaviour
{
    public AbstractMap map;
    public QuadTreeCameraMovement cameraMovementScript;

    public GameObject confirmButtonUI;

    // 🔹 Add this reference in the Inspector
    public GameObject reloadMapPanel;

    private void Awake()
    {
        if (cameraMovementScript == null)
            cameraMovementScript = FindObjectOfType<QuadTreeCameraMovement>();

        if (map == null)
            map = FindObjectOfType<AbstractMap>();
    }

    public void ConfirmLocation()
    {
        if (cameraMovementScript != null)
            cameraMovementScript.inputLocked = true;

        if (map != null)
        {
            var latLong = map.CenterLatitudeLongitude;
            GameManager.Instance.SelectedLatitude = latLong.x;
            GameManager.Instance.SelectedLongitude = latLong.y;

            Debug.Log($"Saved Location: Lat {latLong.x}, Lon {latLong.y}");

            StartCoroutine(CaptureMapScreenshotAndLoadScene());
        }
    }

    private IEnumerator CaptureMapScreenshotAndLoadScene()
    {
        // 🔹 Disable the panel so it's hidden in the screenshot
        if (reloadMapPanel != null)
            reloadMapPanel.SetActive(false);

        yield return new WaitForEndOfFrame();

        Texture2D screenImage = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        screenImage.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenImage.Apply();

        GameManager.Instance.MapScreenshot = screenImage;

        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(2);
    }
}
