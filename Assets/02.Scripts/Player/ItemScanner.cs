using UnityEngine;

public class ItemScanner : MonoBehaviour
{
    [SerializeField] private float scanRadius = 10f;
    [SerializeField] private float scanTime = 0.5f;
    private float curRadius = 0;
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
            if(curRadius >= scanRadius)
            {
                isScanning = false;
                ResetScanner();
            }
        }
    }

    void ResetScanner()
    {
        curRadius = 0;
        Shader.SetGlobalFloat(scanRadiusID, curRadius);
        Shader.SetGlobalVector(scanCenterID, transform.position);
    }

    public void StartScan()
    {
        isScanning = true;
        ResetScanner();
    }
}
