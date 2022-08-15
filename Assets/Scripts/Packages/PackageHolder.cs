using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PackageHolder : MonoBehaviour
{
    public Transform cam;
    public Vector3 boxStartOffset;
    public Vector3 selectorStartOffset;
    public float distanceBetweenBoxes = .5f;
    public Rigidbody playerRB;
    public float speedEffect = .1f;
    public WorldPackage packageObject;
    public GameObject packageSelector;
    public int maxPackages = 6;
    public float maxVelocityForWobble = 20f;

    private List<WorldPackage> packageObjects = new List<WorldPackage> ();
    private int selected = 0;
    

    // Update is called once per frame
    void Update()
    {
        if(packageObjects.Count > 0 && selected > packageObjects.Count-1)
        {
            selected = packageObjects.Count-1;
        }
        selected = Mathf.Max(0, selected);

        Vector3 selectorVel = new Vector3();
        Vector3 vel = transform.InverseTransformDirection(playerRB.velocity);
        Vector3 finalVel = vel.magnitude > maxVelocityForWobble ? vel.normalized * maxVelocityForWobble : (vel.magnitude < .1f ? new Vector3(0, 0, 0) : vel);
        //packageSelector.transform.localPosition = Vector3.SmoothDamp(packageSelector.transform.localPosition, selectorStartOffset + new Vector3(packages.Count == 0 ? -.5f : 0, distanceBetweenBoxes * selected, 0) + - (transform.InverseTransformDirection(playerRB.velocity).normalized) * (((float)selected + 1f) / (maxPackages + 1)) * speedEffect, ref selectorVel, .01f);
        packageSelector.transform.localPosition = packageObjects.Count != 0 ? Vector3.SmoothDamp(packageSelector.transform.localPosition, packageObjects[selected].transform.localPosition + selectorStartOffset, ref selectorVel, .01f) :
            Vector3.SmoothDamp(packageSelector.transform.localPosition, boxStartOffset + selectorStartOffset + new Vector3(packageObjects.Count == 0 ? -.5f : 0, distanceBetweenBoxes * selected, 0) + -(finalVel) * (((float)selected + 1f) / (maxPackages + 1)) * speedEffect, ref selectorVel, .03f);
        for (int i = 0; i < packageObjects.Count; i++)
        {
            WorldPackage pO = packageObjects[i];
            Vector3 boxVel = new Vector3();
            pO.transform.localPosition = Vector3.SmoothDamp(pO.transform.localPosition, boxStartOffset + new Vector3(0, distanceBetweenBoxes * i, 0) + -(finalVel)*(((float)i+1f)/(maxPackages+1))*speedEffect, ref boxVel, .03f);
        }
    }



    public void LaunchCurrentPackage(Vector3 vel)
    {
        // Get selected package
        if(packageObjects.Count < 1)
        {
            return;
        }
        WorldPackage pO = packageObjects[selected];
        if(pO != null)
        {
            pO.Launch(vel);
            RemovePackage(selected);
        }
    }

    public void RemovePackage(int index)
    {
        packageObjects.Remove(packageObjects[index]);
    }
    public void RemovePackage(Package p, WorldPackage packageObject)
    {
        packageObjects.Remove(packageObject);
    }

    public void IncreaseSelection()
    {
        if(packageObjects.Count == 0 || selected == packageObjects.Count-1)
        {   
            return;
        }
        selected ++;
    }

    public void DecreaseSelection()
    {
        if (packageObjects.Count == 0 || selected == 0)
        {
            return;
        }
        selected --;
    }

    public void AddPackage(Package p)
    {
        WorldPackage pO = Instantiate(packageObject, cam);
        pO.SetPackage(p);
        pO.transform.localPosition = boxStartOffset + new Vector3(0, 1f, 0) + new Vector3(0, distanceBetweenBoxes * (packageObjects.Count - 1), 0);
        packageObjects.Add(pO);
    }

    public void ReturnPackage(WorldPackage pO)
    {
        pO.transform.SetParent(cam);
        pO.transform.localPosition = boxStartOffset + new Vector3(0, 1f, 0) + new Vector3(0, distanceBetweenBoxes * (packageObjects.Count - 1), 0);
        packageObjects.Add(pO);
    }

    public bool CanAcceptPackage()
    {
        return packageObjects.Count < maxPackages;
    }

    public void ClearPackages()
    {
        for(int i = packageObjects.Count - 1; i >= 0; i--)
        {
            Destroy(packageObjects[i].gameObject);
            RemovePackage(i);
        }
    }
}
