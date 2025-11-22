using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [Header("카메라 및 이펙트")]
    public Transform playerCamera; // 플레이어의 시점 카메라(총알이 발사되는 기준 방향)
    public ParticleSystem hitEffect;
    public CameraShake camShake; //카메라 스크립트를 쓰겠다
    Animator anim;

    [Header("총기 설정")]
    public float fireRate = 0.1f;  //연사 속도
    private float nextFireTime = 0f; //다음 발사 가능 시간

    [Header("사운드 설정")]
    public AudioSource audioSource;//총 소리 재생을 담당하는 audioSource
    public AudioClip shootSfxSingle; // 단발 발사 시 재생할 오디오 클립
    public AudioClip shootSfxAuto; // 연사 발사 시 루프 재상할 오디오 클립

    [Header("수류탄 설정")]
    //수류탄 프리팹 변수 만들기(자료형은 게임오브젝트이고, 변수명은 grenadePrefab이다)
    public GameObject grenadePrefab;
    public Transform throwPoint; //던질 위치(예 : 카메라 앞, 캐릭터의 손 위치)
    public float throwForce = 15f;
    public float throwUpforce = 5f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Shoot();
        HandleAutoFireSoundStop(); // 오른쪽  클릭 떼면 소리 멈추기

        //수류탄 던지기
        if (Input.GetKeyDown(KeyCode.F))
        {
            //수류탄 던지는 함수 호출
            ThrowGrenade();
        }
    }

    //사운드 처리 함수
    //void 함수이름(매개변수)
    //AudioClip : 소리 데이터를 담고 있는 음원 파일   *CD안의 노래 파일
    //AudioSource : 그 소리를 실제로 재생하는 스피커 역할을 하는 컴포넌트 * CD플레이어
    void PlayShootSound(AudioClip clip, bool loop)
    {
        //audioSource또는 클립이 없으면 종료
        if (audioSource == null || clip == null) return;

        if (loop)
        {
            audioSource.clip = clip;  //재생할 클립 설정
            audioSource.loop = true; // 루프 모드 활성화
            audioSource.Play();     //재생 시작
        }

        else
        {
            //PlayOneShot 소리를 딱 한 번만 재생해!
            audioSource.PlayOneShot(clip);  // 겹쳐서 1회 재생
        }

    }

    //오른쪽 클릭을 떼면 루프사운드 중지
    void HandleAutoFireSoundStop()
    {
        //마우스 오른쪽 버튼에서 손을 뗀 순간
        if (Input.GetMouseButtonUp(1))
        {
            //현재 오디오가 연사용 클립을 재생 중인지 확인
            //오디오소스가 지금 소리를 재생 중이고, 그 소리가 shootSfxAuto 클립일 때만
            if (audioSource.isPlaying && audioSource.clip == shootSfxAuto)
            {
                //오디오 소스 재생 중지
                audioSource.Stop();
                audioSource.clip = null; //클립 참조 해제
                audioSource.loop = false; //루프 재생 플레그 해제
            }

        }
    }
    
