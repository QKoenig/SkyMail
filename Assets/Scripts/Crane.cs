using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crane : MonoBehaviour
{
    public Transform top;
    public float speed = 1;

    [Range(0f, 365f)]
    public float startAngle = 0;
    public float degreesToMove = 90;
    float degreesMoved = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float curAngle = top.eulerAngles.y;
        float degrees = speed * Time.deltaTime;
        degreesMoved += degrees;
        if (Mathf.Abs(degreesMoved) < degreesToMove)
        {
            top.eulerAngles = new Vector3(0, curAngle + degrees, 0);
        }
        else
        {
            speed *= -1;
            degreesMoved = 0;
        }
    }

    private void OnValidate()
    {
        top.eulerAngles = new Vector3(0, startAngle, 0);
    }
}
