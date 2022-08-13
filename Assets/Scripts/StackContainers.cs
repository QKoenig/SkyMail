using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StackContainers : MonoBehaviour
{
    public GameObject container;
    public int width = 2;
    public int depth = 2;
    public int height = 2;
    public float spacing = 0.05f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnValidate()
    {
        foreach (Transform child in transform)
        {
            StartCoroutine(Destroy(child.gameObject));
        }

        if (height > 100 || width > 100 || depth > 100)
        {
            return;
        }

        for (int h = 0; h < height; h++)
        {
            for (int w = 0; w < width; w++)
            {
                for (int i = 0; i < depth; i++)
                {
                    GameObject cont = Instantiate(container, position: new Vector3(transform.position.x + container.transform.localScale.x*i + spacing * i, transform.position.y + container.transform.localScale.y * h + spacing * h, transform.position.z + container.transform.localScale.z * w + spacing * w), transform.rotation);
                    cont.transform.parent = transform;
                }
            }
        }
    }

    IEnumerator Destroy(GameObject go)
    {
        yield return null;
        DestroyImmediate(go);
    }
}
