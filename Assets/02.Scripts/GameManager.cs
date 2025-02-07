using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip clip;
    public GameObject ClearPanel;
    private bool isClipPlaying = false; //클리어 음악 재생중인지 확인
    
    public void GameClear()
    {
        if (!isClipPlaying)
        {
            audioSource.PlayOneShot(clip);
            isClipPlaying = true; 
        }
        ClearPanel.SetActive(true);
    }
}