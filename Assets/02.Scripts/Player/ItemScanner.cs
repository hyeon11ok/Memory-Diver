using System.Collections.Generic;
using UnityEngine;

public class ItemScanner : MonoBehaviour
{
    [SerializeField] private float scanRadius = 10f;
    [SerializeField] private float scanTime = 0.5f;
    [SerializeField] private LayerMask itemLayerMask;

    private float curRadius = -0.5f;
    private Vector3 scanCenter = new Vector3();
    private bool isScanning = false;

    // 스캔된 아이템 ID를 저장하는 리스트, 스캔 중 중복 스캔을 방지
    private HashSet<int> scannedItemIDs = new HashSet<int>();

    private readonly int scanRadiusID = Shader.PropertyToID("_ScanRadius");
    private readonly int scanCenterID = Shader.PropertyToID("_ScanCenter");

    public void Init()
    {
        Shader.SetGlobalFloat(scanRadiusID, curRadius);
    }

    // Update is called once per frame
    void Update()
    {
        if(isScanning)
        {
            curRadius += (scanRadius / scanTime) * Time.deltaTime;
            Shader.SetGlobalFloat(scanRadiusID, curRadius);

            Collider[] items = Physics.OverlapSphere(scanCenter, curRadius, itemLayerMask);
            foreach(Collider item in items)
            {
                int instanceID = item.GetInstanceID();

                if(scannedItemIDs.Contains(instanceID))
                    continue; // 이미 스캔된 아이템이면 건너뜀

                if(item.TryGetComponent<Item>(out Item itemComponent))
                {
                    itemComponent.ScanReflectEffect();
                    scannedItemIDs.Add(instanceID); // 스캔된 아이템 ID 저장
                }
            }

            if(curRadius >= scanRadius)
            {
                isScanning = false;
                scannedItemIDs.Clear(); // 스캔 완료 후 리스트 초기화
                ResetScanner();
            }
        }
    }

    void ResetScanner()
    {
        curRadius = -0.5f;
        scanCenter = transform.position;
        Shader.SetGlobalFloat(scanRadiusID, curRadius);
        Shader.SetGlobalVector(scanCenterID, transform.position);
    }

    public void StartScan()
    {
        isScanning = true;
        ResetScanner();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(scanCenter, curRadius);
    }
}
