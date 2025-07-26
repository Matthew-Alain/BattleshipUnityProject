using UnityEngine;
using UnityEngine.InputSystem;

// Handles all tile interactions - ship placement, shooting, and visual feedback
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))] // Using BoxCollider2D for more precise clicking
public class TileScript : MonoBehaviour
{
    private Camera mainCamera;
    private SpriteRenderer spriteRenderer;
    public Color originalColor; // Stores the tile's default color
    public bool hasShip = false; // True if this tile contains a ship
    public bool shot = false;    // True if this tile has been shot at
    private string tileTag;      // Stores whether this is "PlayerSpaces" or "EnemySpaces"
    private bool showingEnemyShips = false; // Tracks enemy ship visibility state
    public GameObject missSplashEffect; // Stores the Miss Spalsh Effect prefab
    public GameObject hitSplashEffect; // Stores the Miss Spalsh Effect prefab
    public int id;

    void Start()
    {
        // Get component references and initial values
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        mainCamera = Camera.main;
        tileTag = gameObject.tag;
        if (tileTag == "PlayerSpaces")
        {
            id = GameManager.Instance.playerTiles.Count;
            GameManager.Instance.playerTiles.Add(this);
        }
        else
        {
            id = GameManager.Instance.enemyTiles.Count;
            GameManager.Instance.enemyTiles.Add(this);
        }

        // Debug position logging for setup verification
        Debug.Log($"{gameObject.name} at position {transform.position} with tag {tileTag}");

        // Ensure collider fits the tile perfectly
        GetComponent<BoxCollider2D>().size = Vector2.one;
    }

    void Update()
    {
        // Toggle enemy ship visibility when LeftShift is held
        if (Keyboard.current.leftShiftKey.isPressed)
        {
            if (!showingEnemyShips)
            {
                ToggleEnemyShips(true);
                showingEnemyShips = true;
            }
        }
        else if (showingEnemyShips)
        {
            ToggleEnemyShips(false);
            showingEnemyShips = false;
        }

        // Skip input processing during AI turns
        if (GameManager.Instance.IsAITurn) return;

        // Handle mouse input
        if (Mouse.current?.leftButton.wasPressedThisFrame ?? false)
        {
            HandleClick(Mouse.current.position.ReadValue());
        }

        // Handle touch input (only on devices with touchscreen)
        if (Touchscreen.current?.primaryTouch.press.wasPressedThisFrame ?? false)
        {
            HandleClick(Touchscreen.current.primaryTouch.position.ReadValue());
        }
    }

    // Handle touch or mouse clicks on this tile
    private void HandleClick(Vector2 screenPos)
    {
        // Convert screen position to world coordinates
        Vector2 worldPos = mainCamera.ScreenToWorldPoint(screenPos);
        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

        // Check if this tile was clicked
        if (hit.collider != null && hit.collider.gameObject == gameObject)
        {
            GetClicked();
        }
    }

    public void GetClicked()
    {
        if (GameManager.Instance.CurrentState == GameState.PlaceShips)
        {
            Debug.Log("Placed ship");
            PlaceShip();
        }
        else if (GameManager.Instance.CurrentState == GameState.ShootShips && !GameManager.Instance.IsAITurn)
        {
            Debug.Log("Shot ship");
            ShootShip();
        }
        else if (GameManager.Instance.CurrentState == GameState.ShootShips && GameManager.Instance.IsAITurn)
        {
            Debug.Log("Got Shot");
            GetShot();
        }
    }

    // Shows/hides enemy ships (magenta color) when called
    public void ToggleEnemyShips(bool show)
    {
        // Only affects unshot enemy tiles
        if (tileTag == "EnemySpaces" && !shot)
        {
            SetColor(show && hasShip ? Color.magenta : originalColor);
        }
    }

