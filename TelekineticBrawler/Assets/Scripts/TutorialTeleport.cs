using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class TutorialTeleport : SimpleMusicFader
{
    [SerializeField] PlayableDirector director;
    [SerializeField] AudioSource audioSource;
    [SerializeField] float fadeTime;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "FirstPersonController")
        {
            director.Play();
        }
    }
    void Start()
    {
        StartCoroutine(FadeMusicStartScene(audioSource, fadeTime));
    }

    public void FadeMusic()
    {
        StartCoroutine(FadeMusicEndScene(audioSource, fadeDuration));
    }

    public void LoadScene()
    {
        
        SceneManager.LoadScene(2);
    }
}
