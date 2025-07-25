using UnityEngine;
using UnityEngine.UI;

public class MapImageDisplay : MonoBehaviour
{
    public RawImage mapDisplay;

    void Start()
    {
        if (GameManager.Instance.MapScreenshot != null)
        {
            mapDisplay.texture = GameManager.Instance.MapScreenshot;
        }
        else
        {
            Debug.LogWarning("No map screenshot found.");
        }
    }
}
