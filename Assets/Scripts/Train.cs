using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class Train: MonoBehaviour
{
    public GameObject pathCreatorObject;
    PathCreator pathCreator;
    public float speed = 15;
    float curSpeed;
    public float distanceTraveled;
    public int numCarriages = 1;
    public GameObject carriageGameObject;

    bool isStopped = false;
    float stopTime = 5;
    float timeStopped = 0;

    bool isDecelerating = false;
    float decelerateLength = 0;
    float deceleratedDistance = 0;

    bool isAccelerating = false;
    float accelerateLength = 0;
    float acceleratedDistance = 0;

    List<Stop> stops = new List<Stop> ();

    bool startTiming = false;
    float startingStopPos = -1;
    public float totalTime = 87.90213f;
    // Start is called before the first frame update
    void Start()
    {
        pathCreator = pathCreatorObject.GetComponent<PathCreator>();

        foreach (Transform child in pathCreator.transform)
        {
            if (child.GetComponent<Stop>())
            {
                Stop stop = child.GetComponent<Stop>();
                stops.Add(stop);
            }
        }

        curSpeed = speed;

        GameObject firstCarriageGameObject = Instantiate(carriageGameObject);
        Carriage firstCarriage = firstCarriageGameObject.GetComponent<Carriage>();
        firstCarriage.train = GetComponent<Train>();
        firstCarriage.pathCreator = pathCreator;
        Carriage lastCarriage = firstCarriage;

        for (int i = 1; i < numCarriages; i++)
        {
            GameObject carriageGO = Instantiate(carriageGameObject);
            Carriage carriage = carriageGO.GetComponent<Carriage>();
            
            carriage.leadingCarriage = lastCarriage;
            carriage.pathCreator = pathCreator;
            lastCarriage = carriage;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (startTiming)
        {
            totalTime += Time.deltaTime;
        }
    }
    void FixedUpdate()
    {
        if (isDecelerating)
        {
            deceleratedDistance += Time.deltaTime * curSpeed;
            curSpeed = speed * (1 - deceleratedDistance / decelerateLength);
            curSpeed = Mathf.Max(curSpeed, 0.5f);
        }
        if (isAccelerating)
        {
            acceleratedDistance += Time.deltaTime * curSpeed;
            curSpeed = speed * (acceleratedDistance / accelerateLength);
            curSpeed = Mathf.Min(curSpeed, speed);
        }

        if (!isStopped)
        {           
            distanceTraveled += Time.deltaTime * curSpeed;
            distanceTraveled %= pathCreator.path.length;
            transform.position = pathCreator.path.GetPointAtDistance(distanceTraveled);
            transform.rotation = pathCreator.path.GetRotationAtDistance(distanceTraveled);
        }
        else
        {
            if (timeStopped > stopTime)
            {
                isStopped = false;

                isAccelerating = true;
                acceleratedDistance = 0.5f;

            }
            else
            {
                timeStopped += Time.deltaTime;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Stop>() != null)
        {
            Stop stop = other.gameObject.GetComponent<Stop>();
            stopTime = stop.stopTime;
            isStopped = true;
            isDecelerating = false;
            timeStopped = 0;

            if (startingStopPos == -1 && !startTiming)
            {
                startingStopPos = stop.positionOnPath;
                startTiming = true;
            }
            else if (startingStopPos == stop.positionOnPath && startTiming)
            {
                startTiming = false;
                print(totalTime);
            }
        }

        if (other.tag == "Decelerate")
        {
            if (other.transform.parent.GetComponent<Stop>() != null)
            {
                Stop stop = other.transform.parent.GetComponent<Stop>();
                isDecelerating = true;
                decelerateLength = stop.decelerateLength * pathCreator.path.length;
                accelerateLength = stop.accelerateLength * pathCreator.path.length;
                deceleratedDistance = 0.0001f;
            }
        }

        if (other.tag == "Accelerate")
        {
            isAccelerating = false;
            curSpeed = speed;
        }
    }

    class stopTimeTracker {
        float timeUntilNext = -1;
        float lastSeen = -1;
        Stop stop;
        float stopPos;
    }
}
