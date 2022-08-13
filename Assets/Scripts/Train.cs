using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class Train: MonoBehaviour
{
    public PathCreator pathCreator;
    public float speed = 1;
    public float distanceTraveled;
    public int numCarriages = 1;
    public GameObject carriageGameObject;

    // Start is called before the first frame update
    void Start()
    {
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
    void Update()
    {
        distanceTraveled += Time.deltaTime + speed;
        distanceTraveled %= pathCreator.path.length;
        transform.position = pathCreator.path.GetPointAtDistance(distanceTraveled);
        transform.rotation = pathCreator.path.GetRotationAtDistance(distanceTraveled);
    }
}
