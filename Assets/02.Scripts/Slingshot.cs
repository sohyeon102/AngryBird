using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;
using Cinemachine;

public class Slingshot : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public LineRenderer[] lineRenderers;
    public Transform[] stripPositions;
    public GameObject birdPrefab;
    public Transform idlePosition;
    public Transform center;
    public Vector3 currentPosition;
    public float birdOffset;
    public float Power = 1f;
    private Rigidbody2D bird;
    private Collider2D birdCollider;
    private Coroutine trajectoryCoroutine; //

    private Camera MainCamera;
    private Vector3 startPosition; 
    private Vector3 pullPosition;
    [SerializeField] private float maxPullDistance;
    
    // Audio
    public AudioClip dragClip;  //드래그 소리
    public AudioClip launchClip; // 새 발사
    private AudioSource audioSource;
    private bool isDragging; // 드래그 중인지 확인
    private float loopStart = 0.37f; // 반복 시작 시간
    private float loopEnd = 0.67f;   // 반복 종료 시간 
    
    // Cinemachine Virtual Camera
    public CinemachineVirtualCamera vcam;

    private void Awake()
    {
        MainCamera = Camera.main;
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = dragClip; // 드래그 오디오 설정
        audioSource.loop = false;
    }

    void Start()
    {
        startPosition = center.position;

        lineRenderers[0].positionCount = 2;
        lineRenderers[1].positionCount = 2;
        lineRenderers[0].SetPosition(0, stripPositions[0].position);
        lineRenderers[1].SetPosition(0, stripPositions[1].position);

        CreateBird();
    }

    void CreateBird()
    {
        bird = Instantiate(birdPrefab).GetComponent<Rigidbody2D>();
        birdCollider = bird.GetComponent<Collider2D>();
        birdCollider.enabled = false;

        bird.isKinematic = true;

        ResetStrips();
    }

    void ResetStrips()
    {
        currentPosition = idlePosition.position;
        SetStrips(currentPosition);
    }

    void SetStrips(Vector3 position)
    {
        lineRenderers[0].SetPosition(1, position);
        lineRenderers[1].SetPosition(1, position);

        if (bird)
        {
            Vector3 dir = position - center.position;
            bird.transform.position = position + dir.normalized * birdOffset;
            bird.transform.right = -dir.normalized;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        birdCollider.enabled = true;
        pullPosition = MainCamera.ScreenToWorldPoint(
            new Vector3(eventData.position.x,
                eventData.position.y,
                MainCamera.WorldToScreenPoint(transform.position).z));
        isDragging = true;
        PlayDragAudio(); // 드래그 시작 시 오디오 재생
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (MainCamera != null)
        {
            Vector3 mouseWorldPos = MainCamera.ScreenToWorldPoint(
                new Vector3(eventData.position.x,
                eventData.position.y,
                MainCamera.WorldToScreenPoint(transform.position).z));

            Vector3 pullDirection = startPosition - mouseWorldPos;

            Vector3 newBirdPosition;
            if (pullDirection.magnitude > maxPullDistance)
            {
                pullDirection = pullDirection.normalized * maxPullDistance;
                newBirdPosition = startPosition - pullDirection;
            }
            else
            {
                newBirdPosition = mouseWorldPos;
            }

            if (bird != null)
            {
                bird.position = newBirdPosition;
                SetStrips(newBirdPosition);
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
        StopDragAudio(); // 드래그 종료 시 오디오 중지
        
        if (bird != null)
        {
            bird.isKinematic = false;
            birdCollider.enabled = true;
            
            PlayLaunchAudio(); // 새 발사 소리 

            Vector3 launchForce = (startPosition - (Vector3)bird.position) * Power;
            launchForce.y += 2f;
            bird.velocity = launchForce;
            
            Destroy(bird.gameObject, 7f);

            StartCoroutine(ResetAfterLaunch());
        }
    }

    IEnumerator ResetAfterLaunch()
    {
        yield return new WaitForSeconds(2);
        CreateBird();
    }
    
    private void PlayDragAudio()
    {
        if (audioSource != null && dragClip != null)
        {
            audioSource.time = loopStart; // 반복 시작 위치로 설정
            audioSource.Play();
            StartCoroutine(LoopDragAudio());
        }
    }

    private IEnumerator LoopDragAudio()
    {
        while (isDragging)
        {
            if (audioSource.time >= loopEnd)
            {
                audioSource.time = loopStart; // 반복 시작 위치로 되돌림
            }
            yield return null;
        }
    }

    private void StopDragAudio()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
            StopCoroutine(LoopDragAudio());
        }
    }
    private void PlayLaunchAudio() // 새 발사 오디오 
    {
        if (audioSource != null && launchClip != null)
        {
            audioSource.Stop(); 
            audioSource.clip = launchClip; 
            audioSource.loop = false;    
            audioSource.Play();          
        }
    }
}
