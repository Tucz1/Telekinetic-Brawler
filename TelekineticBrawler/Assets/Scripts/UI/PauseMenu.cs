using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour {
    [SerializeField] GameObject PauseMenuUI;
    [SerializeField] GameObject CrosshairStamCanvas;
    [SerializeField] FirstPersonController fpc;
    [SerializeField] GameObject eventSystem;
    public bool ispaused;

    private void Start() {
        eventSystem = GameObject.Find("UIEventSystem");
        eventSystem.SetActive(false);
    }

    public void PauseGame() {
        Time.timeScale = 0f;
        eventSystem.SetActive(true);
        PauseMenuUI.SetActive(true);
        ispaused = true;

        Cursor.lockState = CursorLockMode.None;
        fpc.cameraCanMove = false;
        fpc.lockCursor = false;

        CrosshairStamCanvas.SetActive(false);
    }

    public void ResumeGame() {
        Time.timeScale = 1f;
        PauseMenuUI.SetActive(false);
        ispaused = false;

        Cursor.lockState = CursorLockMode.Locked;
        fpc.cameraCanMove = true;
        fpc.lockCursor = true;

        CrosshairStamCanvas.SetActive(true);
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