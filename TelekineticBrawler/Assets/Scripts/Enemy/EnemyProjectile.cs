using UnityEngine;

public class EnemyProjectile : MonoBehaviour {
    public float lifeTime = 5f;
    public int damage = 10;

    void Start() {
        Destroy(gameObject, lifeTime);
    }

    void OnCollisionEnter(Collision other) {
        if (other.collider.tag == ("Player")) {
            Debug.Log("Player lost 10hp from projectile!");

            Destroy(gameObject);
        }
        else { Destroy(gameObject); }
    }
}