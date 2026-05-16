using UnityEngine;

public class EnemyProjectile : MonoBehaviour {
    public float lifeTime = 5f;
    public int damage = 10;
    private ScoreManager ScoreManager;

    void Awake() {
        Destroy(gameObject, lifeTime);
        ScoreManager = FindAnyObjectByType<ScoreManager>();
    }

    void OnTriggerEnter(Collider collision) {
        if (collision.gameObject.name == "FirstPersonController") {

            var rb = collision.GetComponent<Rigidbody>();
            var backdirection = rb.transform.forward * -1;
            rb.AddForce((rb.transform.up) * 5, ForceMode.Impulse);
            rb.AddForce((backdirection) * 20, ForceMode.Impulse);


            ScoreManager.decreaseRank();

            var col = collision.GetComponent<FirstPersonController>();
            col.playerTakeDamage(damage);
            Destroy(gameObject);
        }
    }
}