using UnityEngine;

public class FXManager : MonoBehaviour
{
    [SerializeField] GameObject HitFX;
    void Start()
    {
        
    }


public void spawnFX(Vector3 pos) {
        Instantiate(HitFX, pos, Quaternion.identity);
    }
}
