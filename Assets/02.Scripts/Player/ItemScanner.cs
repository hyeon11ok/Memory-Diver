using UnityEngine;

public class ItemScanner : MonoBehaviour
{
    [SerializeField] private float scanRadius = 10f;
    [SerializeField] private float scanTime = 0.5f;
    [SerializeField] private LayerMask itemLayerMask;

    private float curRadius = -0.5f;
    private Vector3 scanCenter = new Vector3();
    private bool isScanning = false;

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
                // 아이템과의 상호작용 로직을 여기에 추가
                // 예: 아이템 하이라이트, 수집 등
            }

            if(curRadius >= scanRadius)
            {
                isScanning = false;
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
}
