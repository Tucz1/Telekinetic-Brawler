using Unity.VisualScripting;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private float damage = 10;
    [SerializeField] private ScoreManager ScoreManager;
    [SerializeField] private CapsuleCollider cc;
    [SerializeField] private TutorialEnemy tutorialEnemy;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        ScoreManager = FindAnyObjectByType<ScoreManager>();
        cc = GetComponent<CapsuleCollider>();
    }

    private void Start() {
        tutorialEnemy = GetComponentInParent<TutorialEnemy>();
    }


    // Update is called once per frame
    private void OnTriggerEnter(Collider collision) {
        if (collision.gameObject.name == "FirstPersonController") {
            if (tutorialEnemy != null) {
                disableAttacks();
                return;
            }
            var rb = collision.GetComponent<Rigidbody>();
            var backdirection = rb.transform.forward * -1;
            rb.AddForce((rb.transform.up + backdirection) * 5, ForceMode.Impulse);

            ScoreManager.decreaseRank();

            //IMPLEMENT TAKING DAMAGE
            Debug.Log($"You took " +damage+ " damage");
        }
    }
    public void disableAttacks() {
        if (cc != null) {
            cc.enabled = false;
        }
    }
}
