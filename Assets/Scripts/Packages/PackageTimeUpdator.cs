using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PackageTimeUpdator : MonoBehaviour
{
    public Package package { get; set; }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(package != null)
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
    }
}
