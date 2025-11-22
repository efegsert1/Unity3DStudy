using UnityEngine;
using UnityEngine.UI; //UI를 사용할 때 선언 

public class PlayerHealth : MonoBehaviour
{
    [Header("플레이어 체력 설정")]
    public int maxHealth = 100; //최대 체력
    private int currentHealth;

    [Header("UI 연결 (이미지 사용)")]
    public Image healthFillImage;

    [Header("피격효과")]
    //렌더러 : 게임 속 오브젝트를 화면에 보여지게 만들어주는 역할이다.
    //모양(mesh), 색깔(material)로 구성되어 있다
    //모양(mesh) : 물체의 뼈대(모양)
    //색깔 : 색상, 질감
    public Renderer playerRenderer; //피격 시 색깔 효과를 줄 렌더러
    private Color originalColor; //원래 색상 저장 변수
    public float flashDuration = 0.2f; //깜빡이는 시간


    void Start()
    {
        //처음 시작할 때 체력을 가득 체운다.
        currentHealth = maxHealth;

        //UI 이미지가 연결되어 있다면,시작 시 전체 채우기
        //예외 처리 : 프로그램이 '죽는'것을 막기 위해서
        //에러가 처리되지 않으면, 유니티 에디터가 멈추거나 게임이 강제종료될 수있다.
        //이를 막기 위해서 예외처리를 한다.
        if (healthFillImage != null)
        {
            //인스펙터 창에 있는 fillAmount를 스크립트에서 조절하기 위해서
            healthFillImage.fillAmount = 1f;   // 1 = 100%
        }

        //예외 처리
        //플레이어 색 저장(피격 시 빨갛게 만들기 위해)
        if (playerRenderer != null)
        {
            //originalColor = 원래 플레이어의 색상을 넣어줌
            originalColor = playerRenderer.material.color;  
        }

    }

    void Update()
    {
        
    }

    //플레이어의 체력이 깍인다.
    //접근제한자 : 프로그래밍에서 클래스, 변수, 메서드(함수)
    //같은 코드 요소들이 "어디까지 공개될건지"를 정해주는 키워드다

    // public : 완전 공개 : 어디서든 접근 가능(다른클래스 가능)
    //private : 완전 비공개 : 오직 지금 있는 클래스에서만 접근 가능하다.
    public void TakeDamage(int damage)
    {
        //전달받은 damage 값 만큼 현재 체력을 줄인다.
        currentHealth -= damage;

        //현재 체력이 0이하가 되었는지 확인다.
        if(currentHealth <= 0)
        {
            //체력이 음수가 되는것을 방지하기 위해 0으로 고정
            currentHealth = 0;

            Die(); //사망 처리 함수를 호출한다.

        }

        //체력 비율을 계산한다. 현재체력 / 최대 체력
        //예시 : 75 / 100  = 0.75
        float healthPercent = (float) currentHealth /maxHealth;

        //체력바가 UI에 연결되어 있다면, 계산한 비율을 체력바에 반영한다.
        //(3분)_17분
        if (healthFillImage != null)
        {
            healthFillImage.fillAmount = healthPercent;
        }

        //혹시라도 비율 계산 결과가 0이하일 경우, 사망 처리를 한 번 더 안전하게 수행
        if(healthPercent <= 0)
        { 
            Die();
           

        }

        Debug.Log("플레이어가 데미지를 받음! 현재 체력 : " + currentHealth);


    }

    //사망 처리 함수
    private void Die()
    {
        // 콘솔 "플레이어 사망!" 메시지 ㅣ출력
        Debug.Log("플레이어 사망!");

        //게임오브젝트를 비활성화하여 화면에서 사라지게 한다.
        gameObject.SetActive(false);
    }

   
}
