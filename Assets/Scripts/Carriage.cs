using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class Carriage : MonoBehaviour
{

    public PathCreator pathCreator;
    public Train train;
    public Carriage leadingCarriage;
    public float distanceTraveled;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (train != null) 
        {
            transform.position = pathCreator.path.GetPointAtDistance(train.distanceTraveled - train.transform.localScale.z / 2 - transform.localScale.z / 2 - 0.1f);
            transform.rotation = pathCreator.path.GetRotationAtDistance(train.distanceTraveled - train.transform.localScale.z / 2 - transform.localScale.z / 2 - 0.1f);

            distanceTraveled = train.distanceTraveled - train.transform.localScale.z / 2 - transform.localScale.z / 2 - 0.1f;
        }
        else
        {
            transform.position = pathCreator.path.GetPointAtDistance(leadingCarriage.distanceTraveled - leadingCarriage.transform.localScale.z / 2 - transform.localScale.z / 2 - 0.1f);
            transform.rotation = pathCreator.path.GetRotationAtDistance(leadingCarriage.distanceTraveled - leadingCarriage.transform.localScale.z / 2 - transform.localScale.z / 2 - 0.1f);

            distanceTraveled = leadingCarriage.distanceTraveled - leadingCarriage.transform.localScale.z / 2 - transform.localScale.z / 2 - 0.1f;
        }

    }
}
