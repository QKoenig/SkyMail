using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    public TextMeshProUGUI text1;
    public TextMeshProUGUI text2;

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
        // update time
        if (package != null)
        {
            if (package.StartTime > 0)
            {
                package.TimeRemaining = package.TotalTime - (Time.time - package.StartTime);
                if (package.TimeRemaining <= 0)
                {
                    package.Mode = Package.PackageMode.Expired;
                }
            }
        }


        int minutes = ((int)(package.TimeRemaining / 60));
        int seconds = ((int)(package.TimeRemaining % 60));
        int hundreths = (int)((package.TimeRemaining - Mathf.Floor(package.TimeRemaining)) * 100f);
        text1.text = text2.text = package.TimeRemaining > 0 ? string.Format("<mspace=mspace=.013>{0,2}</mspace>:<mspace=mspace=.013>{1,2}</mspace>", (minutes > 0 ? minutes : seconds).ToString("D2"), (minutes > 0 ? seconds : hundreths).ToString("D2")) :
            (int)(Time.time * 2f) % 2 == 0 ? "EXPRD" : "";

        if (!isPickedUp && package.Mode == Package.PackageMode.Available && holder.CanAcceptPackage() && playerControl != null && Vector3.Distance(transform.position, playerControl.transform.position) < package.PickupRadius)
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
    }
}
