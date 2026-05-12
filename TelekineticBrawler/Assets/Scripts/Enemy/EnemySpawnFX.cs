using UnityEngine;

public class EnemySpawnFX : MonoBehaviour {
    private Animator animator;
    float timer;
    [SerializeField] float despawnTime = 3;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake() {
        animator = GetComponent<Animator>();
        animator.Play("SpawnPortal");
    }

    // Update is called once per frame
    void Update() {
        timer += Time.deltaTime;
        if (timer >= despawnTime) {
            Destroy(gameObject);
        }
    }
}
