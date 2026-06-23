using UnityEngine;
using Steamworks; // 스팀 기능을 쓰기 위한 네임스페이스

[DisallowMultipleComponent]
public class SteamManager:Singleton<SteamManager>
{
    private static SteamManager instance;

    protected override void Awake()
    {
        base.Awake();

        // 스팀 초기화 시도
        try
        {
            if(SteamAPI.RestartAppIfNecessary((AppId_t)480))
            {
                Application.Quit();
                return;
            }
        }
        catch(System.DllNotFoundException e)
        {
            Debug.LogError("[Steamworks.NET] 스팀 플러그인을 로드할 수 없습니다. " + e);
            return;
        }

        // 스팀 기능 켜기! (가장 중요한 부분)
        if(!SteamAPI.Init())
        {
            Debug.LogError("[Steamworks.NET] 스팀 초기화 실패! 스팀 클라이언트가 켜져 있는지 확인하세요.");
            return;
        }

        Debug.Log("[Steamworks.NET] 스팀 초기화 완벽하게 성공!");
    }

    private void Update()
    {
        // 매 프레임마다 스팀 서버와 데이터를 주고받도록 콜백 실행
        SteamAPI.RunCallbacks();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        if(instance == this)
        {
            // 게임이 꺼질 때 스팀 기능도 안전하게 종료
            SteamAPI.Shutdown();
        }
    }
}