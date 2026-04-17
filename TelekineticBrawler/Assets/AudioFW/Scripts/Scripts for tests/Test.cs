using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public string C, V, X;
    public string Q, W;

    Dictionary<KeyCode, string> bindings = new Dictionary<KeyCode, string>();

    void Start()
    {
        bindings.Add(KeyCode.C, C);
        bindings.Add(KeyCode.V, V);
        bindings.Add(KeyCode.X, X);

    }
    void Update()
    {
        foreach (var kc in bindings.Keys) {
            if (Input.GetKeyDown(kc))
                AudioFW.Play(bindings[kc]);
        }
        if (Input.GetKeyDown(KeyCode.Q))
            AudioFW.PlayLoop(Q);
        if (Input.GetKeyDown(KeyCode.W))
            AudioFW.PlayLoop(W); 
        if (Input.GetKeyDown(KeyCode.A))
            AudioFW.StopLoop(Q);
        if (Input.GetKeyDown(KeyCode.S))
            AudioFW.StopLoop(W);
    }
}
