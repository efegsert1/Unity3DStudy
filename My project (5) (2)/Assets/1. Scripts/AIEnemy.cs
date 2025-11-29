using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;
using UnityEngine.AI;

public class AIEnemy : MonoBehaviour
{
    [Header("Enemy Setting")] //인스펙터에서 보기 좋게 구분하기 위해서
    public int maxHealth = 100; //적의 최대 체력
    private int currentHealth; //남아있는 체력을 계산하기 위해서


    [Header("AI Setting")] //AI(자동으로 움직이는 적) 설정 구역
    public Transform player; //따라가고 공격할 플레이어의 위치
    public float detectionRange = 15f; //플레이어를 감지하고 추적을 시작하는 범위(기즈모 빨간색)
    public float attackRange = 2f; //플레이어를 공격하는 밤위(기즈모 노란색)
    //공격 간격
    private float attackRate = 1.5f;
    //공격 쿨타임
    private float nextAttackTime = 0f;

    private NavMeshAgent agent;

    [Header("Animation")]
    private Animator anim;
    //애니메이터에서 "isWalking"이라는 이름을 숫자로 바꿔서 저장하는 코드
    //"isWalking : 학생 이름표" / HashIsWalking  : 학생 번호
    private readonly int HashIsWalking = Animator.StringToHash("isWalking");

    //readonly로 변수를 만든다
    //변수 이름은 HashIsAttacking이고 이 안에 들어갈 값은 애니메이터의 isAttacking이다.
    private readonly int HashIsAttacking = Animator.StringToHash("isAttacking");

    [Header("Effect")]
    public GameObject bloodEffectPrefab;

