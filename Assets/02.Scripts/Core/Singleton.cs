using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T instance;
    private static bool isQuitting = false; // 앱 종료 상태를 추적하기 위한 플래그
    private static readonly object _lock = new object(); // 스레드 안전을 위한 락 객체

    public static T Instance
    {
        get
        {
            // 1. 앱 종료 중이라면 객체를 생성하지 않고 null 반환
            if(isQuitting)
            {
                Debug.LogWarning($"[Singleton] Instance '{typeof(T)}' is already destroyed. Returning null.");
                return null;
            }

            lock(_lock)
            {
                if(instance == null)
                {
                    // 2. 씬에 있는지 먼저 확인
                    instance = FindFirstObjectByType<T>(); 

                    if(instance == null)
                    {
                        // 3. 씬에 없다면 새로 생성
                        GameObject singletonObject = new GameObject();
                        instance = singletonObject.AddComponent<T>();
                        singletonObject.name = typeof(T).ToString() + " (Singleton)";
                    }

                    // 4. 최상위 객체로 빼낸 뒤 DontDestroyOnLoad 적용
                    if(instance.gameObject.transform.parent != null)
                    {
                        instance.gameObject.transform.SetParent(null);
                    }
                    DontDestroyOnLoad(instance.gameObject);
                }
                return instance;
            }
        }
    }

    protected virtual void Awake()
    {
        if(instance == null)
        {
            // 5. 씬에 미리 배치해둔 경우 무거운 Find 연산 방지
            instance = this as T;
            if(transform.parent != null) transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        }
        else if(instance != this as T)
        {
            // 중복 객체 파괴
            Destroy(gameObject);
        }
    }

    protected virtual void OnApplicationQuit()
    {
        // 6. 앱 종료 시 플래그 설정
        isQuitting = true;
    }

    protected virtual void OnDestroy()
    {
        if(instance == this)
        {
            isQuitting = true;
        }
    }

}
