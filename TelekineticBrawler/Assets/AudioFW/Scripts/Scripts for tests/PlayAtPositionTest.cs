using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAtPositionTest : MonoBehaviour
{
    [Header("Play audio to follow transform")]
    public string id1;
    public Transform transform1;
    public KeyCode playFollowTransform1;

    [Header("Play audio at position")]
    public string id2;
    public Transform transform2;
    public KeyCode playAtPosition2;

    [Header("Play audio to follow transform ")]
    public string id3;
    public Transform transform3;
    public KeyCode playFollowTransform3;

    [Header("Play audio at position")]
    public string id4;
    public Transform transform4;
    public KeyCode playAtPosition4;

    [Header("Play audio to follow transform")]
    public string id5;
    public Transform transform5;
    public KeyCode playFollowTransform5;

    [Header("Play audio at position")]
    public string id6;
    public Transform transform6;
    public KeyCode playAtPosition6;

    [Header("Play as 2D sound")]
    public string id7;
    public KeyCode playAs2D1;

    [Header("Play as 2D sound")]
    public string id8;
    public KeyCode playAs2D2;

    [Header("Play as 2D sound")]
    public string id9;
    public KeyCode playAs2D3;



    void Start()
    {
        if (!transform1)
            Debug.LogWarning("Transform 1 is missing");

        if (!transform2)
            Debug.LogWarning("Transform 2 is missing");

        if (!transform3)
            Debug.LogWarning("Transform 3 is missing");

        if (!transform4)
            Debug.LogWarning("Transform 4 is missing");

        if (!transform5)
            Debug.LogWarning("Transform 5 is missing");

        if (!transform6)
            Debug.LogWarning("Transform 6 is missing");

    }

    void Update()
    {
        if (Input.GetKeyDown(playFollowTransform1))
            AudioFW.Play(id1, transform1); //Follow transform

        if (Input.GetKeyDown(playAtPosition2))
            AudioFW.Play(id2, transform2.position); //At position

        if (Input.GetKeyDown(playFollowTransform3))
            AudioFW.Play(id3, transform3); //Follow transform

        if (Input.GetKeyDown(playAtPosition4))
            AudioFW.Play(id4, transform4.position); //At position

        if (Input.GetKeyDown(playFollowTransform5))
            AudioFW.Play(id5, transform5); //Follow transform

        if (Input.GetKeyDown(playAtPosition6))
            AudioFW.Play(id6, transform6.position); //At position

        if (Input.GetKeyDown(playAs2D1))
            AudioFW.Play(id7); //Play as 2D

        if (Input.GetKeyDown(playAs2D2))
            AudioFW.Play(id8); //Play as 2D

        if (Input.GetKeyDown(playAs2D3))
            AudioFW.Play(id9); //Play as 2D
    }
}
