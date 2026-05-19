using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class TutorialTeleport : MonoBehaviour
{
    [SerializeField] PlayableDirector director;

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.name == "FirstPersonController")
        {
            director.Play();
        }
    }

    public void LoadScene()
    {
        SceneManager.LoadScene(2);
    }
}
