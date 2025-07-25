using UnityEngine;
using Mapbox.Unity.Map;

public class MapInitializer : MonoBehaviour
{
    public AbstractMap map;

    void Start()
    {
        if (map == null)
        {
            map = FindObjectOfType<AbstractMap>();
        }

        if (GameManager.Instance != null && map != null && GameManager.Instance.LocationSelected)
        {
            map.Initialize(new Mapbox.Utils.Vector2d(GameManager.Instance.SelectedLatitude, GameManager.Instance.SelectedLongitude), map.AbsoluteZoom);
            Debug.Log($"Map initialized at Lat: {GameManager.Instance.SelectedLatitude}, Lon: {GameManager.Instance.SelectedLongitude}");

            // Rotate map after initialization
            map.transform.rotation = Quaternion.Euler(0, 90, 0); // Example: rotate 90 degrees on Y axis
        }
        else
        {
            Debug.LogWarning("Map or GameManager missing, or no location selected.");
        }
    }
}
