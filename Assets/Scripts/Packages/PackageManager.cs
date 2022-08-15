using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PackageManager : MonoBehaviour
{
    public static PackageManager Instance { get; private set; }
    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            InvokeRepeating("AttemptPackageSpawn", 1f, 1.0f);
        }
    }

    public List<PackageCreator> creatorList = new List<PackageCreator>();


    public int MaxAvailablePackages = 20;

    [Tooltip("Rate at which packages spawn based on how many packages are available / MaxAvailablePackages measured in packages/minute (Should evaluate 0 at x = 1 to enforce max available packages)")]
    public AnimationCurve packageSpawnRate;

    public float spawnRateModifier = 1f;

    public int maxSpawnAttempts = 20;

    private int packagesAvailable = 0;
    private int packagesInTransit = 0;
    private int packagesDelivered = 0;

    private List<Package> packages = new List<Package>();

    // Update is called once per frame
    void Update()
    {
        
    }

    private void AttemptPackageSpawn()
    {
        if(creatorList.Count < 1)
        {
            //Debug.LogWarning("Cannot spawn package, no creators registered with manager.");
            return;
        }
        float packageRate = (packageSpawnRate.Evaluate(packagesAvailable / MaxAvailablePackages) / 60) * spawnRateModifier; // convert curve rate to packages per minute
        if(Random.Range(0f, 1f) < packageRate)
        {
            PackageCreator creatorToSpawn = PackageCreator.RandomFromList(creatorList);
            int tries = 0;
            while (creatorToSpawn.hasPackage && tries++ < maxSpawnAttempts)
            {
                creatorToSpawn = PackageCreator.RandomFromList(creatorList);
            }
            if(!creatorToSpawn.hasPackage) 
            {
                creatorToSpawn.SpawnPackage();
            } else
            {
                //Debug.LogWarning("MaxAttempts reached, no packageless spawner found. Package not spawned.");
            }
        }
    }

    public void RegisterPackage(Package p)
    {
        packages.Add(p);
        packagesAvailable ++;
    }

    public void PackagePickedUp(Package p)
    {
        packagesAvailable--;
        packagesInTransit++;
    }

    public void PackageDelivered(Package p)
    {
        packagesInTransit--;
        packagesDelivered++;
    }

    public void PackageDestroyed(Package p)
    {
        packages.Remove(p);
    }
}
