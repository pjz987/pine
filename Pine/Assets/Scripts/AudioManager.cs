﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // bool fadeBool = true;  // true for start game false for end game
    public Sound[] sounds;
    public static AudioManager instance;

    [HideInInspector]
    public string[] treeGroansShort = new string[]
    {
        "TreeGroanShort1",
        "TreeGroanShort2",
        "TreeGroanShort3",
        "TreeGroanShort4",
        "TreeGroanShort5",
        "TreeGroanShort6",
        "TreeGroanShort7",
        "TreeGroanShort8",
        "TreeGroanShort9",
        "TreeGroanShort10",
        "TreeGroanShort11",
    };

    [HideInInspector]
    public string[] treeGroansLong = new string[]
    {
        "TreeGroan1",
        "TreeGroan2",
        "TreeGroan3",
        "TreeGroan4",
    };

    [HideInInspector]
    public string[] treeGroansAll = new string[]
    {
        "TreeGroan1",
        "TreeGroan2",
        "TreeGroan3",
        "TreeGroan4",
        "TreeGroanShort1",
        "TreeGroanShort2",
        "TreeGroanShort3",
        "TreeGroanShort4",
        "TreeGroanShort5",
        "TreeGroanShort6",
        "TreeGroanShort7",
        "TreeGroanShort8",
        "TreeGroanShort9",
        "TreeGroanShort10",
        "TreeGroanShort11",
    };
    [HideInInspector]
    public string[] treeFlickWhooshes = new string[]
    {
            "TreeWhoosh1",
            "TreeWhoosh2"
    };
    [Header("Screen Wipe SFX Delay")]
    public float waitForScreenAudio = 1.25f;
    [Header("Crossfade on Victory Screen")]
    [Range(0f, 20f)]
    public float crossfadeTime = 10f;

    private Coroutine coroutine;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    void Start () {
        Play("Ambience");
    }

    public void Play (string name, float volume = 0f, float pitch = 0f) 
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.Log("Could not find sound of name " + name);
            return;
        }
        if (volume != 0f) {
            s.source.volume = volume;
        }
        if (pitch != 0f) {
            s.source.pitch = pitch;
        }
        s.source.Play();
    }

  // the following fade out method was found here: https://gamedevbeginner.com/how-to-fade-audio-in-unity-i-tested-every-method-this-ones-the-best/

    private static IEnumerator StartFade (AudioSource audioSource, float duration, float targetVolume)
    {
        float currentTime = 0;
        float start = audioSource.volume;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
            yield return null;
        }
        yield break;
    }

    public void CallFadeCoroutine (string name, float duration, float targetVolume)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.Log("Could not find sound of name " + name);
            return;
        }
        // if (coroutine != null) StopCoroutine(coroutine);
        // coroutine = StartCoroutine("StartFade", (s.source, duration, targetVolume));
        // StopAllCoroutines();
        StartCoroutine(StartFade(s.source, duration, targetVolume));
        // StopStart(s.source, duration, targetVolume);
       
    }

    // private IEnumerator StopStart (AudioSource source, float duration, float targetVolume)
    // {
    //     StopAllCoroutines();
    //     yield return null;
    //     StartCoroutine(StartFade(source, duration, targetVolume));
    // }

    public void PlayRandom (string[] names)
    {
        Play(names[UnityEngine.Random.Range(0, names.Length)]);
    }

    public void PlayWaitPlay(string name1, float wait, string name2)
    {
        StartCoroutine(PlayWaitPlayEnumerator(name1, wait, name2));
    }

    public IEnumerator PlayWaitPlayEnumerator (string name1, float wait, string name2)
    {
        Play(name1);
        yield return new WaitForSeconds(wait);
        Play(name2);
    }

    public void PlayDelayed(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.Log("Could not find sound of name " + name);
            return;
        }
        s.source.PlayDelayed(waitForScreenAudio);
    }

    public void MusicInAmbienceOut()
    {
        CallFadeCoroutine("Ambience", 2f, 0f);
        CallFadeCoroutine("Music", 0.01f, 1f);
        Play("Music");
    }

    public void AmbienceInMusicOut()
    {
        CallFadeCoroutine("Music", crossfadeTime, 0f);
        CallFadeCoroutine("Ambience", crossfadeTime, 1f);
    }
}
