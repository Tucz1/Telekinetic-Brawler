using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class OffsetFollowTransform : MonoBehaviour {
    public Vector3 offset;
    public Transform followTarget;
    public bool copyRotation;

    void LateUpdate() {
        if (followTarget) {
            transform.position = followTarget.position + offset;
            if (copyRotation)
                transform.rotation = followTarget.rotation;
        }
    }
}
