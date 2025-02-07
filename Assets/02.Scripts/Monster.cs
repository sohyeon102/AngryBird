using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Monster : MonoBehaviour
{
    Animator anim;
    Animator anim2; //데미지 텍스트
    private int Count = 0;
    private bool isDead = false;
    
    //Damage Text
    public Transform damagePosition; // 몬스터 머리 위 위치
    public GameObject damagePrefab; // 데미지 텍스트 프리팹
    
    //Audio
    public AudioClip Sound; 
    public float destroyDelay = 2f; // 삭제 전 대기 시간

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bird") && !isDead)
        {
            anim.SetBool("Dead", true);
            ShowDamageAnimation();
            MonsterDeadSound();
            isDead = true;
        }
        else if (collision.gameObject.CompareTag("Object"))
        {
            Count++; //충돌 횟수 증가
            if (Count >= 2)
            {
                anim.SetBool("Hurt", true); // 두 번째 충돌 이후 Hurt 설정
            }
        }
    }

    public void DestroyMonster()
    {
        Destroy(gameObject);
    }

    void ShowDamageAnimation()
    {
        if (damagePrefab != null && damagePosition != null)
        {
            GameObject damageInstance = Instantiate(damagePrefab, damagePosition.position, Quaternion.identity);
            damageInstance.transform.SetParent(damagePosition); // 생성한 프리팹의 부모를 몬스터로 설정 
            damageInstance.transform.localPosition = Vector3.zero; // 로컬 위치 초기화

            Animator damageAnimator = damageInstance.GetComponent<Animator>();
            if (damageAnimator != null)
            {
                damageAnimator.SetTrigger("Damage"); 
            }
            
            Destroy(damageInstance, 2f);
        }
    }
    
    public void MonsterDeadSound()
    {
        PlaySoundAndDestroy();
    }
    private void PlaySoundAndDestroy()
    {
        if (Sound != null)
        {
            GameObject tempAudioObject = new GameObject("TempAudio");
            AudioSource tempAudioSource = tempAudioObject.AddComponent<AudioSource>();
            
            tempAudioSource.clip = Sound;
            tempAudioSource.playOnAwake = false;
            tempAudioSource.Play();
            
            Destroy(tempAudioObject, Sound.length);
        }
        Destroy(gameObject, destroyDelay);
    }

}
