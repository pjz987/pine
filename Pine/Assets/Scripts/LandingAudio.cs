using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandingAudio : MonoBehaviour
{
    AudioManager audioManager;
    [Range(0f, 3f)]
    public float timeoutBetweenLandingSounds = 1;
    public bool firstCollisionBool = true;
    private bool readyForAudio = true;
    string[] landings = new string[] {"Landing1", "Landing2", "Landing3"};
    void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
    }

    private void OnCollisionEnter(Collision other) {
        if (readyForAudio && !firstCollisionBool) {
            audioManager.Play(landings[Random.Range(0, landings.Length)]);
            StartCoroutine(AudioTimeout());
        }
        firstCollisionBool = false;
    }

    IEnumerator AudioTimeout () {
        readyForAudio = false;
        yield return new WaitForSeconds(timeoutBetweenLandingSounds);
        readyForAudio = true;
    }
}
