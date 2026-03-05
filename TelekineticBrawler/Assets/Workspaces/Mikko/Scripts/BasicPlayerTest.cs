using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movement : MonoBehaviour {
    Rigidbody player;
    public float m_speed, rotatespeed;

    private void Start() {
        player = GetComponent<Rigidbody>();
    }
    void FixedUpdate() {
        if (Input.GetKey(KeyCode.W)) {
            player.linearVelocity = transform.forward * m_speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S)) {
            player.linearVelocity = -transform.forward * m_speed * Time.deltaTime;
        }
    }
    void Update() {
        if (Input.GetKey(KeyCode.A)) {
            transform.Rotate(0, -rotatespeed * Time.deltaTime, 0);
        }
        if (Input.GetKey(KeyCode.D)) {
            transform.Rotate(0, rotatespeed * Time.deltaTime, 0);
        }
    }
}