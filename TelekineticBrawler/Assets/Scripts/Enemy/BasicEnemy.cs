using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BasicEnemy : MonoBehaviour {
    NavMeshAgent agent;
    FXManager fxManager;
    public Animator animator;
    public FirstPersonController player;
    public Transform Target;
    public WaveManager WaveManager;
    [SerializeField] bool isPointingUp;
    [SerializeField] private Rigidbody[] ragdollRigidbodies;
    [SerializeField] private EnemyAttack[] enemyAttacks;
    [SerializeField] private bool ragdolling;
    [SerializeField] public int lightStaggerThreshold;
    [SerializeField] public int staggerThreshold;
    [SerializeField] public int ragdollThreshold;
    [SerializeField] private int ragdollTime;
    [SerializeField] private bool isDead = false;
    [SerializeField] private float deathDespawnTime = 10f;

    [SerializeField] private Transform hips;

    private Vector2 Velocity;
    private Vector2 SmoothDeltaPosition;



    //AGGRO LOGIC
    [SerializeField] public bool firstWave = false;

    //HP
    [SerializeField] private float currentHealth = 100;

    //DISABLED LIMBS
    public bool LeftArmDisabled = false;
    public bool RightArmDisabled = false;
    public bool LegsDisabled = false;


    void Awake() {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();
        enemyAttacks = GetComponentsInChildren<EnemyAttack>();
        player = FindAnyObjectByType<FirstPersonController>();
        Target = FindAnyObjectByType<FirstPersonController>().transform;
        WaveManager = FindAnyObjectByType<WaveManager>();
        fxManager = FindAnyObjectByType<FXManager>();

        animator.applyRootMotion = true;
        agent.updatePosition = false;
        agent.updateRotation = true;

        if (!firstWave) StartCoroutine(RagdollStagger());

        if (firstWave) {
            animator.SetBool("isAggressive", false);
            disableRagdolls();
        } 
        else animator.SetBool("isAggressive", true);
        } 
    

    public void takeDamage(float damage, float stagger, WeaponData weaponData) {
        if (firstWave) disableFirstWave();
        currentHealth -= damage;
        if (!isDead) {
            if (currentHealth <= 0) {
                StartCoroutine(RemoveBody());
                enableRagdolls();
                WaveManager.EnemyDied();
                isDead = true;
                player.currentHP += 10;
            }
        }
    }
    private void disableFirstWave() {
        WaveManager.aggressiveEnemies();
    }

    public void enableRagdolls() {
        disableAttacks();
        agent.enabled = false;
        animator.enabled = false;
        foreach (Rigidbody ragdoll in ragdollRigidbodies) {
            ragdoll.isKinematic = false;
        }
        ragdolling = true;

    }

    public void legCheck() {
        if (LegsDisabled) animator.Play("Crawling");
    }

    private void disableRagdolls() {
        if (!isDead) {
            foreach (Rigidbody ragdoll in ragdollRigidbodies) {
                ragdoll.isKinematic = true;
            }
            agent.enabled = true;
            agent.Warp(animator.rootPosition);
            animator.enabled = true;
            ragdolling = false;
            if (LegsDisabled) animator.Play("Crawling");
            else if (!LegsDisabled) {
                if (hips.forward.y < 0) animator.Play("GetUpBack");
                if (hips.forward.y > 0) animator.Play("GetUpFront");
            }
            enableAttacks();
        }

    }
    private void OnAnimatorMove() {
        Vector3 rootPosition = animator.rootPosition;
        rootPosition.y = agent.nextPosition.y;
        transform.position = rootPosition;
        agent.nextPosition = rootPosition;
    }

    void Update() {
            if (ragdolling == false) {
                agent.destination = Target.position;
                SyncAnimatorAndAgent();
            }
    }

    public void SyncAnimatorAndAgent() {

        Vector3 worldDeltaPosition = agent.nextPosition - transform.position;
        worldDeltaPosition.y = 0;

        float dx = Vector3.Dot(transform.right, worldDeltaPosition);
        float dy = Vector3.Dot(transform.forward, worldDeltaPosition);
        Vector2 deltaPosition = new Vector2(dx, dy);

        float smooth = Mathf.Min(1, Time.deltaTime / 0.1f);
        SmoothDeltaPosition = Vector2.Lerp(SmoothDeltaPosition, deltaPosition, smooth);

        Velocity = SmoothDeltaPosition / Time.deltaTime;
        if (agent.remainingDistance <= agent.stoppingDistance) {
            Velocity = Vector2.Lerp(Vector2.zero, Velocity, agent.remainingDistance / agent.stoppingDistance);
        }
        bool shouldMove = Velocity.magnitude > 0.5f
            && agent.remainingDistance > agent.stoppingDistance;

        animator.SetBool("isWalking", shouldMove);
        animator.SetFloat("Blend", Velocity.magnitude);

        float deltaMagnitude = worldDeltaPosition.magnitude;
        if (deltaMagnitude > agent.radius / 2f) {
            transform.position = Vector3.Lerp(animator.rootPosition, agent.nextPosition, smooth);
        }
    }

    private void OnTriggerStay(Collider collision) {
        if (collision.gameObject.name == "FirstPersonController" && !isDead) {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Walking") ||
                animator.GetCurrentAnimatorStateInfo(0).IsName("Crawling")||
                animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))

                AttackPlayer();
        }
    }

    private void AttackPlayer() {
        if (firstWave) disableFirstWave();
        if (LegsDisabled) {
            animator.Play("LeglessAttack");
        } else if (LeftArmDisabled && !RightArmDisabled) {
            animator.Play("WalkAttackRightArm");
        } else if (RightArmDisabled && !LeftArmDisabled) {
            animator.Play("WalkAttackLeftArm");
        } else if (RightArmDisabled && LeftArmDisabled) {
            animator.Play("WeTestThingsHere");
        } else
            animator.Play("WalkAttack");
    }
    public void disableAttacks() {
        foreach (EnemyAttack ea in enemyAttacks) {
            ea.enabled = false;
        }
    }

    public void enableAttacks() {
        foreach (EnemyAttack ea in enemyAttacks) {
            ea.enabled = true;
        }
    }

    public IEnumerator RagdollStagger() {
        agent.updateRotation = false;

        enableRagdolls();
        yield return new WaitForSeconds(ragdollTime);
        disableRagdolls();
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        agent.updateRotation = true;
        yield return null;
    }
    IEnumerator RemoveBody() {
        
        
        yield return new WaitForSeconds(2);
        var fxspawnPos = new Vector3(hips.position.x, animator.rootPosition.y, hips.position.z);
        fxManager.SpawnEnemyDespawnFX(fxspawnPos);
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }

}