using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BasicEnemy : MonoBehaviour {
    NavMeshAgent agent;
    public Transform Target;
    [SerializeField] BoxCollider hitBox;
    [SerializeField] bool isPlayerInside = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        agent = GetComponent<NavMeshAgent>();

    }

    // Update is called once per frame
    void Update() {
        agent.destination = Target.position;

    }
    
    private void OnTriggerEnter(Collider collision) {
        if(collision.gameObject.name == "FirstPersonController") {
        Debug.Log(collision.gameObject.name);
        isPlayerInside = true;
        StartCoroutine(attackPlayer());
        }
    }
    private void OnTriggerExit(Collider collision)  {
        isPlayerInside = false;
        StopCoroutine(attackPlayer());
        StartCoroutine(backtoDefault());
    }

    IEnumerator attackPlayer() {

        GameObject.Find("EnemyModel").GetComponent<Animator>().Play("AttackWindup");

        yield return new WaitForSeconds(3);
        Debug.Log("Testing if player is inside");
        if (isPlayerInside) {
            Debug.Log("player is inside");
            GameObject.Find("EnemyModel").GetComponent<Animator>().Play("AttackPunch");
        }
        yield return null;
    }
    IEnumerator backtoDefault() {
        GameObject.Find("EnemyModel").GetComponent<Animator>().Play("Walking");
        yield return null;
    }
}