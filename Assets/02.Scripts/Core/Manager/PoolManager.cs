using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

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
}
