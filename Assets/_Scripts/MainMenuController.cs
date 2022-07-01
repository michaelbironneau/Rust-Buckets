using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] string tutorialScene;
    [SerializeField] string mainMenuScene;

    public void LoadMainMenu()
    {
        Debug.Log("Loading main menu");
        SceneManager.LoadScene(mainMenuScene, LoadSceneMode.Single);
    }
    public void LoadTutorial()
    {
        Debug.Log("Loading tutorial");
        SceneManager.LoadScene(tutorialScene, LoadSceneMode.Single);
    }

    public void Quit()
    {
        Debug.Log("Quitting");
        Application.Quit();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            LoadMainMenu();
        }
    }
}
