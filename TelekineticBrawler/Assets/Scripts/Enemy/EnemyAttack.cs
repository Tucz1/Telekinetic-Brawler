using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private FirstPersonController Target;
    [SerializeField] private float damage = 10;
    [SerializeField] private ScoreManager ScoreManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Target = FindAnyObjectByType<FirstPersonController>();
        ScoreManager = FindAnyObjectByType<ScoreManager>();
    }

    // Update is called once per frame
    private void OnTriggerEnter(Collider collision) {
        if (collision.gameObject.name == "FirstPersonController") {
            var rb = collision.GetComponent<Rigidbody>();
            var backdirection = rb.transform.forward * -1;
            rb.AddForce((rb.transform.up + backdirection) * 5, ForceMode.Impulse);

            ScoreManager.decreaseRank();

            //IMPLEMENT TAKING DAMAGE
            Debug.Log($"You took " +damage+ " damage");
        }
    }
}
