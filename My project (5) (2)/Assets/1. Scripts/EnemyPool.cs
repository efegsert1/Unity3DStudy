using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System; //자료구조를 쓰기 위함
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
        //게임이 시작되면 미리 정해진 숫자만큼 적을만들어서 보관한다.
        for (int i = 0; i < poolSize; i++) //0, 1, 2, 3, 4, 5, 6, 7, 8, 9
        {
            GameObject obj = Instantiate(enemyPrefab);
            obj.SetActive(false); //처음오브젝트는 꺼둠
            pool.Enqueue(obj);

            //큐 : 편의점에 있는 음료수 같은거다
            //가장 먼저 들어온 데이터가 가장 먼저 나가는 구조
            //FIFO : First In First Out

            //핵심 동작
            //Enqueue(엔큐) : 데이터를 큐에 넣는것
            //Dequeue(디큐) : 큐에서 데이터를 꺼내는 것

            //큐 : [A,B,C] -> [A,B,C]
        }
    }

    //풀(상자)에서 적을 꺼내 사용하는 기능
    public GameObject GetEnemy(Vector3 position, Quaternion rotation)
    {
        GameObject enemy; //변수 생성

        //풀에 있는 애들이 0보다 여러명 있다면
        if (pool.Count > 0)
        {
            //상자 안에 쉬고 있는 적을 꺼내오기
            enemy = pool.Dequeue();
        }

        else
        {
            enemy = Instantiate(enemyPrefab);
        }

        //적을 켜기
        enemy.SetActive(true);

        //적의 위치를 정해진 위치로 이동시키기
        enemy.transform.position = position;

        //적의 방향을 정해진 각도로 돌리기
        enemy.transform.rotation = rotation;

        //활동 중인 적 리스트에 적을 추가함
        //왜 필요할까?
        //지금 필드에 있는 적들을 추적해야하기 떄문에(적이 몇마리 있는지, 다 죽었는지)
        // 적을 되돌릴때 (active -> pool)
        //-> 추적, 관리, 삭제하기 위해서 꼭 필요한 코드다
        activeEnemy.Add(enemy); //Add : 추가하는 것

        return enemy;
        //방금 꺼낸 적을 스폰한 쪽에게 넘겨준다.
    }

    //적을 다시 풀로 돌려보내기
    public void ReturnEnemy(GameObject enemy)
    {
        //적을 끔
        enemy.SetActive(false);
        //다시 상자(pool)에 넣음
        pool.Enqueue(enemy);

        //활동중 리스트에서는 제거 (리스트에서 제거하는 방법은 Remove)
        activeEnemy.Remove(enemy);
    }

    void Update()
    {
           
    }
}