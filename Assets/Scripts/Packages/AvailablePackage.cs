using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvailablePackage : MonoBehaviour
{

    public float rotationSpeed = 1f;
    public float spinSpeed = 20f;
    public float rotationAmount = 20f;

    private Transform holdPoint;
    private Package package;
    private PlayerControl playerControl;
    private PackageHolder holder;

    private bool isPickedUp = false;

    private void Awake()
    {
        playerControl = FindObjectOfType<PlayerControl>();
        holder = playerControl.GetComponent<PackageHolder>();
        if(playerControl == null)
        {
            Debug.LogWarning("No player found, packaage cannot be accepted.");
        }
    }

    private void Update()
    {
        if(!isPickedUp && package.Mode == Package.PackageMode.Available && holder.CanAcceptPackage() && playerControl != null && Vector3.Distance(transform.position, playerControl.transform.position) < package.PickupRadius)
        {
            isPickedUp = true;
            package.Mode = Package.PackageMode.Transit;
            holder.AddPackage(package);
            Destroy(gameObject);
        }
        transform.rotation = Quaternion.Euler(Mathf.Cos(Time.time * rotationSpeed) * rotationAmount, (Time.time * spinSpeed) % 360, Mathf.Sin(Time.time * rotationSpeed) * rotationAmount);
    }

    public void SetHoldPoint(Transform newHP)
    {
        holdPoint = newHP;
        transform.position = holdPoint.position;
    }

    public void SetPackage(Package p)
    {
        package = p;
        GetComponent<PackageTimeUpdator>().package = p;
    }
}
