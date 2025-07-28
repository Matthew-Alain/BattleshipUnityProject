using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
   public void PlayGame()
    {
        if (Application.isMobilePlatform)
        {
            if (Screen.orientation == ScreenOrientation.LandscapeLeft || Screen.orientation == ScreenOrientation.LandscapeRight)
            {
                SceneManager.LoadSceneAsync(1);
                LandscapeWarning.Instance.SetText("");
            }
            else
            {
                LandscapeWarning.Instance.SetText("Please enter landscape mode to play!");
            }
        }
        else
        {
            SceneManager.LoadSceneAsync(1);
        }
        
    }

   public void QuitGame()
    {
        Application.Quit();
    }
}