//총기 발사 함수
void Shoot()
    {
        //왼쪽 클릭 -> 단발 발사
        if (Input.GetMouseButtonDown(0))
        {
            FireBullet(); //실제 발사 처리를 실행
            PlayShootSound(shootSfxSingle, false);
        }

        //오른쪽 클릭 -> 연사 시작(일정 시간 마다 자동 발사)
        if (Input.GetMouseButtonDown(1))
        {
            PlayShootSound(shootSfxAuto, true);
        }

        //오른쪽 마우스 버튼을 클릭한다면
        //발사처리 실행 
        //현재시간(Time.time)이 다음 발사 가능 시각(nextFireTime)보다 크거나 같다면
        if (Input.GetMouseButton(1) && Time.time >= nextFireTime)
        {
            //다음 발사 가능 시각을 갱신
            nextFireTime = Time.time + fireRate;

            //Time.time(현재 시간) 2.0
            //fireRate = 0.5
            //nextFireTime = 2.0 + 0.5 = 2.5
            //2.5초가 되어야 다음 총알을 쏠 수 있다.

            FireBullet();
        }
    }

    //실제 발사 로직(Raycast)
    //Ray(광선) + Cast(쏘다, 던지다)
    //보이지 않는 선을 쏴서 무언가 맞는지 확인한다.
    void FireBullet()
    {
        //SetTirgger -> 애니메이션을 "한 번만" 실행시키기 위한 명령
        //애니메이션안의 트러거 파라미터를 true로 잠깐 켜서
        //애니메이션이 즉시 재생되게 만드는 함수다.
        anim.SetTrigger("Shoot");

        //SetBool vs SetTrigger


        //카메라 방향으로 광선 쏘기
        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        //레이 시작점 : 카메라 위치
        //레이 방향 : 카메라가 바라보는 방향

        //충돌 정보를 담을 변수 선언
        RaycastHit hit;

        //100m까지 레이캐스트 검사, 맞으면 true
        //Physics.Raycast : 보이지 않는 광선을 쏘아, 물리적으로 어떤 오브젝트에 닿았는지 검사하는 함수
        //out hit : 맞은 오브젝트의 충돌 정보를 hit 변수에 담는다.
        //100f : 광선 최대 거리(100f)
        if (Physics.Raycast(ray, out hit, 100f))
        {
            Debug.Log("Hit : " + hit.collider.name);

            //맞은 위치에 파티클 이펙트 생성
            if (hitEffect != null)
            {
                ParticleSystem ps = Instantiate(hitEffect, hit.point, Quaternion.LookRotation(hit.normal));

                //Instantiate : 유니티에서 오브젝트를 복제(생성)하는 함수
                //hitEffect : 복제할 원본 프리펩(인스펙터에 연결된 파티클 이펙트 프리펩)
                //hit.point : 총알이 맞은 정확한 위치
                //Quaternion.LookRotation(hit.normal) : 맞은 표면의 방향을 바라보도록 회전시킴

                ps.Play();
                //파티클을 지워주는 함수 , 1초
                Destroy(ps.gameObject, 1f);
            }

            //적 데미지 주기
            //맞은 오브젝트에서 적 스크립트를 찾는다.
            FirstEnemyAi enemy = hit.collider.GetComponent<FirstEnemyAi>();

            if (enemy != null)
            {
                int damage = 10; //한 번 맞을 때 줄 데미지 값, 원하는대로 조정
                enemy.TakeDamage(damage, hit.point);
            }


            //카메라 반동 효과
            //cameShake가 비어있지 않고 잘 넣어져 있다면
            if (camShake != null)
            {
                //코루틴 실행
                StartCoroutine(camShake.RecoilShake(0.2f, 0.3f));
            }

            //리지드바디 있는 물체 밀어내기
            Rigidbody rb = hit.collider.GetComponent<Rigidbody>();
            // 맞은 대상의 리지드바디를 획득

            if (rb != null)
            {
                //플레이어 카메라의 위치에서 피격지점까지 향하는 방향 벡터를 구하라
                Vector3 forceDir = hit.point - playerCamera.position;

                //playerCamera.position (0,1,0) ->플레이어 머리 위치
                //hit.point(0,1,10)-> 10미터 앞 벽에 부딪힘
                //(0,1,10) - (0,1,0) -> (0,0,10)

                //벡터의 길이를 1로 맞춰서 방향만 남기는 함수
                // 이쪽으로 힘을 줘! 
                forceDir.Normalize();

                //피;격 지점에 힘을 가해 밀어냄
                //AddForceAtPosition : 특정 위치에 힘을 가한다
                rb.AddForceAtPosition(forceDir * 1000f, hit.point);
            }
        }
    }

    //수류탄을 던지는 기능을 가지는 함수다.
    void ThrowGrenade()
    {
        //만약에 수류탄이 프리팹에 있고, 던지는 위치가 있으면 실행(예외처리)
        if (grenadePrefab != null && throwPoint != null)
        {
            //수류탄을 throwPoint 위치에서 새로 만들어낸다(Instantiate = 복사해서 생성)
            //1. 게임오브젝트라는 자료형을 가진 grenade변수를 만든다.
            //2. Instantiate를 사용해서 (프리팹 원본, throwPoint.position, throwPoint.rotation);

            GameObject grenade = Instantiate (grenadePrefab, throwPoint.position, throwPoint.rotation);

            //만들어진 수류탄에서 리지드바디를 찾아서 가져온다.
            Rigidbody rb = grenade.GetComponent<Rigidbody>();

            if (rb != null)
            {
                //playerCamera.foward : 카메라가 바라보는 앞 방향 : (0,0,1)
                //throwForce : 앞쪽으로 밀어주는 힘(던지는 힘) : 10
                //Vector3.up : (0,1,0)
                //throwUpForce : 위로 올려주는 힘 : 10

                //수류탄을 어느 방향으로, 얼마나 세게 던질지 계산하는 공식
                //앞으로 던지는 힘 + 위로 올리는 힘 = 수류탄 날아가는 힘
                Vector3 force = playerCamera.forward * throwForce + Vector3.up * throwForce;
                // (0,0,1) + 10 + (0,1,0) * 10

                //만든 힘을 수류탄에 '한 번 세게' 준다(Inpulse = 순간적인 힘)
                rb.AddForce(force, ForceMode.Impulse);
            }
        }
    }
}