    //스위치처럼 끄고 / 켜고 할 수있는 것
    void OnEnable()
    {
        //풀에서 꺼낼 때 초기화
        currentHealth = maxHealth;

        if (agent  == null) agent = GetComponent<NavMeshAgent>();
        if (agent == null) anim = GetComponent<Animator>();

        //네비메쉬를 켠다.
        if (agent != null) agent.enabled = true;

        //Player자동 찾기
        if (player == null)
        {
            //Player 태그를 가진 오브젝트를 찾아서 playerObj라는 변수에 저장한다. 
            GameObject playerObj = GameObject.FindWithTag("Player");
            //플레이어 오브젝트를 찾았다면
            if (playerObj != null)
            {
                //그 오브젝트의 위치값을 변수에 넣어준다.
                player = playerObj.transform;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null || agent == null) return;
        // 만약 플레이어가 없으면 더 이상 작동하지 않고 멈춘다. 
        //플레이어가 있는 곳으로 길을 찾아 이동하도록 명령한다. 
        //SetDestination : 네비메쉬가 목표위치로 자동으로 이동하도록 명령하는 함수다.,
        //SetDestination(목표 위치)

        if (currentHealth <= 0) return;


        //플레이어와 적 사이의 거리를 잰다
        //Vector3.Distance(a, b) : 두 위치 a와 b 사이의 직선거리를 계산해서 돌려주는 함수다.
        //적 (0,0,0)  플레이어(0,0,12)
        //distance = 12
        float distance = Vector3.Distance(transform.position, player.position);

        //만약에 거리가 공격범위보다 멀면
        if (distance > attackRange)
        {
            //이동 모드를 바꾼다.
            //isStopped : "지금 멈춰 있을까?" 를 정하는 스위치
            //isStopped = true;   //이동을 멈춰
            //isStopped = false;   //계속 움직임
            agent.isStopped = false;
            agent.SetDestination(player.position);

        }
        //공격 
        else
        {
            //거리가 가깝다 = 공격범위 안이다 -> 이동을 멈춘다
            agent.isStopped = true;

            //지금 시간이 다음 공격 가능시간보다 크다면(= 쿨타임이 지났다면)
            //Time.time  : 게임이 시작된 후 흐른 시간을 알려주는 함수다.

            //게임 시작  10초 후 :10
            if (Time.time >= nextAttackTime)
            {
                //애니메이션이 잘 들어가 있다면
                if (anim != null)
                {
                    //공격 애니메이션을 한 번 재생하라고 신호를 보내
                    anim.SetTrigger(HashIsAttacking);
                }

                //플레이어의 체력을 깎고 싶으니, 플레이어에게 PlayerHealth 스크립트가 붙어있는지 찾는다.
                //var : 자료형을 자동으로 정해주는 키워드다 
                //int, float, double 

                // var number = 10;
                // int number = 10;
                var playerHealth = player.GetComponent<PlayerHealth>();

                //PlayerHealth가 있으면
                if (playerHealth != null)
                {
                    //플레이어 체력을 1만큼 줄인다.
                    playerHealth.TakeDamage(10); //1데미지
                }

                //다음에 다시 공격할 수 있는 시간을 정한다 (지금 시간 + 공격 시간)
                //난 빠르게 얻어맞고 싶은 사람들은 여기를 수정하시면 돼요~!

                //지금 시간에 공격 간격을 더해서, 다시 공격할 수 있는 시간을 정한다
                nextAttackTime = Time.time + attackRate;
            }
        }

        if (anim != null) //애니메이터가 비어있지 않는다면
        {
            //적이 움직이고 있는가? 를 검사하는 코드다
            //.magnitude : 속도의 "세기"를 숫자로 바꾼 값 (예 : 0이 멈춤, 2는 달리는 중)
            bool isWalking = agent.velocity.magnitude > 0.1f;
            anim.SetBool(HashIsWalking, isWalking);
        }
    }

    public void TakeDamage(int damage, Vector3 hitPoint)   // 적이 공격을 맞았을 때, 체력을 깎는 기능을 모아둔 함수
    {
        if (currentHealth <= 0) return;                   // 이미 체력이 0이라 죽어 있으면, 이 함수는 아무 것도 하지 않고 바로 끝낸다

        currentHealth -= damage;                          // 현재 체력에서 맞은 만큼(damage) 숫자를 빼서 체력을 줄인다
        Debug.Log($"{gameObject.name} 체력: {currentHealth}/{maxHealth}"); // 지금 이 적의 이름과 남은 체력을 콘솔에 글자로 보여준다

        // 피 이펙트 생성
        if (bloodEffectPrefab != null)                    // 피가 튀는 효과가 준비되어 있다면(없지 않다면)
        {
            GameObject effect = Instantiate(              // 피 효과 게임오브젝트를 새로 하나 만든다
                bloodEffectPrefab,                        // 어떤 것을 만들지: 미리 넣어둔 피 효과 프리팹
                hitPoint,                                 // 어디에 만들지: 총알이나 공격이 적을 맞춘 위치
                Quaternion.identity                       // 방향은 기본값으로 만든다 (회전 없음)
            );
            Destroy(effect, 1f);                          // 만든 피 효과를 1초 뒤에 자동으로 없앤다
        }

        // 체력이 다 닳으면 사망
        if (currentHealth <= 0)                           // 체력이 0 이하가 되었는지 다시 확인한다
        {
            Die();                                        // 체력이 없으면 Die() 함수를 불러서 적을 죽이는 처리를 한다
        }
    }
    private void Die()   // 적이 완전히 죽었을 때 실행되는 함수
    {
        Debug.Log(gameObject.name + " 사망!");
        // 콘솔에 "이 적은 죽었습니다!" 라고 글자를 보여준다

        if (agent != null) agent.enabled = false;
        // 적을 자동으로 움직이게 하는 NavMeshAgent가 있다면,
        // 죽었으니까 더 이상 움직이지 못하게 꺼버린다

        anim.SetTrigger("isDead");
        // 애니메이터에서 "죽는 애니메이션"을 실행한다

        CapsuleCollider cap = GetComponent<CapsuleCollider>();
        // 적 몸에 부딪힘을 감지하는 캡슐 모양의 충돌 박스를 찾아온다

        if (cap != null) Destroy(cap);
        // 그 충돌 박스가 있다면 없애서, 죽은 적이 더 이상 충돌되지 않게 한다

        //Destroy(gameObject, 3f);
        // 적 캐릭터를 3초 후에 게임 화면에서 완전히 삭제한다
        // (죽고 나서 바로 사라지지 않고, 쓰러져 있는 모습을 조금 보여주려고 3초 뒤에 없앤다)

        //3초 기다린 다음에 enemypool로 되돌리기
        StartCoroutine(ReturnToPoolAfterDelay(3f));
    }
    
    //일정 시간 뒤에 EnemyPool로 되돌리고
    private IEnumerator ReturnToPoolAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        EnemyPool.instance.ReturnEnemy(gameObject);
    }
}