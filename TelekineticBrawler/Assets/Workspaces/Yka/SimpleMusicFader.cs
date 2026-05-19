using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SimpleMusicFader : MonoBehaviour {

    public float fadeDuration = 2;

    public void FadefromTo(AudioSource from, AudioSource to, float duration) {
        StartCoroutine(FadeMusicfromTo(from, to, duration));
    }
    public IEnumerator FadeMusicEndScene(AudioSource source, float fadeTime)
    {
        float startVol = source.volume;
        float timer = 0;
        while (timer <= fadeTime) {
            float t = timer / fadeTime;
            source.volume = Mathf.Lerp(startVol, 0, t);
            timer += Time.deltaTime;
            yield return null;
        }
        source.Stop();
    }
    public IEnumerator FadeMusicStartScene(AudioSource source, float fadeTime)
    {
        float startVol = source.volume;
        source.Play();
        float timer = 0;
        while (timer <= fadeTime) {
            float t = timer / fadeTime;
            source.volume = Mathf.Lerp(0, startVol, t);
            timer += Time.deltaTime;
            yield return null;
        }
        source.volume = startVol;
    }
    IEnumerator FadeMusicfromTo(AudioSource from, AudioSource to, float fadeTime) {
        float startVol = from.volume;
        float timer = 0;
        while (timer <= fadeTime / 2) {
            float t = timer / (fadeTime / 2);
            from.volume = Mathf.Lerp(startVol, 0, t);
            timer += Time.deltaTime;
            yield return null;
        }
        float endVol = to.volume;
        from.Stop();
        //////
        to.Play();
        timer = 0;
        while (timer <= fadeTime / 2) {
            float t = timer / (fadeTime / 2);
            to.volume = Mathf.Lerp(0, endVol, t);
            timer += Time.deltaTime;
            yield return null;
        }
    }
}
