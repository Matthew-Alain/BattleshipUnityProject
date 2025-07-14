using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class TileScript : MonoBehaviour
{
    private Camera mainCamera;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    public bool hasShip = false; //Set this to private when enemy placing is automated
    private bool shot = false;
    public static int maxShips = 5;
    public static int shipsPlaced = 0;
    public static int enemyShips = maxShips;
    private new string tag;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        mainCamera = Camera.main;
        tag = gameObject.tag;
    }

    void Update()
    {
        // Check for left mouse click or screen tap
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Vector2 worldPos = mainCamera.ScreenToWorldPoint(mousePos);
            RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);
            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                if (GameManager.Instance.CurrentState == GameState.PlaceShips)
                {
                    placeShip();
                }
                else if (GameManager.Instance.CurrentState == GameState.ShootShips)
                {
                    shootShip();
                }
            }
        }
    }

    void setColor(Color newColor)
    {
        spriteRenderer.color = newColor;
    }

    void placeShip()
    {
        if (tag == "EnemySpaces")
        {
            UITextHandler.Instance.SetText("PlaceWrongSide");
        }
        else if (tag == "PlayerSpaces" && hasShip)
        {
            UITextHandler.Instance.SetText("PlaceSameSpace");
        }else if(tag == "PlayerSpaces" && !hasShip)
        {
            shipsPlaced++;
            UITextHandler.Instance.SetText("PlacedShip");
            hasShip = true;
            setColor(Color.green);
            if (shipsPlaced >= maxShips)
            {
                GameManager.Instance.SetGameState(GameState.ShootShips);
            }
        }
    }

    void shootShip()
    {
        if (tag == "PlayerSpaces")
        {
            UITextHandler.Instance.SetText("ShootWrongSide");
        }
        else if (tag == "EnemySpaces" && shot)
        {
            UITextHandler.Instance.SetText("ShootSameSpace");
        }
        else
        {
            shot = true;
            if (!hasShip)
            {
                setColor(Color.darkGray);
                Debug.Log("Miss");
            }
            else
            {
                setColor(Color.red);
                Debug.Log("Hit");
                enemyShips--;
                if (enemyShips <= 0)
                {
                    GameManager.Instance.SetGameState(GameState.Victory);
                    Debug.Log("You win!");
                }                
            }
            //This would be where I call the enemy turn state
        }
    }
}