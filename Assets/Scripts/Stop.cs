using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class Stop : MonoBehaviour
{
    public PathCreator path;
    [Range(0f, 1f)]
    public float positionOnPath = 0;

    [Range(0f, 1f)]
    public float decelerateLength;
    public Transform decelerate;

    [Range(0f, 1f)]
    public float accelerateLength;
    public Transform accelerate;

    public float stopTime = 10;

    public float timeToFirstStop = -1;
    public float timeUntilNextStop = -1;
    public float totalTimeForPath;
    // Start is called before the first frame update
    void Start()
    {
        if (timeToFirstStop != -1)
        {
            timeUntilNextStop = timeToFirstStop;
        }

        totalTimeForPath = path.totalTime;
    }

    // Update is called once per frame
    void Update()
    {
        totalTimeForPath = path.totalTime;
        timeUntilNextStop -= Time.deltaTime;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawCube(transform.position, new Vector3(0.1f, 2, 0.1f));

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(decelerate.position, 0.2f);

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(accelerate.position, 0.2f);
    }

    private void OnValidate()
    {
        if (path != null)
        {
            transform.position = path.path.GetPointAtTime(positionOnPath);
            Vector3 pathRotation = path.path.GetRotation(positionOnPath).eulerAngles;
            transform.rotation = Quaternion.Euler(pathRotation.x, pathRotation.y, 0);

            decelerate.position = path.path.GetPointAtTime(positionOnPath - decelerateLength);

            accelerate.position = path.path.GetPointAtTime(positionOnPath + accelerateLength);
        }
    }

    public void resetTimer()
    {
        totalTimeForPath = path.totalTime;
        timeUntilNextStop = totalTimeForPath;
    }
}
