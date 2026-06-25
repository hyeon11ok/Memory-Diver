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
                    UIManager.Instance.ShowUI<InteractUI>()?.SetInteractText(curInteractable.GetInteractPrompt());
                }
            }
            else
            {
                if(curInteractGameObject != null)
                {
                    curInteractGameObject = null;
                    curInteractable = null;
                    UIManager.Instance.CloseUI<InteractUI>();
                }
            }
        }

        if(player.InputHandler.IsInteract)
        {
            Interact();
        }
    }

    public void Interact()
    {
        if(!isLocalPlayer) return;

        // 클라이언트가 상호작용 키를 누르면, 로컬에서 실행하지 않고 서버로 요청함!
        if(curInteractable != null && curInteractGameObject != null)
        {
            CmdInteract(curInteractGameObject);
        }
    }

    // 서버에서 실행되는 함수 (접두어 Cmd 필수)
    [Command]
    private void CmdInteract(GameObject targetObj)
    {
        // 서버 측에서 해당 오브젝트가 상호작용 가능한지 한 번 더 검증 (보안)
        if(targetObj != null && targetObj.TryGetComponent<IInteractable>(out IInteractable interactable))
        {
            interactable.OnInteract(player);
            // 주의: IInteractable 인터페이스의 OnInteract 로직 내부에서 
            // 오브젝트 삭제(Destroy)나 상태 변경이 일어난다면, 
            // 반드시 NetworkServer.Destroy() 나 [ClientRpc]를 사용하도록 해당 인터페이스 구현부도 수정해야 함!
        }
    }
}
