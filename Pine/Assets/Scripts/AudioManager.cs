using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
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
            // Debug.Log("This happened");
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    void Start () {
        Play("Music");
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
}
