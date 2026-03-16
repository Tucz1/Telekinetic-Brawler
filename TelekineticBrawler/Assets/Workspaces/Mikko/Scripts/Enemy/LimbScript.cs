using UnityEngine;

public class LimbScript : MonoBehaviour
{
    [SerializeField] private BasicEnemy BasicEnemy;
    public LimbType limb;

    //Manually set child limb
    public LimbScript childLimb;
    public float maxHealth = 10f;
    [SerializeField] private float currentHealth;

    public float damageMultiplierToMain = 1f;
    private void Awake() {
        BasicEnemy = GetComponentInParent<BasicEnemy>();
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage) {
        currentHealth -= damage;

        //Send damage to main but avoid huge overflow damage
        float damageToMain = damage * damageMultiplierToMain;
        if (damageToMain > maxHealth) {
            damageToMain = maxHealth * damageMultiplierToMain;
            BasicEnemy.takeDamage(damageToMain);
        }
        BasicEnemy.takeDamage(damageToMain);
        if (currentHealth <= 0) {
            if (childLimb != null) {
                childLimb.disableLimb();
            }
            disableLimb();
        }
    }

    private void disableLimb() {
        //Insert dismemberment logic
    }
    
}
