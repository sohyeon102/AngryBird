using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgmPlayer : MonoBehaviour
{
    AudioSource audioSource;
    public AudioClip bgSound;

    public void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = bgSound;
        audioSource.Play();
    }
}
