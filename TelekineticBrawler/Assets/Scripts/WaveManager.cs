using System.Collections;
using UnityEngine;

[System.Serializable]
public class Wave {
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

    private void Awake() {
        spawners = FindObjectsByType<EnemySpawner>(FindObjectsSortMode.None);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            if (aliveEnemies == 0)
            StartWave();
        }
    }

    private void StartWave() {
        if (currentWave >= waves.Length) {
            //ENDING HERE?
            return;
        }
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
}