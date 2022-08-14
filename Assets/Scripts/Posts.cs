using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Posts : MonoBehaviour
{
    public GameObject post;
    public int numPosts = 10;
    public int spacing = 10;
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

        if (numPosts > 5000)
        {
            return;
        }

        Transform lastPost = transform;
        for (int i = 0; i < numPosts; i++)
        {
            GameObject lastPostObj = Instantiate(post, position: new Vector3(lastPost.position.x + spacing, lastPost.position.y, lastPost.position.z), lastPost.rotation);
            lastPost = lastPostObj.transform;
            lastPost.parent = transform;
        }
    }

    IEnumerator Destroy(GameObject go)
    {
        yield return null;
        DestroyImmediate(go);
    }
}
