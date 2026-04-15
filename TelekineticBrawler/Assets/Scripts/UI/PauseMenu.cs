using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour {
    [SerializeField] GameObject PauseMenuUI;
    [SerializeField] FirstPersonController fpc;
    bool ispaused;

    public void PauseGame() {
        Time.timeScale = 0f;
        PauseMenuUI.SetActive(true);
        ispaused = true;

        Cursor.lockState = CursorLockMode.None;
        fpc.cameraCanMove = false;
        fpc.lockCursor = false;
    }

    public void ResumeGame() {
        Time.timeScale = 1f;
        PauseMenuUI.SetActive(false);
        ispaused = false;

        Cursor.lockState = CursorLockMode.Locked;
        fpc.cameraCanMove = true;
        fpc.lockCursor = true;
    }

    public void restartLevel() {
        ResumeGame();
        var scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    public void ReturnToMainMenu() {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
    public void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (!ispaused) {
                PauseGame();
            }
            else {
                ResumeGame();
            }
            
        }
    }
}