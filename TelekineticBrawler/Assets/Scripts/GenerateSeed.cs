using UnityEngine;

public class GenerateSeed : MonoBehaviour
{
    private LineRenderer rend;
    public float test;

    void Awake()
    {
        rend = GetComponent<LineRenderer>();

        test = Random.value;

        rend.material.SetFloat("_offsetSeed", test);

    }


}
