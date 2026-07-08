using Mirror;
using UnityEngine;

public class Interaction :NetworkBehaviour
{
    private Player player;

    // 레이 캐스트 관련 변수
    [SerializeField] private float checkRate = 0.05f;
    private float lastCheckTime;
    [SerializeField] private float maxCheckDistance;

    // 현재 상호작용 가능한 게임 오브젝트와 인터랙터블 인터페이스 참조
    private GameObject curInteractGameObject;
    private IInteractable curInteractable;

    public void Init(Player player)
    {
        this.player = player;
    }

    // Update is called once per frame
    void Update()
    {
        if(!isLocalPlayer) return; // 레이캐스트 낭비 방지 & 로컬 UI만 띄우기 위함

        if(Time.time - lastCheckTime > checkRate)
        {
            lastCheckTime = Time.time;

            Ray ray = player.PlayerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
            RaycastHit hit; //레이에 맞은 물체를 저장하는 변수

            if(Physics.Raycast(ray, out hit, maxCheckDistance) &&
                hit.transform.TryGetComponent<IInteractable>(out IInteractable interactable))
            {
                if(hit.collider.gameObject != curInteractGameObject)
                {
                    curInteractGameObject = hit.collider.gameObject;
                    curInteractable = interactable;
                    UIManager.Instance?.ShowUI<InteractUI>()?.SetInteractText(curInteractable.GetInteractPrompt());
                }
            }
            else
            {
                if(curInteractGameObject != null)
                {
                    InteractCancel();
                    curInteractGameObject = null;
                    curInteractable = null;
                    UIManager.Instance?.CloseUI<InteractUI>();
                }
            }
        }
    }

    // InputHandler에서 호출됨
    public void InteractStart()
    {
        if(!isLocalPlayer) return;
        if(curInteractable != null && curInteractGameObject != null)
            CmdInteractStart(curInteractGameObject); // 서버로 상호작용 '시작'을 알림
    }

    public void InteractCancel()
    {
        if(!isLocalPlayer) return;
        if(curInteractable != null && curInteractGameObject != null)
            CmdInteractCancel(curInteractGameObject); // 서버로 상호작용 '중단'을 알림
    }

    [Command]
    private void CmdInteractStart(GameObject targetObj)
    {
        if(targetObj != null && targetObj.TryGetComponent<IInteractable>(out IInteractable interactable))
            interactable.OnInteractStart(player);
    }

    [Command]
    private void CmdInteractCancel(GameObject targetObj)
    {
        if(targetObj != null && targetObj.TryGetComponent<IInteractable>(out IInteractable interactable))
            interactable.OnInteractCancel(player);
    }
}
