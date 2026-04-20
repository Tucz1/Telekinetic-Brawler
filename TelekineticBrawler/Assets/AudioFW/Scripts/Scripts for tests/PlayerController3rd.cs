using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[HelpURL("http://lmgtfy.com/")]
public class PlayerController3rd : MonoBehaviour {
    public float speed;
    float stepTimer = 0f;
    float stepMinInterval = 0.5f;
    
    void Update() {
        stepTimer -= Time.deltaTime;
        var input = Input.GetAxis("Horizontal") * Vector3.right +
            Input.GetAxis("Vertical") * Vector3.forward;
        if (input.magnitude > 0.1f) {
            // TODO: gated effect max repeat rate inside AudioFW..?
            if (stepTimer < 0) {
                AudioFW.Play("footsteps-normal");
                stepTimer = stepMinInterval;
            }
        }
        var velocity = input.normalized * speed;
        transform.position += velocity * Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Alpha9)) {
            AudioFW.Play("gunshot");
        }

    }
}