    // Shortcut method for changing tile color
    public void SetColor(Color newColor)
    {
        spriteRenderer.color = newColor;
    }
    // Handles ship placement on this tile
    void PlaceShip()
    {
        // Prevent placing ships on enemy side
        if (tileTag == "EnemySpaces")
        {
            UITextHandler.Instance.SetText("PlaceWrongSide");
            return;
        }

        // Prevent placing multiple ships on same tile
        if (hasShip)
        {
            UITextHandler.Instance.SetText("PlaceSameSpace");
            return;
        }

        // Place the ship and update game state
        hasShip = true;
        SetColor(Color.green);
        GameManager.playerShipsPlaced++;
        UITextHandler.Instance.SetText("PlacedShip");

        // Transition to shooting phase when all ships placed
        if (GameManager.playerShipsPlaced >= GameManager.MAX_SHIPS)
        {
            UITextHandler.Instance.SetText("All ships placed!");
            GameManager.Instance.SetGameState(GameState.ShootShips);
            return;
        }
    }

    // Handles shooting at this tile
    void ShootShip()
    {
        // Validate game state
        if (GameManager.Instance.IsAITurn || GameManager.Instance.CurrentState != GameState.ShootShips) return;

        // Prevent shooting own ships
        if (tileTag != "EnemySpaces")
        {
            UITextHandler.Instance.SetText("ShootWrongSide");
            return;
        }

        // Prevent shooting same tile twice
        if (shot)
        {
            UITextHandler.Instance.SetText("ShootSameSpace");
            return;
        }

        shot = true;

        if (hasShip) // Successful hit
        {
            SetColor(Color.red);
            GameManager.Instance.enemyShipsRemaining--;
            UITextHandler.Instance.SetText("Hit"); 

            if (hitSplashEffect != null)
            {
                Instantiate(hitSplashEffect, transform.position, Quaternion.identity); // Spawn hit effect on miss
            }
            
            // Check for victory
            if (GameManager.Instance.enemyShipsRemaining <= 0)
            {
                GameManager.Instance.SetGameState(GameState.Victory);
            }
        }
        else // Miss
        {
            SetColor(Color.gray);
            UITextHandler.Instance.SetText("Miss");

                if (missSplashEffect != null)
                {
                    Instantiate(missSplashEffect, transform.position, Quaternion.identity); // Spawn splash effect on miss
                }

            StartAITurn(1.5f); // Give AI turn after delay
        }
    }

    void GetShot()
    {
        if (id >= 0 && id < GameManager.Instance.playerTiles.Count) //Sanity check
        {
            if (!shot)  // Second sanity check
            {
                shot = true;
                Debug.Log("Set tile to shot");

                if (hasShip) // Successful hit
                {
                    UITextHandler.Instance.SetText("EnemyHit");
                    SetColor(new Color(1, 0.9f, 0.2f));
                    GameManager.Instance.playerShipsRemaining--;

                    if (hitSplashEffect != null)
                    {
                        Instantiate(hitSplashEffect, transform.position, Quaternion.identity); // Spawn hit effect on miss
                    }

                    Debug.Log($"AI aimed at index {id} and HIT");

                    if (GameManager.Instance.playerShipsRemaining <= 0) // If the game is over, set defeat
                    {
                        GameManager.Instance.SetGameState(GameState.Defeat);
                    }
                    else    // If game is not over, they take another turn
                    {
                        Invoke(nameof(RunAITurn), 2f); // Chain consecutive hits
                    }
                }
                else    // Miss
                {
                    UITextHandler.Instance.SetText("EnemyMiss");
                    GameManager.Instance.IsAITurn = false; // Turn ends when AI misses
                    SetColor(new Color(0.2f, 0.9f, 1));

                    if (missSplashEffect != null)
                    {
                        Instantiate(missSplashEffect, transform.position, Quaternion.identity); // Spawn splash effect on miss
                    }

                    Debug.Log($"AI aimed at index {id} and MISSED");
                }
            }
            else
            {
                Debug.Log($"AI attempted to shoot a tile that was previously shot");
            }
        }
        else
        {
            Debug.Log($"AI attempted an invalid shot");
        }
    }

    // Starts AI turn after specified delay
    void StartAITurn(float delay = 1.5f)
    {
        GameManager.Instance.IsAITurn = true;
        Invoke(nameof(RunAITurn), delay);
    }

    // Executes AI turn
    void RunAITurn()
    {
        GameManager.Instance.ai.TakeTurn();
        
        // If AI hit a ship, this will be handled in SimpleBattleshipAI.cs
    }
}