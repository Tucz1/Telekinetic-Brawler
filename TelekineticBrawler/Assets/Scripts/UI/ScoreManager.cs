using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class ScoreManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI uiText;
    [SerializeField] private TextMeshProUGUI uiPtsText;
    [SerializeField] private TextMeshProUGUI rankText;
    [SerializeField] private TextMeshProUGUI rankTextmp;

    [SerializeField] Slider rankSlider;
    [SerializeField] bool inTutorial;
    int score = 0;
    int rankScore = 0;
    int rankLevel = 0;
    float scoreMultiplier = 1;
    float rankTimer = 0;
    float rankDecayTime = 10;
    int uiScore = 0;
    string rankS = "S";
    string rankSmp = "3x";
    string rankA = "A";
    string rankAmp = "2x";

    string rankB = "B";
    string rankBmp = "1.5x";

    string rankC = "C";
    string rankCmp = "1.2x";

    string rankD = "D";
    string rankDmp = "1x";



    float delaySpeed = 0.01f;

    Coroutine currentCoroutineScore;

    private void Awake() {
        //scene 0 = main menu, scene 1 = tutorial, if in tutorial disable score UI
        if (SceneManager.GetActiveScene().buildIndex == 1) inTutorial = true;
        if (inTutorial) {
            uiText.alpha = 0f;
            uiPtsText.alpha = 0f;
            rankText.alpha = 0f;
            rankTextmp.alpha = 0f;
            var slider = GameObject.Find("Slider").GetComponent<Slider>();
            slider.gameObject.SetActive(false);
        }
    }
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
            rankTextmp.text = rankDmp;
            scoreMultiplier = 1;
        }
        if (rankLevel == 1) {
            rankText.text = rankC;
            rankTextmp.text = rankCmp;

            scoreMultiplier = 1.2f;

        }
        if (rankLevel == 2) {
            rankText.text = rankB;
            rankTextmp.text = rankBmp;
            scoreMultiplier = 1.5f;

        }
        if (rankLevel == 3) {
            rankText.text = rankA;
            rankTextmp.text = rankAmp;
            scoreMultiplier = 2;

        }
        if (rankLevel >= 4)  {
            rankText.text = rankS;
            rankTextmp.text = rankSmp;
            rankLevel = 4;
            scoreMultiplier = 3;

        }
    }
    public void decreaseRank() {
        rankLevel--;
        checkRank();
    }

    public void AddPoints(int points) {
        if (currentCoroutineScore != null) StopCoroutine(currentCoroutineScore);

        uiScore = score;
        score += (int)Mathf.Round(points * scoreMultiplier);
        rankScore += points;
        if (rankScore >= 250) { 
            rankLevel++;
            rankScore = 0;
            rankTimer = rankDecayTime;
        }
        currentCoroutineScore = StartCoroutine(CountUp());

        if (rankTimer < rankDecayTime - 1) rankTimer += 1;
        checkRank();
    }

    void Update()
    {
        if(rankLevel > 0) {
            float decayMultiplier = 1f + (rankLevel * 0.5f); //rank 1 = 1.5x speed, 2 = 2x speed and so on, min 1
            rankTimer -= Time.deltaTime * decayMultiplier;
            if(rankTimer <= 0) {
                rankTimer = rankDecayTime;
                rankLevel--;
                checkRank();
            }
        }
        rankSlider.value = Mathf.InverseLerp(0, rankDecayTime, rankTimer);
    }
}
