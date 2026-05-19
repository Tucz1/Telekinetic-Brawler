using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HighscoreTable : MonoBehaviour {
    private Transform entryContainer;
    private Transform entryTemplate;
    private List<Transform> highscoreEntryTransformList;
    private List<HighscoreEntry> highScoreFirstEntry;

    [SerializeField] TMP_InputField inputField;
    [SerializeField] private GameObject submitButton;
    [SerializeField] private GameObject inputFieldObject;
    [SerializeField] private GameObject MainMenuButton;
    [SerializeField] private GameObject YourScoreContainer;
    [SerializeField] private TextMeshProUGUI yourScore;


    private void Awake() {
        //ENTERING FROM MAIN MENU, VIEW ONLY
        if (ScoreContainer.state == 0) {
            submitButton.SetActive(false);
            inputFieldObject.SetActive(false);
            YourScoreContainer.SetActive(false);
        }
        //IF PLAYER WINS
        if (ScoreContainer.state == 1) {
            submitButton.SetActive(true);
            inputFieldObject.SetActive(true);
            YourScoreContainer.SetActive(true);
        }
        //IF PLAYER LOSES
        if (ScoreContainer.state == 2) {
            submitButton.SetActive(true);
            inputFieldObject.SetActive(true);
            YourScoreContainer.SetActive(true);
        }

        yourScore.text = ScoreContainer.score.ToString();


        entryContainer = transform.Find("HighscoreEntryContainer");
        entryTemplate = entryContainer.Find("HighscoreEntryTemplate");

        entryTemplate.gameObject.SetActive(false);

        string jsonString = PlayerPrefs.GetString("highscoreTable");

        Highscores highscores;

        if (string.IsNullOrEmpty(jsonString)) {
            highscores = new Highscores {
                highscoreEntryList = new List<HighscoreEntry>()
            };

            string json = JsonUtility.ToJson(highscores);
            PlayerPrefs.SetString("highscoreTable", json);
            PlayerPrefs.Save();
        }
        else {
            highscores = JsonUtility.FromJson<Highscores>(jsonString);
        }

        //Sort entry list by score
        for (int i = 0; i < highscores.highscoreEntryList.Count; i++) {
            for (int j = i + 1; j < highscores.highscoreEntryList.Count; j++) {
                if (highscores.highscoreEntryList[j].score > highscores.highscoreEntryList[i].score) {
                    //swap
                    HighscoreEntry tmp = highscores.highscoreEntryList[i];
                    highscores.highscoreEntryList[i] = highscores.highscoreEntryList[j];
                    highscores.highscoreEntryList[j] = tmp;
                }
            }
        }
        if (highscores.highscoreEntryList.Count > 10) {
            for (int h = highscores.highscoreEntryList.Count; h > 10; h--) {
                highscores.highscoreEntryList.RemoveAt(10);
            }
        }

        highscoreEntryTransformList = new List<Transform>();
        foreach (HighscoreEntry highscoreEntry in highscores.highscoreEntryList) {
            CreateHighscoreEntryTransform(highscoreEntry, entryContainer, highscoreEntryTransformList);
        }
    }

    private void CreateHighscoreEntryTransform(HighscoreEntry highscoreEntry, Transform container, List<Transform> transformList) {
        float templateHeight = 35f;
        Transform entryTransform = Instantiate(entryTemplate, container);
        RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
        entryRectTransform.anchoredPosition = new Vector2(0, -templateHeight * transformList.Count);
        entryTransform.gameObject.SetActive(true);


        int rank = transformList.Count + 1;
        string rankString;
        switch (rank) {
            default:
                rankString = rank + "TH"; break;

            case 1: rankString = "1ST"; break;
            case 2: rankString = "2ND"; break;
            case 3: rankString = "3RD"; break;

        }
        entryTransform.Find("PosText").GetComponent<TextMeshProUGUI>().text = rankString;

        int score = highscoreEntry.score;
        entryTransform.Find("ScoreText").GetComponent<TextMeshProUGUI>().text = score.ToString();

        string name = highscoreEntry.name;
        entryTransform.Find("NameText").GetComponent<TextMeshProUGUI>().text = name;

        transformList.Add(entryTransform);
    }
    public void submitButtonPressed() {
        string name = inputField.text;

        if (string.IsNullOrWhiteSpace(name)) {
            name = "AAA";
        }

        int score = ScoreContainer.score;

        AddHighscoreEntry(score, name);

        RefreshUI();

        submitButton.SetActive(false);
        inputFieldObject.SetActive(false);
    }
    public void mainMenuButtonPressed() {
        ScoreContainer.score = 0;
        ScoreContainer.state = 0;
        SceneManager.LoadScene(0);
    }

    public void AddHighscoreEntry(int score, string name) {

        HighscoreEntry highscoreEntry = new HighscoreEntry {
            score = score,
            name = name
        };

        string jsonString = PlayerPrefs.GetString("highscoreTable");
        Highscores highscores;

        if (string.IsNullOrEmpty(jsonString)) {
            highscores = new Highscores {
                highscoreEntryList = new List<HighscoreEntry>()
            };
        }
        else {
            highscores = JsonUtility.FromJson<Highscores>(jsonString);
        }

        highscores.highscoreEntryList.Add(highscoreEntry);

        // Sort descending
        highscores.highscoreEntryList.Sort((a, b) => b.score.CompareTo(a.score));

        // Keep top 10
        if (highscores.highscoreEntryList.Count > 10) {
            highscores.highscoreEntryList.RemoveRange(10, highscores.highscoreEntryList.Count - 10);
        }

        string json = JsonUtility.ToJson(highscores);

        PlayerPrefs.SetString("highscoreTable", json);
        PlayerPrefs.Save();
    }
    private void RefreshUI() {
        // destroy old entries
        foreach (Transform child in entryContainer) {
            if (child == entryTemplate) continue;
            Destroy(child.gameObject);
        }

        string jsonString = PlayerPrefs.GetString("highscoreTable");

        Highscores highscores = JsonUtility.FromJson<Highscores>(jsonString);

        if (highscores == null || highscores.highscoreEntryList == null)
            return;

        // sort
        highscores.highscoreEntryList.Sort((a, b) => b.score.CompareTo(a.score));

        highscoreEntryTransformList = new List<Transform>();

        foreach (HighscoreEntry entry in highscores.highscoreEntryList) {
            CreateHighscoreEntryTransform(entry, entryContainer, highscoreEntryTransformList);
        }
    }
    private class Highscores {
        public List<HighscoreEntry> highscoreEntryList;
    }

    [System.Serializable]
    private class HighscoreEntry {
        public int score;
        public string name;
    }
}
