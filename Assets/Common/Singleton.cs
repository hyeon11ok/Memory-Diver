using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    #region [Inspector]

    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindAnyObjectByType(typeof(T)) as T;
                if (instance == null)
                {
                    SetupInstance();
                }

                DontDestroyOnLoad(instance.gameObject);
            }

            return instance;
        }
    }

    #endregion


    #region [LifeCycle]

    private void Awake()
    {
        RemoveDuplicates();
    }

    #endregion


    #region [Methods]

    void RemoveDuplicates()
    {
        if (instance != null && instance != this as T)
            Destroy(gameObject);
    }

    static void SetupInstance()
    {
        GameObject gameObj = new GameObject();
        gameObj.name = typeof(T).Name;
        instance = gameObj.AddComponent<T>();
    }

    #endregion

}
