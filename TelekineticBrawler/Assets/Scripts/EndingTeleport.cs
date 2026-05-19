using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingTeleport : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.name == "FirstPersonController")
        {
            Cursor.lockState = CursorLockMode.None;
            ScoreContainer.state = 1;
            var finalscore = ScoreContainer.score * 1.2;
            ScoreContainer.score = (int)finalscore;
            SceneManager.LoadScene(3);
        }
    }
}
