using UnityEngine;

public class VSyncLimiter : MonoBehaviour
{
    private void Awake() {
        QualitySettings.vSyncCount = 1;
    }
}

