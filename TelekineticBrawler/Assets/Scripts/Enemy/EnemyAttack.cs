using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private FirstPersonController Target;
    [SerializeField] private float damage = 10;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Target = FindAnyObjectByType<FirstPersonController>();
    }

    // Update is called once per frame
    private void OnTriggerEnter(Collider collision) {
        if (collision.gameObject.name == "FirstPersonController") {
            //IMPLEMENT TAKING DAMAGE
            Debug.Log($"You took " +damage+ " damage");
        }
    }
}
