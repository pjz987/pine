using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    AudioSource audioSource;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnCollisionExit(Collision other) {
        audioSource.Play()
    }
    void Update()
    {
        
    }
}
