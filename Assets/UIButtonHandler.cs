using UnityEngine;

public class UIButtonHandler : MonoBehaviour
{
    public void OnButtonClick()
    {
        if (GameManager.Instance.CurrentState == GameState.MainMenu)
        {
            // Start game button
            GameManager.Instance.SetGameState(GameState.PlaceShips);
            gameObject.SetActive(false);
        }
        else if (GameManager.Instance.CurrentState == GameState.Victory || 
                GameManager.Instance.CurrentState == GameState.Defeat)
        {
            // Restart game button
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
        }
        /* 
         * We no longer need manual state toggling since:
         * - Ship placement completion is handled in TileScript
         * - Game flow is automatic after placement
         */
    }
}