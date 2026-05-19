using UnityEngine;

public class DestroyAfterDelay : MonoBehaviour
{
    [SerializeField] float destroyDelay = 5f;
    void Start()
    {
        Destroy(this.gameObject, destroyDelay);
    }

    void OnDestroy()
    {
        // Debug.Log($"Destroying {this.gameObject}");
    }
}
