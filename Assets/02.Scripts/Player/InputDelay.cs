using System;
using UnityEngine;

[Serializable]
public class InputDelay
{
    [SerializeField] private float delayTime; // 버퍼 시간
    private float timer; // 타이머
    public InputDelay(float delayTime)
    {
        this.delayTime = delayTime;
        this.timer = 0;
    }
    public void Activate() // 입력 버퍼 활성화
    {
        timer = delayTime;
    }
    public void Update() // 타이머 업데이트
    {
        if(timer > 0f)
        {
            timer -= Time.deltaTime;
        }
    }
    /// <summary>
    /// 입력 버퍼가 활성화되어 있는지 확인하는 메서드입니다.
    /// True == 버퍼 활성화, False == 버퍼 비활성화
    /// </summary>
    /// <returns></returns>
    public bool IsActive() // 입력 버퍼 활성 여부 확인
    {
        return timer > 0f;
    }
}
