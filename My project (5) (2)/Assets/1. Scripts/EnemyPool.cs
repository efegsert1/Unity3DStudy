using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices; //자료구조를 쓰기 위함
//처음에 적 여러명을 미리 만들어서 상자에 넣는것.
//필요할 때는 꺼내서 쓰고, 다 쓰면 상자에 넣어놓는것

//오브젝트 풀링(Object pooling)
//왜 생겼을까 ? 몬스터 / 이펙트 .. 계속 새로 만들고, 필요없을 때 지우면
//컴퓨터가 너무 느려진다.

//게임이 시작할때 미리 여러개 만들어서 보관해두는 방법
//필요할 때 꺼내 쓰고, 다 쓰면 다시 넣는 것.

public class EnemyPool : MonoBehaviour
{
    public static EnemyPool instance; //게임 안에서 EnemyPool을 하나만 만들기 위해서 쓰는 특별한 변수다.

    [Header("풀링할 enemy 프리팹")]
    public GameObject enemyPrefab;

    [Header("풀 크기")]
    public int poolSize = 10; // 상자에 몇마리가 처음에 들어갈 지

    //안쓰는 적들이 줄을 서서 기다리는 상자
    private Queue<GameObject> pool = new Queue<GameObject>();

    //지금 게임에 나와 움직이는 적 목록
    private List<GameObject> activeEnemy = new List<GameObject>();

    //현재 살아있는 적의 수
    //=> 화살표 연산자 : 오른쪽 값(결과를 그대로 돌려준다)
    //왼쪽은 오른쪽으로 만든 값이다

    public int ActiveEnemyCount => activeEnemy.Count;
    //int Double => value * 2;

    //start보다 더 빠르게 실행되는 것이 Awake다
    private void Awake()
    {
        instance = this; //EnemyPool을 하나만 쓰도록 지정하는 코드다.
    }

    void Start()
    {
        
    }

    
    void Update()
    {
        
    }
}
