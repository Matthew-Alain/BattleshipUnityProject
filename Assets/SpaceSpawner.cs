using UnityEngine;
using UnityEngine.InputSystem;


public class SpaceSpawner : MonoBehaviour
{

    public GameObject spacePrefab;
    private Camera mainCamera;

    void Awake()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame && false) //GameManager.Instance.CurrentState == GameState.PlaceShipsNew
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Vector2 worldPos = mainCamera.ScreenToWorldPoint(mousePos);

            Instantiate(spacePrefab, worldPos, Quaternion.identity);
            GameManager.shipsPlaced++;
            if (GameManager.shipsPlaced >= GameManager.maxShips)
            {
                GameManager.Instance.SetGameState(GameState.ShootShips);
            }
        }
    }
}
