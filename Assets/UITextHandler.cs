using TMPro;
using UnityEngine;

public class UITextHandler : MonoBehaviour
{
    public static UITextHandler Instance { get; private set; }
    private TextMeshProUGUI tmp;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        tmp = GetComponent<TextMeshProUGUI>();
    }
    public void SetText(string newText)
    {
        switch (newText)
        {
            case "State":
                switch (GameManager.Instance.CurrentState)
                {
                    case GameState.MainMenu:
                        text("Welcome! Click \"Start Game\" to start!");
                        break;
                    case GameState.PlaceShips:
                        text("Place your ships! Remaining ships: "+(TileScript.maxShips - TileScript.shipsPlaced).ToString());
                        break;
                    case GameState.ShootShips:
                        text("Select the enemy space you would like to shoot!");
                        break;
                    case GameState.Victory:
                        text("You sunk all of the enemy ships, you win!");
                        break;
                    case GameState.Defeat:
                        text("The enemy sunk all of your ships, you lose...");
                        break;
                    default:
                        break;
                }
                break;
            case "PlacedShip":
                text("Place your ships! Remaining ships: "+(TileScript.maxShips - TileScript.shipsPlaced).ToString());
                break;
            case "PlaceWrongSide":
                text("You can only place ships on your side. Remaining ships: "+(TileScript.maxShips - TileScript.shipsPlaced).ToString());
                break;
            case "PlaceSameSpace":
                text("That space is too close to an existing ship. Remaining ships: "+(TileScript.maxShips - TileScript.shipsPlaced).ToString());
                break;
            case "ShootWrongSide":
                text("You can only shoot ships on the enemy side. Select the enemy space you would like to shoot!");
                break;
            case "ShootSameSpace":
                text("You already shot that space, try somewhere else. Select the enemy space you would like to shoot!");
                break;
            default:
                break;
        }

    }

    private void text(string newText)
    {
        tmp.text = newText;
    }
}
