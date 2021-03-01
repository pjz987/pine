using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandingAudio : MonoBehaviour
{
    AudioManager audioManager;
    string[] landings = new string[] {"Landing1", "Landing2", "Landing3"};
    void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
    }

    private void OnCollisionEnter(Collision other) {
        audioManager.Play(landings[Random.Range(0, landings.Length)]);
    }
}
