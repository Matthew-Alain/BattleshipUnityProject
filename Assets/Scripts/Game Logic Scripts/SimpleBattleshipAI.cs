using System.Collections.Generic;
using UnityEngine;


/// Handles AI logic for the Battleship game, including guessing positions,
/// tracking shots, and visualizing results on the player's grid.

public class SimpleBattleshipAI : MonoBehaviour
{   
    // Tracks all cells the AI has already guessed to avoid repeats
    private readonly HashSet<int> guessedCells = new();

    // Generates a new unique guess position that hasn't been tried before
    public int GetNextGuess()
    {
        // If all possible cells have been guessed, return invalid position
        if (guessedCells.Count >= GameManager.GRID_SIZE * GameManager.GRID_SIZE)
        {
            Debug.LogWarning("AI: No more moves available");
            GameManager.Instance.SetGameState(GameState.Defeat);
            return -1;
        }

        int guess;
        do
        {
            // Generate random grid coordinates within bounds
            guess = Random.Range(0, GameManager.Instance.playerTiles.Count);
        } while (guessedCells.Contains(guess)); // Ensure guess is unique

        guessedCells.Add(guess); // Remember this guess
        Debug.Log($"AI has guessed index {guess}");
        return guess;
    }


    // Executes the AI's turn: makes a guess, processes result, and visualizes it
    public void TakeTurn()
    {
        // Only take turns during shooting phase
        if (GameManager.Instance.CurrentState != GameState.ShootShips) return;

        GameManager.Instance.IsAITurn = true; // Lock player input

        int guess = GetNextGuess();
        Debug.Log("GetNextGuess successful");

        GameManager.Instance.playerTiles[guess].GetClicked();
        Debug.Log("GetClicked successfully");
    }

    // Resets the AI's memory of previous guesses when starting new game
    public void ResetGuesses()
    {
        guessedCells.Clear();
        Debug.Log("AI guesses reset");
    }
}