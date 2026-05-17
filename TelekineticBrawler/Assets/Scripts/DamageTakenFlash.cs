using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DamageTakenFlash : MonoBehaviour
{

    public Image damageImage;
    public float maxAlpha = 0.1f;
    public float duration = 0.3f;


    public void Flash() {
        StartCoroutine(FlashRoutine());
    }

    IEnumerator FlashRoutine() {
        float timer = 0f;

        while (timer < duration) {
            timer += Time.deltaTime;

            float alpha = Mathf.Lerp(maxAlpha, 0f, timer / duration);

            Color color = damageImage.color;
            color.a = alpha;

            damageImage.color = color;

            yield return null;
        }

        Color finalColor = damageImage.color;
        finalColor.a = 0f;
        damageImage.color = finalColor;
    }
}
