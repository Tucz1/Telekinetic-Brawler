using System.Collections;
using UnityEngine;

public class TimeWarp : MonoBehaviour
{
    Interactable interactable;

    [SerializeField] float idealTimeScale = 1f;
    [SerializeField] float impactTimeScale = 0.2f;
    [SerializeField] float decayRate;
    [SerializeField] float slowDownAmp;
    public IEnumerator ImpactFrame()
    {
        while (Time.timeScale > impactTimeScale)
        {
            Time.timeScale -= slowDownAmp * Time.unscaledDeltaTime;

            Time.timeScale = Mathf.Clamp01(Time.timeScale);

            Debug.Log(Time.deltaTime);

            Time.fixedDeltaTime = 0.02f * Time.timeScale;

            yield return null;
        }

        while (Time.timeScale < idealTimeScale)
        {
            Time.timeScale += decayRate * Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Clamp01(Time.timeScale);


            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            yield return null;
        }

        Time.timeScale = idealTimeScale;

        yield break;
    }
}
