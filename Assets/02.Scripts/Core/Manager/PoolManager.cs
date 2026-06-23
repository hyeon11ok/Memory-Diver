using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Mirror;

public class PoolManager : Singleton<PoolManager>
{
    private Dictionary<int, object> pools = new Dictionary<int, object>();

    public IObjectPool<T> GetOrCreatePool<T>(T prefab, int defaultCapacity = 20, int maxSize = 100) where T : Component
    {
        int key = prefab.gameObject.GetInstanceID();

        // 이미 만들어진 풀이 있다면 그대로 반환
        if(pools.TryGetValue(key, out object pool))
        {
            return (IObjectPool<T>)pool;
        }

        // 없다면 새로 생성
        IObjectPool<T> newPool = null; // 람다식 내부에서 캡처하기 위해 미리 선언

        newPool = new ObjectPool<T>(
            createFunc: () =>
            {
                T obj = Instantiate(prefab);
                // 생성된 객체가 IPoolable 인터페이스를 가지고 있다면 풀 참조를 전달
                if(obj is IPoolable<T> poolable)
                {
                    poolable.SetPool(newPool);
                }
                return obj;
            },
            actionOnGet: (obj) => obj.gameObject.SetActive(true),
            actionOnRelease: (obj) => obj.gameObject.SetActive(false),
            actionOnDestroy: (obj) => Destroy(obj.gameObject),
            collectionCheck: false, // 릴리즈 빌드 최적화를 위해 false 추천
            defaultCapacity: defaultCapacity,
            maxSize: maxSize
        );

        pools.Add(key, newPool);
        return newPool;
    }

    /// <summary>
    /// NetworkIdentity가 붙은 프리팹을 Mirror의 Spawn 시스템과 PoolManager에 연동합니다.
    /// 게임 시작 시(NetworkManager의 Start 등) 미리 호출해두어야 합니다.
    /// </summary>
    public void RegisterNetworkPool<T>(T prefab, int defaultCapacity = 20, int maxSize = 100) where T : NetworkBehaviour
    {
        // 1. 일반 풀 생성
        IObjectPool<T> pool = GetOrCreatePool(prefab, defaultCapacity, maxSize);

        // 2. 미러의 Spawn을 가로채서 풀에서 꺼내주도록 설정 (Handler 등록)
        NetworkClient.RegisterPrefab(prefab.gameObject,
            spawnHandler: (SpawnMessage msg) =>
            {
                // 서버가 Spawn하라고 명령하면 새로 생성하지 않고 풀에서 꺼냄
                T obj = pool.Get();
                return obj.gameObject;
            },
            unspawnHandler: (GameObject spawned) =>
            {
                // 서버가 UnSpawn하라고 명령하면 파괴하지 않고 풀로 돌려보냄
                T obj = spawned.GetComponent<T>();
                if(obj != null)
                {
                    pool.Release(obj);
                }
            }
        );
    }
}
