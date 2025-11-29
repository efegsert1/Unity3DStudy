using UnityEngine;

public class Spawner : MonoBehaviour
{
    //위치값의 배열
    public Transform[] spawnPoints;

    //적이 몇초마다 한 번씩 나타날지 정해주는 시간
    public float spawnInterval = 5; //5초 마다

    //시간이 얼마나 지났는지 체크하는 변수
    private float timer;

    void Start()
    {

    }

    void Update()
    {
        timer += Time.deltaTime; //1초에 1씩 timer가 증가한다(게임 실제 흐르는 시간)

        //만약에 timer가 spawnInterval보다 크면 적을 소환
        //timer : 1초, 2초 ...
        //5초가 되면 나가라
        if (timer >= spawnInterval)
        {
            //다시 0초로 초기화되서 다음 5초를 기다림
            timer = 0;

            //적을 소환하는 함수()
            SpawnEnemy();
        }
    }

    //적을 실제로 소환하는 함수

    void SpawnEnemy()
    {
        //만약에 적이 너무 많으면 새로운 적을 만들지 말고 돌아가라
        if (EnemyPool.instance.ActiveEnemyCount >= 5)
        {
            return;
        }

        //랜덤한 위치를 뽑기(spawnPoints 중 한 곳)
        //배열의 길이 : 배열명.Length
        int index = Random.Range(0, spawnPoints.Length);

        //EnemyPool에서 적 하나 꺼내와서 지정된 자리에 배치
        GameObject enemyobj = EnemyPool.instance.GetEnemy(spawnPoints[index].position, Quaternion.identity);

        //배열 : 상자[0] 상자[1], 상자[2]

        //Player 자동 연결
        FirstEnemyAi enemy = enemyobj.GetComponent<FirstEnemyAi>();

        if (enemy != null && enemy.player == null)
        {
            GameObject playerobj = GameObject.FindWithTag("Player");
        }
    }
}