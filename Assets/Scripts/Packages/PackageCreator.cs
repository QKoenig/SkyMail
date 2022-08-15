using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PackageCreator : MonoBehaviour
{
    public float acceptanceRadius = 10;
    public float arivalTimeMultiplier = 1f;
    public float deliveryTimeMultiplier = 1f;
    public float minTimeMultiplier = .8f;
    public float maxTimeMultiplier = 1.2f;
    public Transform player;
    public AvailablePackage packagePrefab;

    public bool hasPackage = false;

    private List<PackageAcceptor> packageAcceptors;


    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        packageAcceptors = new List<PackageAcceptor>(FindObjectsOfType<PackageAcceptor>());
    }

    public void SpawnPackage()
    {
        if(packageAcceptors.Count == 0)
        {
            Debug.LogWarning("No package acceptors registered.");
            return;
        }

        PackageAcceptor packageAcceptor = RandomFromList(packageAcceptors);

        float distanceFromPlayer = Vector3.Distance(transform.position, player.position);
        float distanceFromDestination = Vector3.Distance(transform.position, packageAcceptor.transform.position);

        float timeToDeliver = (distanceFromPlayer * arivalTimeMultiplier + distanceFromDestination * deliveryTimeMultiplier) * Random.Range(minTimeMultiplier, maxTimeMultiplier);

        if(packagePrefab == null)
        {
            Debug.LogWarning("No package prefab provided to package creator.");
            return;
        }

        AvailablePackage newPackage = Instantiate(packagePrefab);
        Package p = new Package(packageAcceptor, Package.PackageMode.Available, acceptanceRadius, timeToDeliver, this);
        newPackage.SetPackage(p);
        newPackage.SetHoldPoint(transform);
        hasPackage = true;
    }

    public void PackageTaken()
    {
        hasPackage = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, acceptanceRadius);
    }

    public static T RandomFromList<T>(List<T> list)
    {
        if (list.Count == 0)
            return default(T);
        if (list.Count == 1)
            return list[0];
        return list[Random.Range(0, list.Count)];
    }
}
