using UnityEngine;

public class UIButtonHandler : MonoBehaviour
{
    public static UIButtonHandler Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }
    public void OnButtonClick()
    {
        if (GameManager.Instance.CurrentState == GameState.MainMenu)
        {
            // Start game button
            GameManager.Instance.SetGameState(GameState.PlaceShips);
            ToggleButton();

        }
        else if (GameManager.Instance.CurrentState == GameState.Victory ||
                GameManager.Instance.CurrentState == GameState.Defeat)
        {
            GameManager.Instance.SetGameState(GameState.PlaceShips);
            ToggleButton();
            // Restart game button
            //UnityEngine.SceneManagement.SceneManager.LoadScene(
            //    UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
        }
        /* 
         * We no longer need manual state toggling since:
         * - Ship placement completion is handled in TileScript
         * - Game flow is automatic after placement
         */
    }

    public void ToggleButton()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
}