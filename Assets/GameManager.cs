using UnityEngine;
using System.Linq;

// Central game controller that manages game state, ship tracking, and AI turns
public class GameManager : MonoBehaviour
{
    // Singleton instance for global access
    public static GameManager Instance { get; private set; }

    // Current game state (menu, placement, shooting, etc.)
    public GameState CurrentState { get; private set; }
    public static int maxShips = 5;
    public static int shipsPlaced = 0;
    public static int enemyShips = maxShips;

    // For map coords
    public double SelectedLatitude { get; set; }
    public double SelectedLongitude { get; set; }
    public bool LocationSelected => SelectedLatitude != 0 || SelectedLongitude != 0;
    public Texture2D MapScreenshot { get; set; }




    // Reference to the AI component
    public SimpleBattleshipAI ai;
    public bool IsAITurn { get; set; }

    // Player ship tracking
    public int totalPlayerShipsPlaced = 0;
    public const int MAX_SHIPS = 5; // Total ships per player
    public int playerShipsRemaining = MAX_SHIPS;
    public int enemyShipsRemaining = MAX_SHIPS;

    // AI turn timing control
    public bool aiHasTakenFirstTurn = false;
    public float aiInitialDelay = 2f; // Delay before AI's first turn

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
        ai.gridSize = 5;
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
        TileScript[] allEnemyTiles = FindObjectsByType<TileScript>(FindObjectsSortMode.None)
            .Where(t => t.CompareTag("EnemySpaces"))
            .ToArray();

        foreach (TileScript tile in allEnemyTiles) tile.hasShip = false;

        // Place MAX_SHIPS randomly on empty tiles
        for (int i = 0; i < MAX_SHIPS; i++)
        {
            TileScript[] availableTiles = allEnemyTiles.Where(t => !t.hasShip).ToArray();
            if (availableTiles.Length == 0) break; // Safety check

            int randomIndex = Random.Range(0, availableTiles.Length);
            availableTiles[randomIndex].hasShip = true;
            Debug.Log($"AI placed ship at {availableTiles[randomIndex].transform.position}");
        }
    }

    // Processes a shot at the given coordinates on player's grid
    // Returns true if hit, false if miss
    public bool GetShot(Vector2Int pos)
    {
        // Get all player tiles by tag
        TileScript[] playerTiles = GameObject.FindGameObjectsWithTag("PlayerSpaces")
                               .Select(go => go.GetComponent<TileScript>())
                               .ToArray();

        // Convert grid coordinates to tile index (0-24)
        int tileIndex = pos.y * 5 + pos.x;

        if (tileIndex >= 0 && tileIndex < playerTiles.Length)
        {
            TileScript tile = playerTiles[tileIndex];
            if (!tile.shot)
            {
                tile.shot = true;
                if (tile.hasShip) // Successful hit
                {
                    tile.SetColor(Color.red);
                    playerShipsRemaining--;
                    Debug.Log($"AI hit at tile {tileIndex + 1}");

                    // Immediate defeat check
                    if (playerShipsRemaining <= 0)
                    {
                        SetGameState(GameState.Defeat);
                    }
                    return true;
                }
                // Miss
                tile.SetColor(Color.gray);
                return false;
            }
        }
        return false; // Invalid shot
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
        // Reset ship counts
        totalPlayerShipsPlaced = 0;
        playerShipsRemaining = MAX_SHIPS;
        enemyShipsRemaining = MAX_SHIPS;

        // Reset AI state
        aiHasTakenFirstTurn = false;
        ai.ResetGuesses();
        PlaceAIShips(); // Place new enemy ships

        // Reset all tiles in the game
        TileScript[] allTiles = FindObjectsByType<TileScript>(FindObjectsSortMode.None);
        foreach (TileScript tile in allTiles)
        {
            tile.shot = false;
            tile.SetColor(tile.originalColor);
        }
    }
}