using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneButtonHandler : MonoBehaviour
{
    public void GoToTitleScreen()
    {
        SceneManager.LoadScene("TitleScreen");
    }

    public void GoToLevel2()
    {
        SceneManager.LoadScene("Level2");
    }

    public void GoToLevel3()
    {
        SceneManager.LoadScene("Level3");
    }
}
