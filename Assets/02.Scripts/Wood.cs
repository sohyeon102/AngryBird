using System;
using UnityEngine;

public class Wood : MonoBehaviour
{
    Animator anim;
    Animator anim2; //데미지 텍스트
    public GameObject damagePrefab; // 데미지 텍스트 프리팹
    private AudioSource audioSource;
    private bool isDead = false;
    public AudioClip Sound; 
    public float destroyDelay = 2f; // 삭제 전 대기 시간
    public float volume = 0.5f;

    void Awake()
    {
        anim = GetComponent<Animator>();
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false; 
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isDead && collision.gameObject.CompareTag("Bird"))
        {
            isDead = true;
            anim.SetBool("Dead", true);
            ShowDamageAnimation();
            DestroyObject(); //임시 오브젝트 삭제
        }
    }

    public void DestroyWood()
    {
        Destroy(gameObject);
    }
    
    public void DestroyObject()
    {
        PlaySoundAndDestroy();
    }

    void ShowDamageAnimation()
    {
        if (damagePrefab != null)
        {
            GameObject damageInstance = Instantiate(damagePrefab, transform.position, Quaternion.identity);
            damageInstance.transform.SetParent(transform); // 생성한 프리팹 부모를 현재 오브젝트로 설정
            damageInstance.transform.localPosition = Vector3.zero; // 로컬 위치 초기화

            Animator damageAnimator = damageInstance.GetComponent<Animator>();
            if (damageAnimator != null)
            {
                damageAnimator.SetTrigger("Damage"); 
            }
            Destroy(damageInstance, 3f);
        }
    }
    
    private void PlaySoundAndDestroy()
    {
        if (Sound != null)
        {
            GameObject tempAudioObject = new GameObject("TempAudio"); // 임시 GameObject 생성
            AudioSource tempAudioSource = tempAudioObject.AddComponent<AudioSource>();
            
            tempAudioSource.clip = Sound;
            tempAudioSource.playOnAwake = false; //오디오 꺼두기
            tempAudioSource.volume = volume; //소리크기
            tempAudioSource.Play();

            // 오디오가 끝난 후 임시 GameObject 삭제
            Destroy(tempAudioObject, Sound.length);
        }

        // 원래 오브젝트 삭제
        Destroy(gameObject, destroyDelay);
    }
}