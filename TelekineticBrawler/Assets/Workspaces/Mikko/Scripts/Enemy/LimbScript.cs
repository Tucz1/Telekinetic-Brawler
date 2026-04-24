using UnityEngine;

public class LimbScript : MonoBehaviour {
    [SerializeField] private BasicEnemy BasicEnemy;
    public LimbType limb;

    //Manually set child limb
    public LimbScript childLimb;
    public GameObject limbToDismember;
    public GameObject limbToSpawn;
    public float maxHealth = 10f;
    [SerializeField] private float currentHealth;
    [SerializeField] private float damageCooldown = 2f;
    [SerializeField] private float damageCooldownTimer;

    private bool dismembered = false;

    public float damageMultiplierToMain = 0.5f;
    private void Awake() {
        BasicEnemy = GetComponentInParent<BasicEnemy>();
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage, float stagger, WeaponData weaponData) {
        if (damageCooldownTimer >= damageCooldown) {
            damageCooldownTimer = 0f;
            currentHealth -= damage;

            //Send damage to main but avoid huge overflow damage
            float damageToMain = damage * damageMultiplierToMain;
            if (damageToMain > maxHealth) {
                damageToMain = maxHealth * damageMultiplierToMain;
            }
            BasicEnemy.takeDamage(damageToMain, stagger, weaponData);

            if (currentHealth <= 0) {
                if (childLimb != null)
                    childLimb.disableChildOnly();

                disableLimb(); // only this one spawns
            }
        }
    }
    private void Update() {
        damageCooldownTimer += Time.deltaTime;
    }

    private void disableLimb() {
        if (limbToDismember != null) {
            var spawnPos = limbToDismember.transform.position;
            var rotPos = limbToDismember.transform.rotation;

            if (dismembered) return;
            limbToDismember.SetActive(false);
            dismembered = true;
            if (limbToSpawn != null) {
                Instantiate(limbToSpawn, spawnPos, rotPos);
            }
        }
    }
    private void disableChildOnly() {
        if (limbToDismember != null) {
            if (childLimb != null)
                childLimb.disableChildOnly();

            limbToDismember.SetActive(false);
        }
    }
}
