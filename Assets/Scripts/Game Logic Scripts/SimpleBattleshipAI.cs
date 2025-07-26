using System.Collections.Generic;
using UnityEngine;


/// Handles AI logic for the Battleship game, including guessing positions,
/// tracking shots, and visualizing results on the player's grid.

public class SimpleBattleshipAI : MonoBehaviour
{   
    // Tracks all cells the AI has already guessed to avoid repeats
    private readonly HashSet<Vector2Int> guessedCells = new();

    // Generates a new unique guess position that hasn't been tried before
    public Vector2Int GetNextGuess()
    {
        // If all possible cells have been guessed, return invalid position
        if (guessedCells.Count >= GameManager.GRID_SIZE * GameManager.GRID_SIZE)
        {
            Debug.LogWarning("AI: No more moves available");
            GameManager.Instance.SetGameState(GameState.Defeat);
            return new Vector2Int(-1, -1);
        }

        Vector2Int guess;
        do
        {
            // Generate random grid coordinates within bounds
            guess = new Vector2Int(
                Random.Range(0, GameManager.GRID_SIZE),  // Random x (column)
                Random.Range(0, GameManager.GRID_SIZE)   // Random y (row)
            );
        } while (guessedCells.Contains(guess)); // Ensure guess is unique

        guessedCells.Add(guess); // Remember this guess
        Debug.Log($"AI has guessed {guess}");
        return guess;
    }


    // Executes the AI's turn: makes a guess, processes result, and visualizes it
    public void TakeTurn()
    {
        // Only take turns during shooting phase
        if (GameManager.Instance.CurrentState != GameState.ShootShips) return;

        GameManager.Instance.IsAITurn = true; // Lock player input

        Vector2Int guess = GetNextGuess();
        Debug.Log("GetNextGuess successful");

        bool hit = GameManager.Instance.ProcessShot(guess); // Check if guess hit a ship
        Debug.Log("ProcessShot successful");

        VisualizeAIGuess(guess, hit); // Show result on player's grid
        Debug.Log("VisualizeAIGuess successful");

        // If hit and game isn't over, take another turn after delay
        if (hit && GameManager.Instance.playerShipsRemaining == 0)
        {
            GameManager.Instance.SetGameState(GameState.Defeat);
            return;
        }
        else if (hit && GameManager.Instance.playerShipsRemaining > 0 &&
                GameManager.Instance.CurrentState == GameState.ShootShips)
        {
            UITextHandler.Instance.SetText("EnemyHit");
            Invoke(nameof(TakeTurn), 1.5f); // Chain consecutive hits
        }
        else
        {
            UITextHandler.Instance.SetText("EnemyMiss");
            GameManager.Instance.IsAITurn = false; // Turn ends when AI misses
        }
    }


    /// Visualizes the AI's guess on the player's grid with appropriate colors

    void VisualizeAIGuess(Vector2Int guess, bool hit)
    {
        // Get all player tiles using their tag
        GameObject[] playerTileObjects = GameObject.FindGameObjectsWithTag("PlayerSpaces");

        // Convert grid coordinates (0-4,0-4) to linear index (0-24)
        // Formula: (row * gridWidth) + column
        int tileIndex = guess.y * GameManager.GRID_SIZE + guess.x;

        // Validate index is within bounds
        if (tileIndex >= 0 && tileIndex < playerTileObjects.Length)
        {
            Debug.Log("Guessed tile is valid");
            Debug.Log("I'm going to guess this is where it goes wrong");
            if (playerTileObjects[tileIndex].TryGetComponent<TileScript>(out var tile))
            {
                Debug.Log("Or maybe here");
                tile.shot = true; // Mark tile as shot at
                tile.hasShip = hit; // Update whether this tile had a ship
                Debug.Log("Nope, I was wrong, that works");

                // Determine color based on hit/miss
                Color guessColor = hit ?
                    new Color(1, 0.9f, 0.2f) : // Bright yellow for hits
                    new Color(0.2f, 0.9f, 1);  // Bright cyan for misses

                tile.SetColor(guessColor);
                Debug.Log($"AI visualized {(hit ? "HIT" : "MISS")} at tile {tileIndex + 1} (grid pos: {guess.x + 1}, {guess.y + 1})");
            }
            else
            {
                Debug.LogError($"Tile at index {tileIndex} missing TileScript component");
            }
        }
        else
        {
            Debug.LogError($"Invalid tile index {tileIndex} from guess {guess}");
        }
    }

    // Resets the AI's memory of previous guesses when starting new game

    public void ResetGuesses()
    {
        guessedCells.Clear();
        Debug.Log("AI guesses reset");
    }
}