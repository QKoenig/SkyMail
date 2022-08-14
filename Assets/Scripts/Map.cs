using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    bool isVisible = false;
    public GameObject map;
    private void Start()
    {
        isVisible = map.activeInHierarchy;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            isVisible = !isVisible;
            map.SetActive(isVisible);
        }
    }
}
