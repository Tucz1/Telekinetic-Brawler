using UnityEngine;

public class FXManager : MonoBehaviour
{
    [SerializeField] GameObject HitFX;
    [SerializeField] GameObject EnemyDespawnFX;

    void Start()
    {
        
    }


public void spawnFX(Vector3 pos) {
        Instantiate(HitFX, pos, Quaternion.identity);
    }

public void SpawnEnemyDespawnFX(Vector3 pos) {
    Instantiate(EnemyDespawnFX, pos, Quaternion.identity);
}
}
