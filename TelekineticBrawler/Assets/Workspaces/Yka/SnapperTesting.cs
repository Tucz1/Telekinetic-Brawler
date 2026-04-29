using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class SnapperTesting : MonoBehaviour {
    [SerializeField] Transform throwPoseAnchor;
    [SerializeField] Transform holdPoseAnchor;
    [SerializeField] Transform objectToSnap;
    [SerializeField] Transform holder;
    [SerializeField] string upKey = "q";
    [SerializeField] string downKey = "a";
    
    void Update() {
        var upkeyControl = Keyboard.current.FindKeyOnCurrentKeyboardLayout(upKey);
        // var upkeyControl = Keyboard.current.qKey;
        
        if (upkeyControl.wasPressedThisFrame) {
            var T = holder.rotation;
            var invSl = Quaternion.Inverse(throwPoseAnchor.localRotation);
            objectToSnap.rotation = T * invSl;
            var diff = throwPoseAnchor.position - objectToSnap.position;
            objectToSnap.position = holder.position - diff;
        }
        var downkeyControl = Keyboard.current.FindKeyOnCurrentKeyboardLayout(downKey);
        
        if (downkeyControl.wasPressedThisFrame) {
            var T = holder.rotation;
            var invSl = Quaternion.Inverse(holdPoseAnchor.localRotation);
            objectToSnap.rotation = T * invSl;
            var diff = holdPoseAnchor.position - objectToSnap.position;
            objectToSnap.position = holder.position - diff;
        }
    }
}
