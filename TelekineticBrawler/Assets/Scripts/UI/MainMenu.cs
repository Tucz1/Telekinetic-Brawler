using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : SimpleMusicFader
{
   [SerializeField] GameObject MainMenuUI;
   [SerializeField] GameObject SettingsUI;
   [SerializeField] GameObject CreditsUI;

   [Header("Music")]
   [SerializeField] AudioSource audioSource;
   [SerializeField] float fadeTime;
   [SerializeField] Animator animator;

    void Start()
    {
        StartCoroutine(FadeMusicStartScene(audioSource, fadeTime));
    }


    //MAIN MENU
    public void PlayGame() {
        animator.Play("FadeOut");
        StartCoroutine(FadeMusicEndScene(audioSource, fadeDuration, 1));
    }
    public void SkipTutorial() {
        animator.Play("FadeOut");
        StartCoroutine(FadeMusicEndScene(audioSource, fadeDuration, 2));
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
