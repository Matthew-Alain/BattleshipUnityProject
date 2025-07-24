using UnityEngine;
using UnityEngine.InputSystem;

// Handles all tile interactions - ship placement, shooting, and visual feedback
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))] // Using BoxCollider2D for more precise clicking
public class TileScript : MonoBehaviour
{
    private Camera mainCamera;
    private SpriteRenderer spriteRenderer;
    [HideInInspector] public Color originalColor; // Stores the tile's default color

    [SerializeField]
    public bool hasShip = false; // True if this tile contains a ship
    public bool shot = false;    // True if this tile has been shot at
    private static bool isAITurn = false; // Shared flag for all tiles during AI turns

    private string tileTag;      // Stores whether this is "PlayerSpaces" or "EnemySpaces"
    private bool showingEnemyShips = false; // Tracks enemy ship visibility state

    void Start()
    {
        // Get component references and initial values
        spriteRenderer = GetComponent<SpriteRenderer>();
        mainCamera = Camera.main;
        tileTag = gameObject.tag;

        // Debug position logging for setup verification
        Debug.Log($"{gameObject.name} at position {transform.position} with tag {tileTag}");

        // Ensure collider fits the tile perfectly
        GetComponent<BoxCollider2D>().size = Vector2.one;
    }

    void Update()
    {
        // Skip input processing during AI turns
        if (isAITurn) return;
        if (GameManager.Instance.IsAITurn) return;

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

        // Handle mouse input
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            HandleClick(Mouse.current.position.ReadValue());
        }

        // Handle touch input (only on devices with touchscreen)
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
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
            if (GameManager.Instance.CurrentState == GameState.PlaceShips)
            {
                PlaceShip();
            }
            else if (GameManager.Instance.CurrentState == GameState.ShootShips && !isAITurn)
            {
                ShootShip();
            }
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
    public void SetColor(Color newColor) => spriteRenderer.color = newColor;

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

        // Check ship limit
        if (GameManager.Instance.totalPlayerShipsPlaced >= GameManager.MAX_SHIPS)
        {
            UITextHandler.Instance.SetText("All ships placed!");
            return;
        }

        // Place the ship and update game state
        hasShip = true;
        SetColor(Color.green);
        GameManager.Instance.totalPlayerShipsPlaced++;
        UITextHandler.Instance.SetText("PlacedShip");

        // Transition to shooting phase when all ships placed
        if (GameManager.Instance.totalPlayerShipsPlaced >= GameManager.MAX_SHIPS)
        {
            GameManager.Instance.SetGameState(GameState.ShootShips);
        }
    }

    // Handles shooting at this tile
    void ShootShip()
    {
        // Validate game state
        if (isAITurn || GameManager.Instance.CurrentState != GameState.ShootShips) return;

        // Prevent shooting own ships
        if (tileTag != "EnemySpaces")
        {
            UITextHandler.Instance.SetText("Aim for enemy waters!");
            return;
        }

        // Prevent shooting same tile twice
        if (shot)
        {
            UITextHandler.Instance.SetText("Already shot here!");
            return;
        }

        shot = true;

        if (hasShip) // Successful hit
        {
            SetColor(Color.red);
            GameManager.Instance.enemyShipsRemaining--;
            UITextHandler.Instance.SetText("Direct hit!");

            // Check for victory
            if (GameManager.Instance.enemyShipsRemaining <= 0)
            {
                GameManager.Instance.SetGameState(GameState.Victory);
            }
        }
        else // Miss
        {
            SetColor(Color.gray);
            UITextHandler.Instance.SetText("Miss! Enemy's turn...");
            StartAITurn(1.5f); // Give AI turn after delay
        }
    }

    // Starts AI turn after specified delay
    void StartAITurn(float delay = 1.5f)
    {
        isAITurn = true;
        Invoke(nameof(RunAITurn), delay);
    }

    // Executes AI turn
    void RunAITurn()
    {
        GameManager.Instance.ai.TakeTurn();
        isAITurn = false;
    }
}