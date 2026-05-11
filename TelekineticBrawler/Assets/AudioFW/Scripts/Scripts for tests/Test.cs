using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public string C, V, X;

    Dictionary<KeyCode, string> bindings = new Dictionary<KeyCode, string>();

    void Start()
    {
        bindings.Add(KeyCode.C, C);
        bindings.Add(KeyCode.V, V);
        bindings.Add(KeyCode.X, X);

    }

    // Update is called once per frame
    void Update()
    {
        foreach (var kc in bindings.Keys) {
            if (Input.GetKeyDown(kc))
                AudioFW.Play(bindings[kc]);
        }
    }
}
