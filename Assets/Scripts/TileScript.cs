using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class TileScript : MonoBehaviour
{
    private Camera mainCamera;
    private SpriteRenderer spriteRenderer;
    public bool hasShip = false; //Set this to private when enemy placing is automated
    private bool shot = false;
    private new string tag;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        mainCamera = Camera.main;
        tag = gameObject.tag;
        //if (GameManager.Instance.CurrentState == GameState.PlaceShipsNew)
        //{
        //    hasShip = true;
        //    tag = "PlayerSpaces";
        //}
    }

    void Update()
    {
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

    private void HandleClick(Vector2 screenPos)
    {
        Vector2 worldPos = mainCamera.ScreenToWorldPoint(screenPos);
        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

        if (hit.collider != null && hit.collider.gameObject == gameObject)
        {
            switch (GameManager.Instance.CurrentState)
            {
                case GameState.PlaceShips:
                    placeShip();
                    break;
                case GameState.ShootShips:
                    shootShip();
                    break;
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
            GameManager.shipsPlaced++;
            UITextHandler.Instance.SetText("PlacedShip");
            hasShip = true;
            setColor(Color.green);
            if (GameManager.shipsPlaced >= GameManager.maxShips)
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
                GameManager.enemyShips--;
                if (GameManager.enemyShips <= 0)
                {
                    GameManager.Instance.SetGameState(GameState.Victory);
                    Debug.Log("You win!");
                }                
            }
            //This would be where I call the enemy turn state
        }
    }
}