using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [Header("이동 관련 설정")]
    public float moveSpeed = 5f;  //이동 속도 변수
    Rigidbody rb;  //플레이어의 rigidbody 컴포넌트 변수
     
    Animator anim; //플레이어의 애니메이터 컴포넌트 변수

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //리지드바디 컴포넌트 가져오기
        rb = GetComponent<Rigidbody>();

        //애니메이터 컴포넌트 가져오기
        anim = GetComponent<Animator>();
        
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    } 

    //플레이어 움직이는 함수 만들기
    void Move()
    {
        //1. 입력 받기
        //"Horizontal" : A(-1)왼 ,D(+1)오른
        float h = Input.GetAxis("Horizontal");
        
        //"Vertical" : S(-1)아래, W(+1)위
        float v = Input.GetAxis("Vertical");

        //2. 이동 방향 계산
        //transform.right : 플레이어의 오른쪽 방향
        //transform.forward : 플레이어의 앞 방향
        Vector3 moveDir = transform.right * h + transform.forward * v;

        //3. 기존의 y축 속도 유지(중력 영향 보존)
        Vector3 newVel = new Vector3(moveDir.x * moveSpeed, rb.linearVelocity.y, moveDir.z * moveSpeed);

        //4. 계산된 속도로 리지드바디 이동 적용(최종 적용)
        rb.linearVelocity = newVel;

        //h가 0이 아니거나, v가 0이 아니면 true
        //-> 플레이어가 움직임 입력(wsad)를 하고 있다면 true
        //플레이어가 어떤 방향으로든 이동 키를 누르고 있다면 true
        bool isWalking = (h != 0 || v != 0);
        anim.SetBool("isWalk", isWalking);


    }



}
