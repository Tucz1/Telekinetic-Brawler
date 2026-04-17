using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSourceDemos : MonoBehaviour
{
    AudioSource myAudio;
    public AudioClip whatToPlay;
    void Awake()
    {
        myAudio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0)) {
            // myAudio.Play();
            myAudio.PlayOneShot(whatToPlay);
            //myAudio.PlayOneShot(myAudio.clip);
        }
    }
}
