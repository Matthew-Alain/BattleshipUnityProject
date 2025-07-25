using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameState CurrentState { get; private set; }
    public static int maxShips = 5;
    public static int shipsPlaced = 0;
    public static int enemyShips = maxShips;

    // For map coords
    public double SelectedLatitude { get; set; }
    public double SelectedLongitude { get; set; }
    public bool LocationSelected => SelectedLatitude != 0 || SelectedLongitude != 0;
    public Texture2D MapScreenshot { get; set; }




    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        CurrentState = GameState.MainMenu;
    }

    public void SetGameState(GameState newState)
    {
        if (CurrentState == newState) return;

        CurrentState = newState;
        UITextHandler.Instance.SetText("State");

        Debug.Log("Game state changed to: " + newState);
    }

}
