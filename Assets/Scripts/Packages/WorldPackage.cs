using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WorldPackage : MonoBehaviour
{
    public float smallScale = .1f;
    public float largeScale = .3f;

    public TextMeshProUGUI text1;
    public TextMeshProUGUI text2;

    private Package package;

    private PackageHolder packageHolder;

    private Quaternion startRotation;

    private float launchTime = 0;
    private bool inHand = true;

    // Start is called before the first frame update
    void Start()
    {
        startRotation = transform.localRotation;
        packageHolder = FindObjectOfType<PackageHolder>();
    }

    // Update is called once per frame
    void Update()
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


        Vector3 vel = new Vector3(0, 0, 0);
        transform.localScale = Vector3.SmoothDamp(transform.localScale, (inHand ? Vector3.one * smallScale : Vector3.one * largeScale), ref vel, .02f);

        if(Vector3.Distance(transform.position, package.Destination.transform.position) < package.Destination.acceptanceRadius)
        {
            package.Destination.AcceptPackage(package);
            packageHolder.RemovePackage(package, this);
            Destroy(gameObject);
        }

        int minutes = ((int)(package.TimeRemaining / 60));
        int seconds = ((int)(package.TimeRemaining % 60));
        int hundreths = (int)((package.TimeRemaining - Mathf.Floor(package.TimeRemaining)) * 100f);
        text1.text = text2.text = package.TimeRemaining > 0 ? string.Format("<mspace=mspace=.013>{0,2}</mspace>:<mspace=mspace=.013>{1,2}</mspace>", (minutes > 0 ? minutes : seconds).ToString("D2"), (minutes > 0 ? seconds : hundreths).ToString("D2")) :
            (int)(Time.time*2f) % 2 == 0 ? "EXPRD" : "";
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player" && packageHolder.CanAcceptPackage())
        {
            // Don't pick it up if its less than half a second since you threw it.
            if(Time.time - launchTime < .5f)
            {
                return;
            }
            // give package a rigid body and launch
            Destroy(GetComponent<Rigidbody>());
            Destroy(GetComponent<BoxCollider>());
            // Remove package and tell it it's launched
            package.Mode = Package.PackageMode.Transit;
            packageHolder.ReturnPackage(this);
            inHand = true;
            transform.localRotation = startRotation;
        }
    }

    public void Launch(Vector3 vel)
    {
        transform.SetParent(null, true);
        // give package a rigid body and launch
        Rigidbody rb = gameObject.AddComponent<Rigidbody>();
        gameObject.AddComponent<BoxCollider>();
        rb.velocity = vel;
        // Remove package and tell it it's launched
        package.Mode = Package.PackageMode.Thrown;
        launchTime = Time.time;
        inHand = false;
    }

    public void SetPackage(Package p)
    {
        package = p;
    }
}
