using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandingAudio : MonoBehaviour
{
    AudioManager audioManager;
    [Range(0f, 5f)]
    public float timeoutLength = 1.5f;
    bool readyToPlay = true;
    public bool firstCollisionBool = false;
    string[] landings = new string[] {"Landing1", "Landing2", "Landing3"};
    void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
    }

    private void OnCollisionEnter(Collision other) {
        if (readyToPlay)
        {
            if (firstCollisionBool)
            {
                audioManager.Play(landings[Random.Range(0, landings.Length)]);
                StartCoroutine(LandingAudioTimeout());
            } else firstCollisionBool = true;
        }
    }

    IEnumerator LandingAudioTimeout()
    {
        Debug.Log("Hello?");
        readyToPlay = false;
        yield return new WaitForSeconds(timeoutLength);
        readyToPlay = true;
  }
}
