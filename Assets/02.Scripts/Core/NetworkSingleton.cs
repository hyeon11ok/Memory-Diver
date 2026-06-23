using UnityEngine;
using Mirror; // Mirror 추가

// 자동 생성 로직이 빠진, 오직 캐싱만 하는 안전한 네트워크 싱글톤
public class NetworkSingleton<T>:NetworkBehaviour where T : NetworkSingleton<T>
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if(instance == null)
            {
                Debug.LogError($"[NetworkSingleton] {typeof(T)} 가 씬에 존재하지 않습니다! 서버에서 Spawn 했는지 확인하세요.");
            }
            return instance;
        }
    }

    protected virtual void Awake()
    {
        if(instance == null)
        {
            instance = this as T;

            // 주의: 네트워크 객체는 보통 NetworkManager가 DontDestroyOnLoad를 관리하거나, 
            // 씬이 넘어갈 때 서버가 다시 Spawn 해주는 방식을 씁니다.
            // 필요하다면 여기에 DontDestroyOnLoad(gameObject); 를 넣을 수 있지만, 신중해야 합니다.
        }
        else if(instance != this as T)
        {
            Debug.LogWarning($"[NetworkSingleton] {typeof(T)} 중복 발견! 파괴합니다.");
            Destroy(gameObject);
        }
    }
}