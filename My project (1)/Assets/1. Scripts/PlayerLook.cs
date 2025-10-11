using Unity.VisualScripting;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    [Header("마우스 회전 설정")]
    public float mouseSen = 100f; //마우스 감도(높으면 높을 수록 빨리 회전)
    float xRoatation = 0f; //카메라의 상하 회전 누적값

    [Header("카메라 참조 객체")]
    public Transform playerCamera; //플레이어의 1인칭 카메라 Trasnform
    void Start()
    {
        //마우스 커서를 화면 중앙에 고정
        //마우스가 게임 화면 밖으로 나가지 못하게 막는 기능
        Cursor.lockState = CursorLockMode.Locked;

        //마우스 커서를 화면에 보이지 않게 숨겨라
        //false : 보이지 않음
        //true  : 보이는 것
        Cursor.visible = false;
    }
    
    void Update()
    {
        Look();
    }

    //플레이어 및 카메라 회전 함수
    void Look()
    {
        //마우스 입력 받기
        //MouseX : 마우스 좌우 이동 -> 좌우 회전
        float mouseX = Input.GetAxisRaw("Mouse X") * mouseSen * Time.deltaTime;

        //MouseY : 마우스 상 하 이동 -> 위 아래 시선 회전
        float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSen * Time.deltaTime;

        //2. 플레이어(몸통) 좌우 회전
        //플레이어를 y축 기준으로 mouseX만큼 회전시켜라
        //Vector3.up (0,1,0) y축
        transform.Rotate(Vector3.up * mouseX);

        //3. 카메라 상하 회전
        // xRotation 값을마우스 y 입력에 따라 누적
        // xRotation : 카메라가 "위/아래로 얼마나 돌아갔는가?"를 나타내는 구조
        // xRotation = 0; : 정면
        // xRotation감소  : 아래쪽 보기
        // xRotation증가  : 위쪽 보기
        xRoatation -= mouseY;

        //4. 카메라 회전 제한(고개가 너무 위/아래로 꺾이지 않게)
        //시야가 360도로 뒤집히지 않게 제한
        //Mathf.Clamp(값, 최솟값, 최대값)
        //숫자 값이 최솟값(min)과 최대값(max)사이를 넘지 못하게 제한하는 함수
        xRoatation = Mathf.Clamp(xRoatation, -80f, 80f);
        //Mathf.Clamp(120, 0f, 100f) X
        //Mathf.Clamp(50, 0f, 100) O

        //5. 카메라 로컬 회전 적용
        //Quaternion : 쿼터니언
        //Euler : 오일러(수학자 이름)
        //오일러 각(x,y,z 회전값)을 이용해서 회전을 만들어라
        //오브젝트를 x,y,z축으로 돌려라
        playerCamera.localRotation = Quaternion.Euler(xRoatation, 0f, 0f);
    }
}