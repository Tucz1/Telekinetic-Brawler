using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
   [SerializeField] GameObject MainMenuUI;
   [SerializeField] GameObject SettingsUI;
   [SerializeField] GameObject CreditsUI;


    //MAIN MENU
    public void PlayGame() {
        SceneManager.LoadScene(1);
    }
    public void SkipTutorial() {
        SceneManager.LoadScene(2);
    }
    public void SettingsMenu() {
        MainMenuUI.SetActive(false);
        SettingsUI.SetActive(true);
    }
    public void CreditsMenu()
    {
        MainMenuUI.SetActive(false);
        CreditsUI.SetActive(true);
    }
    public void HideCreditsMenu()
    {
        CreditsUI.SetActive(false);
        MainMenuUI.SetActive(true);
    }
public void Leaderboard() {
        SceneManager.LoadScene(3);
    }
public void ExitToMainMenu() {
        SceneManager.LoadScene(0);
    }
public void ExitGame() {
        Application.Quit();
    }

    //SETTINGS MENU

    public void SettingsBack() {
        SettingsUI.SetActive(false);
        MainMenuUI.SetActive(true);
    }

}
