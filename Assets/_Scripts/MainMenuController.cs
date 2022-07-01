using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] string tutorialScene;
    public void LoadTutorial()
    {
        Debug.Log("Loading tutorial");
        SceneManager.LoadScene(tutorialScene);
    }

    public void Quit()
    {
        Debug.Log("Quitting");
        Application.Quit();
    }
}
