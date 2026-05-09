using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public class Wave {
    public string waveText;
    public int meleeCount;
    public int rangedCount;
}

public class WaveManager : MonoBehaviour {
    [SerializeField] private EnemySpawner[] spawners;
    [SerializeField] private GameObject meleeEnemy;
    [SerializeField] private GameObject rangedEnemy;
    [SerializeField] private Wave[] waves;
    public float spawnDelay = 1f;
    [SerializeField] private int currentWave = 0;
    [SerializeField] private int aliveEnemies = 0;
    [SerializeField] private BasicEnemy[] startingEnemies;
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
        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            if (aliveEnemies <= 0)
            StartWave();
        }
    }

    private void StartWave() {
        if (currentWave >= waves.Length) {
            //ENDING HERE?
            return;
        }

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
            float newAlpha = Mathf.Lerp(startValue, 1, elapsedTime / duration);
            text.alpha = newAlpha;
            yield return null;
        }

        //Wait at max alpha
        yield return new WaitForSeconds(3);

        //Fade Out
        elapsedTime = 0;
        while (elapsedTime < duration) {
            elapsedTime += Time.deltaTime;
            float newerAlpha = Mathf.Lerp(1, 0, elapsedTime / duration);
            text.alpha = newerAlpha;
            yield return null;
        }
    }
}