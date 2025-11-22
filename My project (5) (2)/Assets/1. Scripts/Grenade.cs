using UnityEngine;

public class Grenade : MonoBehaviour
{
    public float delay = 3f; //폭발까지 시간
    public float exForce = 700f; //폭발 하는 힘
    public float exRadius = 5f; //폭발 범위
    public GameObject exEffect; //폭발 이펙트 프리팹

    private float countdown; //폭발하기 까지의 카운트 다운 변수
    private bool hasEx = false; //폭발했을 때 보여줄 이펙트

    void Start()
    {
        //countdown = 3;
       countdown = delay; //게임이 시작하면 카운트다운을 설정
    }

    
    void Update()
    {
        //1초에 1씩 줄어드는 타이머. 시간 흐르면 countDown이 줄어들게
        countdown -= Time.deltaTime;

        //시간이 다 되었고, 아직 안터졌다면
        if (countdown <= 0f && !hasEx)
        {
            //터지는 함수 호출
            Explode();
        }
    }

    //폭발 함수 만들기
    void Explode()
    {
        hasEx = true;

        //effect가 잘 들어가 있다면
        if (exEffect != null)
        {
            GameObject effect = Instantiate(exEffect, transform.position, transform.rotation);

            //이펙트가 2초뒤에 없어지게
            Destroy(effect, 2f);
        }

        //주변에 있는 오브젝트들을 모두 찾아낸다.
        //Physics.OverlapShere() : 그 범위 안에 들어있는 모든 물체를 찾는다.
        Collider[] colliders = Physics.OverlapSphere(transform.position, exRadius);

        //배열이나 리스트를 쓸 때 손쉽게 쓸 수있는 반복문이다!
        //foreach : 바구니(배열) 안에 있는 물건들을 하나씩 꺼내서 사용하게 해주는 문장!
        //문법 : foreach(자료형 변수이름 in 반복할 대상)
        // foreach(int age in 배열)
        //{
        //}

        foreach(Collider nearby in colliders)
        {
            //=적 처리==
            //GetComponent : 겉에 있는 컴포넌트만 찾는다(자식을 찾을 순 없음)

            //GetComponentInChildren : 자식에서부터 부모까지 싹 다 스크립트 찾아라
            FirstEnemyAi enemy = nearby.GetComponentInChildren<FirstEnemyAi>();

            //만약에 그 오브젝트가 '적'이라면?
            if (enemy != null)
            {
                //데미지를 입히기
                enemy.TakeDamage(2147483647, transform.position);
            }

            //물리 반응
            Rigidbody rb = nearby.GetComponent<Rigidbody>();
            //근처오브젝트가 '리지드바디'(물리 반응을 하는 몸체)를 갖고있는지

            if (rb != null)
            {
                //폭발 하는 힘을 사용해 주변 오브젝트를 밀어낸다
                //AddExplosionForce : 폭발하는 힘을 작용하여, 특정 위치에 퍼져나가는 힘을 준다. 리지드바디에
                //rb.AddExplosionForce(힘의 세기, 폭발 위치, 폭발 범위);
                rb.AddExplosionForce(exForce, transform.position, exRadius);
            }
        }

        //수류탄 본체(gameobject)를 게임에서 제거
        //GameObject : 자료형
        //gameObject : 물건
        Destroy(gameObject);
    }
}
