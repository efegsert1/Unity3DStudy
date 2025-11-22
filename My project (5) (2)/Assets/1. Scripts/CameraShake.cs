using NUnit.Framework.Constraints;
using System.Collections; //코루틴 사용을 위해 필요한 네임스페이스
using System.Collections.Generic; //제네릭 컬렉션용 네임스페이스
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private Vector3 originalPos; //흔들기 시작 전 카메라의 원래 로컬 위치를 저장하는 변수
    private bool isShaking = false; // 현재 흔들림 코루틴이 실행중인지 여부를 나타내는 것


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //시작 시점의 로컬 위치를 저장하여, 흔들림 후 복구할 때 사용
        originalPos = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //코루틴 : 잠시 멈췄다가 다음 프레임에 다시 실행되는 함수 
    //실행중인 함수를 일시적으로 중단하고, 이후 한 프레임 뒤나 일정시간 뒤에 
    //다시 이어서 실행할 수 있게하는 함수다.

    //IEnumerator : 코루틴용 함수의 타입 이름
    //duration : 지속 시간
    //magnitdue : 강도

    public IEnumerator RecoilShake(float duration, float magnitdue)
    {
        //이미 실행중이라면 중복 실행 방지를 즉시 종료
        //yield break : 코루틴을 중간에 강제로 끝내는 명령어
        if (isShaking) yield break;

        isShaking = true; // 흔들림 시작 상태로 설정

        //경과 시간을 누적할 변수 초기화
        float timespeed = 0f;


        //지정한 지속 시간 동안 루프 반복
        while (timespeed < duration)
        { 
            //항상 원래자리 기준으로 오프셋을 더해 흔들림을 적용
            float x = Random.Range(-0.1f, 0.1f) * magnitdue; //x축 방향 무작위 오프셋
            float y = Random.Range(-0.1f, 0.1f) * magnitdue; //y축 방향
        
            //원래 위치에 오프셋을 더해 현재 프레임의 흔들림 위치 적용
            transform.localPosition = originalPos + new Vector3(x, y, 0);

            //한 프레임이 지날 때 마다 시간을 더해요
            timespeed += Time.deltaTime;

            //다음 프레임까지 잠깐 기다려요.
            yield return null;
            
        }

        //흔들림이 끝나면 카메라를 원래 자리로 돌려놔요
        transform.localPosition = originalPos;
        //이제 흔들림이 끝났어요
        isShaking = false;



    }
}
