using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

/*
    Name: Alec Reyerson
    ID: 1826582
    Email: reyer101@mail.chapman.edu
    Course: CPSC-344-01
    Assignment: Gold Milestone

    Description: Script ensures that background music loops between scenes.
    */

// MusicManager
public class MusicManager : MonoBehaviour {
    string prevScene, currScene;
    AudioSource audio;

    // Awake
	void Awake()
    {
        audio = GetComponent<AudioSource>();
        currScene = SceneManager.GetActiveScene().name;
        prevScene = currScene; 
        DontDestroyOnLoad(GetComponent<AudioSource>());
    }	

    // Update
    void Update()
    {
        currScene = SceneManager.GetActiveScene().name;
        if(currScene != prevScene)
        {
            Debug.Log("Changed scenes");
            if(currScene == "1-1KH")
            {
                Debug.Log("Loading main scene audio");
                audio.clip = Resources.Load("Audio/MainLevel") as AudioClip;
                audio.Play();
            }
            else if(currScene == "BossFight")
            {
                Debug.Log("Loading boss scene audio");
                audio.clip = Resources.Load("Audio/BossLevel") as AudioClip;
                audio.Play();
            }
            prevScene = currScene;
        }
    }
}
