using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public class Wave {
    public string waveText;
    public string waveEndText;
    public int meleeCount;
    public int rangedCount;
}

public class WaveManager : MonoBehaviour {
    [SerializeField] private GameObject exit;

    [SerializeField] private GameObject spawnEffect;
    [SerializeField] private EnemySpawner[] spawners;
    [SerializeField] private FlyingEnemySpawner[] flyingEnemySpawners;

    private int prevRandom = -1;
    [SerializeField] private GameObject meleeEnemy;
    [SerializeField] private GameObject rangedEnemy;
    [SerializeField] private Wave[] waves;

    public float spawnDelay = 1f;
    [SerializeField] private int currentWave = 0;
    [SerializeField] private int aliveEnemies = 0;
    [SerializeField] private BasicEnemy[] startingEnemies;
    [SerializeField] private FlyingEnemy[] flyingEnemies;

    bool firstWave = true;
    bool roundStarted = false;
    //UI
    public string EndingText;

    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private TextMeshProUGUI aliveEnemiesText;

    [SerializeField] float fadeTime;
    private void Awake() {
        spawners = FindObjectsByType<EnemySpawner>(FindObjectsSortMode.None);
        flyingEnemySpawners = FindObjectsByType<FlyingEnemySpawner>(FindObjectsSortMode.None);
        startingEnemies = FindObjectsByType<BasicEnemy>(FindObjectsSortMode.None);
        flyingEnemies = FindObjectsByType<FlyingEnemy>(FindObjectsSortMode.None);

        foreach (BasicEnemy enemy in startingEnemies) {
            aliveEnemies++;
        }
        foreach(FlyingEnemy enemy in flyingEnemies) {
            aliveEnemies++;
        }
    }

    public void aggressiveEnemies() {
        foreach (BasicEnemy enemy in startingEnemies) {
            enemy.firstWave = false;
            enemy.animator.SetBool("isAggressive", true);
        }
        foreach (FlyingEnemy enemy in flyingEnemies) {
            enemy.firstWave = false;
        }
    }

    private void Update() {
        aliveEnemiesText.text = aliveEnemies.ToString();
        if (aliveEnemies <= 0 && roundStarted == false)
                StartCoroutine(EndTextAndStartNewWave(currentWave));
    }
    private IEnumerator EndTextAndStartNewWave(int currentWave) {
        roundStarted = true;

        if (!firstWave) { 
        waveText.text = waves[currentWave-1].waveEndText;
        StartCoroutine(fade(waveText, fadeTime)); 
        }

        firstWave = false;
        yield return new WaitForSeconds(fadeTime*2);

        if (currentWave >= waves.Length) {
            StartCoroutine(endingSequence());
            yield return null;
        }
        else {
            StartWave();
            roundStarted = false;
            yield return null;
        }
    }
    private void StartWave() {

        aliveEnemies = 0;
        waveText.text = waves[currentWave].waveText;
        StartCoroutine(fade(waveText, fadeTime));
        StartCoroutine(SpawnWaveCoroutine(waves[currentWave]));
        currentWave++;
    }

    private IEnumerator SpawnWaveCoroutine(Wave wave) {
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
        if (enemyPrefab.name == "BasicEnemy") {
            int random = Random.Range(0, spawners.Length);

            EnemySpawner selectedSpawner = spawners[random];
            if (random != prevRandom) {
                Instantiate(spawnEffect, selectedSpawner.transform.position + (Vector3.up * 3), Quaternion.identity);
                Instantiate(enemyPrefab, selectedSpawner.transform.position, Quaternion.identity);
                aliveEnemies++;
                prevRandom = random;
            }
            else {
                SpawnEnemy(enemyPrefab);
            }
        }
        if (enemyPrefab.name == "FlyingEnemy") {
            int random = Random.Range(0, flyingEnemySpawners.Length);

            FlyingEnemySpawner selectedFlyingEnemySpawner = flyingEnemySpawners[random];
            if (random != prevRandom) {
                Instantiate(spawnEffect, selectedFlyingEnemySpawner.transform.position + (Vector3.up * 3), Quaternion.identity);
                Instantiate(enemyPrefab, selectedFlyingEnemySpawner.transform.position, Quaternion.identity);
                aliveEnemies++;
                prevRandom = random;
            }
            else {
                SpawnEnemy(enemyPrefab);
            }
        }
    }

    public void EnemyDied() {
        aliveEnemies--;
    }

    IEnumerator endingSequence() {
        exit.SetActive(true);
        waveText.text = EndingText;
        StartCoroutine(fadeInOnly(waveText, fadeTime));
        while (true) {
            SpawnEnemy(meleeEnemy);
            yield return new WaitForSeconds(spawnDelay*2);
        }
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
    IEnumerator fadeInOnly(TextMeshProUGUI text, float duration) {

        float elapsedTime = 0;
        float startValue = 0;

        // Fade In
        while (elapsedTime < duration) {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startValue, 1, elapsedTime * duration);
            text.alpha = newAlpha;
            yield return null;
        }
    }
}