using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    public bool selfDestruct;
    

    // Update is called once per frame
    void Update()
    {
        if(selfDestruct)
            Destroy(gameObject);
    }
}
