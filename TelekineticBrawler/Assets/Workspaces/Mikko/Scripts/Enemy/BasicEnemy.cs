using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BasicEnemy : MonoBehaviour {
    NavMeshAgent agent;
    Animator animator;
    public Transform Target;
    [SerializeField] bool isPlayerInside = false;
    [SerializeField] private Rigidbody[] ragdollRigidbodies;
    [SerializeField] private bool ragdolling;

    private Vector2 Velocity;
    private Vector2 SmoothDeltaPosition;

    void Awake() {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();

        animator.applyRootMotion = true;
        agent.updatePosition = false;
        agent.updateRotation = true;
        disableRagdolls();
    }

    private void enableRagdolls() {
        agent.enabled = false;
        animator.enabled = false;
        foreach (Rigidbody ragdoll in ragdollRigidbodies) {
            ragdoll.isKinematic = false;
        }
        ragdolling = true;

    }

    private void disableRagdolls() {
        foreach (Rigidbody ragdoll in ragdollRigidbodies) {
            ragdoll.isKinematic = true;
        }
        agent.enabled = true;
        animator.enabled = true;
        ragdolling = false;
    }
    private void OnAnimatorMove() {
        Vector3 rootPosition = animator.rootPosition;
        rootPosition.y = agent.nextPosition.y;
        transform.position = rootPosition;
        agent.nextPosition = rootPosition;
    }

    // Update is called once per frame
    void Update() {

        if (ragdolling == false) {
            agent.destination = Target.position;
            SyncAnimatorAndAgent();
        }

        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            enableRagdolls();
        }
        if (Input.GetKeyDown(KeyCode.DownArrow)) {
            disableRagdolls();
        }
    }
    private void SyncAnimatorAndAgent() {

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
    private void OnTriggerEnter(Collider collision) {
        if(collision.gameObject.name == "FirstPersonController") {
        isPlayerInside = true;
        StartCoroutine(attackPlayer());
        }
    }
    private void OnTriggerExit(Collider collision)  {
        isPlayerInside = false;
        //StopCoroutine(attackPlayer());
        //StartCoroutine(backtoDefault());
    }

    IEnumerator attackPlayer() {
        animator.SetBool("isAttacking", true);
        yield return new WaitForSeconds(3);
        Debug.Log("Testing if player is inside");
        if (isPlayerInside) {
            //Deal damage
            Debug.Log("player is inside");
            animator.SetBool("isAttacking", false);
        }
        animator.SetBool("isAttacking", false);
        yield return null;
    }

}