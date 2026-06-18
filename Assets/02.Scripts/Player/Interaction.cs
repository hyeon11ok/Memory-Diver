using TMPro;
using UnityEngine;

public interface IInteractable
{
    public string GetInteractPrompt();
    public void OnInteract(Player player);
}


public class Interaction : MonoBehaviour
{
    private Player player;
    private Camera _camera;

    // 레이 캐스트 관련 변수
    [SerializeField] private float checkRate = 0.05f;
    private float lastCheckTime;
    [SerializeField] private float maxCheckDistance;
    [SerializeField] private LayerMask layerMask;

    // 현재 상호작용 가능한 게임 오브젝트와 인터랙터블 인터페이스 참조
    private GameObject curInteractGameObject;
    private IInteractable curInteractable;

    public void Init(Player player)
    {
        this.player = player;
        _camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time - lastCheckTime > checkRate)
        {
            lastCheckTime = Time.time;

            Ray ray = _camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
            RaycastHit hit; //레이에 맞은 물체를 저장하는 변수

            if(Physics.Raycast(ray, out hit, maxCheckDistance, layerMask))
            {
                if(hit.collider.gameObject != curInteractGameObject)
                {
                    curInteractGameObject = hit.collider.gameObject;
                    curInteractable = hit.collider.GetComponent<IInteractable>();
                }
            }
            else
            {
                if(curInteractGameObject != null)
                {
                    curInteractGameObject = null;
                    curInteractable = null;
                }
            }
        }
    }

    public void Interact()
    {
        if(curInteractable != null)
        {
            if(curInteractable is Object unityObject && unityObject != null)
            {
                curInteractable.OnInteract(player);
            }
        }
    }
}
