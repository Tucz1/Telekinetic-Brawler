using UnityEngine;

public class LimbScript : MonoBehaviour {
    [SerializeField] private BasicEnemy BasicEnemy;
    public LimbType limb;

    //Manually set child limb
    public LimbScript childLimb;
    public GameObject limbToDismember;
    public GameObject limbToSpawn;
    public float maxHealth = 10f;
    public Animator parentAnimator;
    [SerializeField] private float currentHealth;
    [SerializeField] private float damageCooldown = 2f;
    [SerializeField] private float damageCooldownTimer;

    [SerializeField] private bool dismembered = false;

    public float damageMultiplierToMain = 0.5f;
    private void Awake() {
        BasicEnemy = GetComponentInParent<BasicEnemy>();
        currentHealth = maxHealth;
        parentAnimator = GetComponentInParent<Animator>();
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

            if (limb == LimbType.UpperRightArm || limb == LimbType.LowerRightArm) parentAnimator.Play("StaggerLeft");
            if (limb == LimbType.UpperLeftArm || limb == LimbType.LowerLeftArm) parentAnimator.Play("StaggerRight");

            if (currentHealth <= 0) {
                if (childLimb != null)
                    disableChildOnly();
                    disableLimb();

                disableLimb(); // only this one spawns
            }
        }
    }
    private void Update() {
        damageCooldownTimer += Time.deltaTime;
    }
    private void disableChildOnly() {
        if (limbToDismember != null) {
            if (childLimb != null)
                childLimb.limbToDismember.SetActive(false);

        }
    }

    private void disableLimb() {
        if (limb == LimbType.LowerLeftArm || limb == LimbType.UpperLeftArm) BasicEnemy.LeftArmDisabled = true;
        if (limb == LimbType.LowerRightArm || limb == LimbType.UpperRightArm) BasicEnemy.RightArmDisabled = true;

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
}
