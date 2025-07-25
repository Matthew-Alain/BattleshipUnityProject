using UnityEngine;

public class LocationTester : MonoBehaviour
{
    void Start()
    {
        Debug.Log("Selected Latitude Battleship: " + GameManager.Instance.SelectedLatitude);
        Debug.Log("Selected Longitude Battleship: " + GameManager.Instance.SelectedLongitude);
    }
}
