using UnityEngine;
using UnityEngine.InputSystem;

public class SnapperTesting : MonoBehaviour {
    [SerializeField] Transform throwPoseAnchor;
    [SerializeField] Transform holdPoseAnchor;
    [SerializeField] Transform objectToSnap;
    [SerializeField] Transform holder;

    void Update() {
        if (Keyboard.current.qKey.wasPressedThisFrame) {
            var T = holder.rotation;
            var invSl = Quaternion.Inverse(throwPoseAnchor.localRotation);
            objectToSnap.rotation = T * invSl;
            var diff = throwPoseAnchor.position - objectToSnap.position;
            objectToSnap.position = holder.position - diff;
        }

        if (Keyboard.current.aKey.wasPressedThisFrame) {
            var T = holder.rotation;
            var invSl = Quaternion.Inverse(holdPoseAnchor.localRotation);
            objectToSnap.rotation = T * invSl;
            var diff = holdPoseAnchor.position - objectToSnap.position;
            objectToSnap.position = holder.position - diff;
        }
    }
}
