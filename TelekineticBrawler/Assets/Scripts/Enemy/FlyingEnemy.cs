using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class FlyingEnemy : MonoBehaviour
{

    public float maxhealth = 800;
    public float currentHealth = 800;
    [SerializeField] private bool isDead = false;
    [SerializeField] public bool firstWave = false;
    [SerializeField] private WaveManager WaveManager;
    [SerializeField] private TutorialWaveManager TutorialWaveManager;
    [SerializeField] private FXManager FXManager;
    [SerializeField] private Animator animator;
    public FirstPersonController Player;



    public float floatSpeed = 2f;
    public float floatHeight = 1f;

    public GameObject projectilePrefab;
    public Transform firePoint;
    public float shootInterval = 2f;
    public float projectileSpeed = 6f;

    private Vector3 startPos;
    private Transform player;
    private float shootTimer;

    void Awake() {
        startPos = transform.position;
        currentHealth = maxhealth;
        Player = FindAnyObjectByType<FirstPersonController>();
        WaveManager = FindAnyObjectByType<WaveManager>();
        TutorialWaveManager = FindAnyObjectByType<TutorialWaveManager>();
        player = FindAnyObjectByType<FirstPersonController>().transform;
        FXManager = FindAnyObjectByType<FXManager>();
        animator = GetComponentInChildren<Animator>();

    }

    void Update() {
        if (!firstWave) {
            ShootAtPlayer();
        }
            FloatMovement();
            transform.rotation = Quaternion.LookRotation((transform.position - player.transform.position)*-1);
    }

    void FloatMovement() {
        float newY = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;

        transform.position = new Vector3(transform.position.x, newY,transform.position.z);
    }

    void ShootAtPlayer() {

        shootTimer += Time.deltaTime;

        if (shootTimer >= shootInterval) {
            shootTimer = 0f;
            animator.Play("Attack");
        }
    }

    //Animator event uses this
    void shootFireball() {
        Vector3 direction = (player.position - firePoint.position).normalized;

        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null) {
            rb.linearVelocity = direction * projectileSpeed;
        }
    }
    public void takeDamage(float damage, float stagger, WeaponData weaponData) {
        animator.Play("Stagger");
        if (firstWave) WaveManager.aggressiveEnemies();
        
        shootTimer = 0;
        currentHealth -= damage;
        if (!isDead) {
            if (currentHealth <= 0) {
                if (WaveManager != null) WaveManager.EnemyDied();
                if (TutorialWaveManager != null) TutorialWaveManager.EnemyDied();

                StartCoroutine(RemoveBody());
                isDead = true;
                Player.currentHP += 10;
            }
        }
    }

    IEnumerator RemoveBody() {
        FXManager.SpawnEnemyDespawnFX(transform.position);
        Destroy(gameObject);
        yield return null;
    }
}
