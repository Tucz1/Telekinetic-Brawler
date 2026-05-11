using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public class tutorialWave {
    public string waveText;
    public string waveEndText;
    public int meleeCount;
    public int rangedCount;
}

public class TutorialWaveManager : MonoBehaviour {
    [SerializeField] private EnemySpawner[] spawners;
    [SerializeField] private GameObject meleeEnemy;
    [SerializeField] private GameObject rangedEnemy;
    [SerializeField] private tutorialWave[] waves;
    public float spawnDelay = 1f;
    [SerializeField] private int currentWave = 0;
    [SerializeField] private int aliveEnemies = 0;
    [SerializeField] private BasicEnemy[] startingEnemies;
    bool firstWave = true;
    bool roundStarted = false;
    //UI
    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] float fadeTime;
    private void Awake() {
        spawners = FindObjectsByType<EnemySpawner>(FindObjectsSortMode.None);
        startingEnemies = FindObjectsByType<BasicEnemy>(FindObjectsSortMode.None);

        foreach (BasicEnemy enemy in startingEnemies) {
            aliveEnemies++;
        }
    }

    public void aggressiveEnemies() {
        foreach (BasicEnemy enemy in startingEnemies) {
            enemy.firstWave = false;
            enemy.animator.SetBool("isAggressive", true);
        }
    }

    private void Update() {
        if (aliveEnemies <= 0 && roundStarted == false)
                StartCoroutine(EndTextAndStartNewWave(currentWave));
    }
    private IEnumerator EndTextAndStartNewWave(int currentWave) {
        roundStarted = true;

        if (currentWave >= waves.Length) {
            //ENDING HERE?
            yield return null;
        }
        if (!firstWave) { 
        waveText.text = waves[currentWave-1].waveEndText;
        StartCoroutine(fade(waveText, fadeTime)); 
        }

        firstWave = false;
        yield return new WaitForSeconds(fadeTime*2);
        StartWave();
        roundStarted = false;
        yield return null;
    }
    private void StartWave() {

        aliveEnemies = 0;
        waveText.text = waves[currentWave].waveText;
        StartCoroutine(fade(waveText, fadeTime));
        StartCoroutine(SpawnWaveCoroutine(waves[currentWave]));
        currentWave++;
    }

    private IEnumerator SpawnWaveCoroutine(tutorialWave wave) {
        for (int i = 0; i < wave.meleeCount; i++) {
            SpawnEnemy(meleeEnemy);

            yield return new WaitForSeconds(spawnDelay);
        }

        for (int i = 0; i < wave.rangedCount; i++) {
            SpawnEnemy(rangedEnemy);

            yield return new WaitForSeconds(spawnDelay);
        }
    }

    private void SpawnEnemy(GameObject enemyPrefab) {
        int random = Random.Range(0, spawners.Length);
        EnemySpawner selectedSpawner = spawners[random];

        Instantiate(enemyPrefab, selectedSpawner.transform.position, Quaternion.identity);
        aliveEnemies++;
    }

    public void EnemyDied() {
        aliveEnemies--;
    }

    IEnumerator fade(TextMeshProUGUI text, float duration) {

        float elapsedTime = 0;
        float startValue = 0;

        // Fade In
        while (elapsedTime < duration) {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startValue, 1, elapsedTime * duration);
            text.alpha = newAlpha;
            yield return null;
        }

        //Wait at max alpha
        yield return new WaitForSeconds(duration/2);

        //Fade Out
        elapsedTime = 0;
        while (elapsedTime < duration) {
            elapsedTime += Time.deltaTime;
            float newerAlpha = Mathf.Lerp(1, 0, elapsedTime * duration);
            text.alpha = newerAlpha;
            yield return null;
        }
    }
}