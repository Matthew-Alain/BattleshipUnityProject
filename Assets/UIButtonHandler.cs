using UnityEngine;

public class UIButtonHandler : MonoBehaviour
{
    public void OnButtonClick()
    {
        if (GameManager.Instance.CurrentState == GameState.MainMenu)
        {
            GameManager.Instance.SetGameState(GameState.PlaceShips);
            gameObject.SetActive(false);
        }
        else if (GameManager.Instance.CurrentState == GameState.PlaceShips)
        {
            GameManager.Instance.SetGameState(GameState.ShootShips);
        }
        else if (GameManager.Instance.CurrentState == GameState.ShootShips)
        {
            GameManager.Instance.SetGameState(GameState.PlaceShips);
        }
    }
}
