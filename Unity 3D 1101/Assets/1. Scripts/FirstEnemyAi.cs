using UnityEngine;
using Unity.AI;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine.AI; //길찾기(NavMesh)를 사용하기 위해서 쓴다.

public class FirstEnemyAi : MonoBehaviour
{
    [Header("Enemy Setting")] //인스펙터에서 보기 좋게 구분하기 위해서
    public int maxHealth = 100; //적의 최대 체력
    private int currentHealth; //남아있는 체력을 계산하기 위해서

    [Header("AI Setting")] //AI(자동으로 움직이는 적) 설정 구역
    public Transform player; //따라가고 공격할 플레이어의 위치
    public float detectionRange = 15f; //플레이어를 감지하고 추적을 시작하는 범위(기즈모 빨간색)
    public float attackRange = 2f; //플레이어를 공격하는 범위(기즈모 노란색)

    private NavMeshAgent agent;
    private Animator anim;
    
    void Start()
    {
        agent = GetComponent<NavMeshAgent>(); //NevMeshAgent 가져오기
        anim = GetComponent<Animator>();
        currentHealth = maxHealth; //시작할 때 체력을 최대로 설정

        //비어있는지 아닌지
        if (agent == null)
        {
            Debug.LogWarning("NavMesh 컴포넌트가 없습니다. EnemyAI 가 제대로 작동하지 않을 수 있습니다.");
        }
    }
    
    void Update()
    {
        if (player == null) return; //만약 플레이어가 없으면 더 이상 작동하지 않고 멈춘다.

        //플레이어가 있는 곳으로 길을 찾아 이동하도록 명령한다
        //SetDestination : NavMesh가 목표위치로 자동으로 이동하도록 명령하는 함수다.
        //SetDestination(목표 위치)
        agent.SetDestination(player.position);
    }
}
