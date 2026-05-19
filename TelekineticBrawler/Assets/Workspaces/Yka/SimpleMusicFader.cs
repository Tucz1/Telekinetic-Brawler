using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class SimpleMusicFader : MonoBehaviour {
    public AudioSource currentLoop;
    public AudioSource nextLoop;
    public float fadeDuration = 4;
    void Start() {
        currentLoop.Play();
    }

    void Update() {
        if (Keyboard.current.spaceKey.wasPressedThisFrame) {
            FadeFromTo(currentLoop, nextLoop, fadeDuration);
        }
    }

    public void FadeFromTo(AudioSource from, AudioSource to, float duration) {
        StartCoroutine(FadeMusicFromTo(from, to, duration));
    }
    IEnumerator FadeMusicFromTo(AudioSource from, AudioSource to, float fadeTime) {
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
