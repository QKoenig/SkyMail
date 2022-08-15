using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PackageAcceptor : MonoBehaviour
{
    public float acceptanceRadius = 10;
    public CompassMarker marker;
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, acceptanceRadius);
    }


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetMarkerEnabled(bool val)
    {
        marker.enabled = val;
    }

    public void AcceptPackage(Package p)
    {
        p.Mode = Package.PackageMode.Delivered;
    }
}
