using TMPro;
using UnityEngine;

// Handles all UI text updates in the game
public class UITextHandler : MonoBehaviour
{
    // Singleton instance for easy access from other scripts
    public static UITextHandler Instance { get; private set; }

    // Reference to the TextMeshPro component
    private TextMeshProUGUI tmp;

    // Set up singleton pattern when object awakens
    void Awake()
    {
        Instance = this;
    }

    // Get reference to text component when game starts
    void Start()
    {
        tmp = GetComponent<TextMeshProUGUI>();
    }

    // Updates UI text based on the provided message key
    public void SetText(string newText)
    {
        switch (newText)
        {
            case "State":
                UpdateStateText(); // Special case - updates based on game state
                break;
            case "PlacedShip":
                tmp.text = $"Ships placed: {GameManager.Instance.totalPlayerShipsPlaced}/{GameManager.MAX_SHIPS}";
                break;
            case "PlaceWrongSide":
                tmp.text = $"Place on your side only! Ships placed: {GameManager.Instance.totalPlayerShipsPlaced}/{GameManager.MAX_SHIPS}";
                break;
            case "PlaceSameSpace":
                tmp.text = $"Space occupied! Ships placed: {GameManager.Instance.totalPlayerShipsPlaced}/{GameManager.MAX_SHIPS}";
                break;
            case "ShootWrongSide":
                tmp.text = "Aim for enemy waters!";
                break;
            case "ShootSameSpace":
                tmp.text = "You already shot here!";
                break;
            case "Hit":
                tmp.text = "Direct hit! Shoot again!";
                break;
                // Note: No default case needed since we want silent failure
        }
    }

    // Updates text based on current game state
    private void UpdateStateText()
    {
        switch (GameManager.Instance.CurrentState)
        {
            case GameState.MainMenu:
                tmp.text = "Welcome! Click 'Start Game' to begin!";
                break;
            case GameState.PlaceShips:
                tmp.text = $"Place your ships ({GameManager.Instance.totalPlayerShipsPlaced}/{GameManager.MAX_SHIPS})";
                break;
            case GameState.ShootShips:
                tmp.text = "Attack enemy waters!";
                break;
            case GameState.Victory:
                tmp.text = "Victory! All enemy ships sunk!";
                break;
            case GameState.Defeat:
                tmp.text = "Defeat! Your fleet was destroyed!";
                break;
        }
    }
}