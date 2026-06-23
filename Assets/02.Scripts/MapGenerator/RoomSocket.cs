using UnityEngine;
using Mirror;

public class RoomSocket :NetworkBehaviour
{
    // 이 소켓이 속한 방
    [SerializeField] private Room parentRoom;
    // 연결 여부
    // 서버에서 이 값이 바뀌면 클라이언트들에게 신호를 보내 'OnConnectionChanged' 함수를 실행함
    [SyncVar(hook = nameof(OnConnectionChanged))]
    private bool isConnected = false;
    // 연결이 안된 소켓을 막기 위해 생성되는 막힌 문(벽) 오브젝트
    // 기본으로 활성화 상태, 연결되면 비활성화
    [SerializeField] private GameObject closedDoorObject;

    public Room ParentRoom { get => parentRoom;  }
    public bool IsConnected { get => isConnected; }

    public void Init(Room parentRoom)
    {
        this.parentRoom = parentRoom;
        if(closedDoorObject != null) closedDoorObject.SetActive(true);
    }

    [Server]
    public void ConnectSocket()
    {
        // SyncVar 값을 변경하면 자동으로 클라이언트들에게 동기화
        isConnected = true;
    }

    // isConnected 값이 변경될 때마다 서버/클라이언트 모두에서 자동으로 실행되는 Hook 함수
    private void OnConnectionChanged(bool oldVal, bool newVal)
    {
        if(newVal == true && closedDoorObject != null)
        {
            closedDoorObject.SetActive(false); // 모든 유저 화면에서 벽이 비활성화
        }
    }

    // 늦게 접속한 사람(Late Joiner)을 위한 방어막
    // 맵 생성이 다 끝난 뒤에 친구가 방에 들어와도 이미 열려있는 문은 확실하게 열어줌
    public override void OnStartClient()
    {
        if(isConnected && closedDoorObject != null)
        {
            closedDoorObject.SetActive(false);
        }
    }

    private void OnDrawGizmos()
    {
        // 에디터에서 소켓의 방향을 시각적으로 확인하기 위함
        Gizmos.color = isConnected ? Color.red : Color.green;
        Gizmos.DrawRay(transform.position, transform.forward * 2f);
        Gizmos.DrawWireSphere(transform.position, 0.2f);
    }
}
