using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class ScoreManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI uiText;
    [SerializeField] private TextMeshProUGUI rankText;
    [SerializeField] Slider rankSlider;
    int score = 0;
    int rankScore = 0;
    int rankLevel = 0;
    float rankTimer = 0;
    float rankDecayTime = 4;
    int uiScore = 0;
    string rankS = "S";
    string rankA = "A";
    string rankB = "B";
    string rankC = "C";
    string rankD = "D";


    float delaySpeed = 0.01f;

    Coroutine currentCoroutineScore;
    Coroutine currentCoroutineRank;

    IEnumerator CountUp() {
        while (uiScore < score) {
            uiScore++;
            uiText.text = uiScore.ToString();

            yield return new WaitForSeconds(delaySpeed);
        }
        currentCoroutineScore = null;
    }
    private void checkRank() {
        if (rankLevel <= 0) {
            rankText.text = rankD;
        }
        if (rankLevel == 1) {
            rankText.text = rankC;
        }
        if (rankLevel == 2) {
            rankText.text = rankB;
        }
        if (rankLevel == 3) {
            rankText.text = rankA;
        }
        if (rankLevel == 4)  {
            rankText.text = rankS;
            rankLevel = 4;
        }
    }

    public void AddPoints(int points) {
        if (currentCoroutineScore != null) StopCoroutine(currentCoroutineScore);

        uiScore = score;
        score += points;
        rankScore += points;
        if (rankScore >= 250) { 
            rankLevel++;
            rankScore = 0;
        }
        currentCoroutineScore = StartCoroutine(CountUp());

        if (rankTimer < rankDecayTime - 1) rankTimer += 1;
        checkRank();
    }

    void Update()
    {
        if(rankLevel > 0) {
            rankTimer -= Time.deltaTime;
            if(rankTimer <= 0) {
                rankTimer = rankDecayTime;
                rankLevel--;
                checkRank();
            }
        }
        rankSlider.value = Mathf.InverseLerp(0, rankDecayTime, rankTimer);
    }
}
