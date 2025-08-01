using TMPro;
using UnityEngine;

// Handles all UI text updates in the game
public class UITextHandler : MonoBehaviour
{
    // Singleton instance for easy access from other scripts
    public static UITextHandler Instance { get; private set; }

    // Reference to the TextMeshPro component
    private TextMeshProUGUI tmp;

    // Current active temporary message
    private string currentTemporaryMessage = null;

    // Tracks previous game state to prevent duplicate audio triggers
    private GameState lastState = GameState.StartGame;

    // Set up singleton pattern when object awakens
    void Awake()
    {
        Instance = this;
    }

    // Get reference to text component when game starts
    void Start()
    {
        tmp = GetComponent<TextMeshProUGUI>();
        // Play initial background music
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayBackgroundMusic();
        }
    }

    // Updates UI text based on the provided message key
    public void SetText(string newText)
    {
        switch (newText)
        {
            case "State":
                UpdateStateText();
                currentTemporaryMessage = null;
                break;
            default:
                // Store temporary message until next state change
                currentTemporaryMessage = newText;
                UpdateDisplay();
                break;
        }
    }

    // Updates display based on current message
    private void UpdateDisplay()
    {
        if (currentTemporaryMessage != null)
        {
            switch (currentTemporaryMessage)
            {
                case "PlacedShip":
                    tmp.text = $"Ship placed! ({GameManager.playerShipsPlaced}/{GameManager.MAX_SHIPS})";
                    break;
                case "PlaceWrongSide":
                    tmp.text = $"Place on your side only! ({GameManager.playerShipsPlaced}/{GameManager.MAX_SHIPS})";
                    break;
                case "PlaceSameSpace":
                    tmp.text = $"Space occupied! ({GameManager.playerShipsPlaced}/{GameManager.MAX_SHIPS})";
                    break;
                case "ShootWrongSide":
                    tmp.text = "Aim for enemy waters!";
                    break;
                case "ShootSameSpace":
                    tmp.text = "You already shot here!";
                    break;
                case "Miss":
                    tmp.text = "Miss! Enemy's turn...";
                    break;
                case "Hit":
                    tmp.text = "Direct hit! Shoot again!";
                    break;
                case "EnemyMiss":
                    tmp.text = "Enemy Misses! Your turn...";
                    break;
                case "EnemyHit":
                    tmp.text = "Enemy hit your ship! Another shot is incoming!";
                    break;
            }
        }
        else
        {
            UpdateStateText();
        }
    }

    // Updates text based on current game state
    private void UpdateStateText()
    {
        if (GameManager.Instance == null) return;

        var currentState = GameManager.Instance.CurrentState;

        // Only update if state actually changed
        if (currentState == lastState) return;
        lastState = currentState;

        switch (currentState)
        {
            case GameState.StartGame:
                tmp.text = "Welcome! Click 'Start Game' to begin!";
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlayBackgroundMusic();
                }
                break;
            case GameState.PlaceShips:
                tmp.text = $"Place your ships on the left grid ({GameManager.playerShipsPlaced}/{GameManager.MAX_SHIPS})";
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlayBackgroundMusic();
                }
                break;
            case GameState.ShootShips:
                tmp.text = "Attack enemy waters!";
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlayBackgroundMusic();
                }
                break;
            case GameState.Victory:
                tmp.text = "Victory! All enemy ships sunk!";
                UIButtonHandler.Instance.ToggleButton();
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlayVictoryMusic();
                }
                break;
            case GameState.Defeat:
                tmp.text = "Defeat! Your fleet was destroyed!";
                UIButtonHandler.Instance.ToggleButton();
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlayDefeatMusic();
                }
                break;
        }
    }
}