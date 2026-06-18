using UnityEngine;

public abstract class BaseUI : MonoBehaviour
{
    public virtual void Initialize() 
    { 
        // Awake나 Start 대신 매니저가 프리팹을 생성할 때 한 번만 호출
    }
    
    public virtual void OnOpen() 
    { 
        gameObject.SetActive(true); 
    }
    
    public virtual void OnClose() 
    { 
        gameObject.SetActive(false); 
    }
}






