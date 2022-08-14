using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnFog : MonoBehaviour
{
    public ParticleSystem fog;
    public GameObject fogGO;
    public int width = 2;
    public int height = 2;
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

        if (height > 100 || width > 100)
        {
            return;
        }

        for (int h = 0; h < height; h++)
        {
            for (int w = 0; w < width; w++)
            {
                GameObject cont = Instantiate(fogGO, position: new Vector3(transform.position.x + 100 * w, transform.position.y, transform.position.z + 100 * w), transform.rotation);
                cont.transform.parent = transform;
            }
        }
    }

    IEnumerator Destroy(GameObject go)
    {
        yield return null;
        DestroyImmediate(go);
    }
}
