using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

// Central game controller that manages game state, ship tracking, and AI turns
public class GameManager : MonoBehaviour
{
    // Singleton instance for global access
    public static GameManager Instance { get; private set; }

    // Current game state (menu, placement, shooting, etc.)
    public GameState CurrentState { get; private set; }

    // Ship tracking
    public const int MAX_SHIPS = 5;
    public const int GRID_SIZE = 5;
    public static int playerShipsPlaced = 0;
    public int playerShipsRemaining = MAX_SHIPS;
    public int enemyShipsRemaining = MAX_SHIPS;

    // For map coords
    public double SelectedLatitude { get; set; }
    public double SelectedLongitude { get; set; }
    public bool LocationSelected => SelectedLatitude != 0 || SelectedLongitude != 0;
    public Texture2D MapScreenshot { get; set; }

    // Reference to the AI component
    public SimpleBattleshipAI ai;
    public bool IsAITurn { get; set; }
    public float aiInitialDelay = 2f; // Delay before AI's first turn

    // Tile Arrays
    public List<TileScript> playerTiles = new();
    public List<TileScript> enemyTiles = new();

    void Awake()
    {
        // Ensure only one GameManager exists (Singleton pattern)
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // Persist across scenes
        CurrentState = GameState.MainMenu;
    }

    void Start()
    {
        // Only initialize AI, ships will be placed when game starts
        ai = gameObject.AddComponent<SimpleBattleshipAI>();
        ai.ResetGuesses();
    }

    // Changes the current game state and handles transitions
    public void SetGameState(GameState newState)
    {
        if (CurrentState == newState) return;

        CurrentState = newState;

        // Reset game when entering placement phase
        if (newState == GameState.PlaceShips)
        {
            ResetGame();
        }

        UITextHandler.Instance.SetText("State"); // Update UI
        Debug.Log($"Game state changed to: {newState}");
    }

    // Randomly places enemy ships on their grid
    void PlaceAIShips()
    {
        // Clear existing ships first
        foreach (TileScript tile in enemyTiles) tile.hasShip = false;

        // Place MAX_SHIPS randomly on empty tiles
        for (int i = 0; i < MAX_SHIPS; i++)
        {
            TileScript[] availableTiles = enemyTiles.Where(t => !t.hasShip).ToArray();
            if (availableTiles.Length == 0) break; // Safety check

            int randomIndex = Random.Range(0, availableTiles.Length);
            availableTiles[randomIndex].hasShip = true;
            Debug.Log($"AI placed ship at index {availableTiles[randomIndex]}");
        }
    }

    // Starts AI turn after initial delay
    public void StartAITurnWithDelay()
    {
        IsAITurn = true;
        Invoke(nameof(StartAITurn), aiInitialDelay);
    }

    // Directly starts AI turn if in correct state
    private void StartAITurn()
    {
        if (CurrentState == GameState.ShootShips)
        {
            ai.TakeTurn();
        }
    }

    // Resets all game data for a new round
    void ResetGame()
    {

        // Reset all tiles in the game
        TileScript[] allTiles = FindObjectsByType<TileScript>(FindObjectsSortMode.None);
        foreach (TileScript tile in playerTiles)
        {
            tile.shot = false;
            tile.hasShip = false;
            tile.SetColor(tile.originalColor);
        }
        foreach (TileScript tile in enemyTiles)
        {
            tile.shot = false;
            tile.hasShip = false;
            tile.SetColor(tile.originalColor);
        }

        // Reset ship counts
        playerShipsRemaining = MAX_SHIPS;
        enemyShipsRemaining = MAX_SHIPS;
        playerShipsPlaced = 0;

        // Reset AI state
        ai.ResetGuesses();
        PlaceAIShips(); // Place new enemy ships
        IsAITurn = false;

    }
}