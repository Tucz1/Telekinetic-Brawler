using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    public float speed;
    public Vector3 rotationAxis = Vector3.zero;
    void Update()
    {
        transform.Rotate(rotationAxis, Time.deltaTime * speed);
    }
}
