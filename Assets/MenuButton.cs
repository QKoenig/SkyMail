using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButton : MonoBehaviour
{
    public float vertThrow;
    public float rotThrow;
    public float vertSpeed;
    public float rotSpeed;

    private float offset;
    private Vector3 pos;
    // Start is called before the first frame update
    void Start()
    {
        pos = transform.position;
        offset = Random.Range(0, 1000);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = pos + new Vector3(0, Mathf.Sin(Time.time * vertSpeed + offset) * vertThrow, 0);
        transform.rotation = Quaternion.Euler(new Vector3(Mathf.Sin(Time.time * rotSpeed + offset) * rotThrow, 0, Mathf.Cos(Time.time * rotSpeed) * rotThrow));
    }
}
