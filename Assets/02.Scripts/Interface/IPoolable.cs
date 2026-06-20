using UnityEngine;
using UnityEngine.Pool;

// 어떤 컴포넌트든 이 인터페이스를 상속받으면 풀링이 가능해집니다.
public interface IPoolable<T> where T : Component
{
    void SetPool(IObjectPool<T> pool);
}
