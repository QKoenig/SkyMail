using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawOnMap : MonoBehaviour
{
    public Texture2D texture;
    public Material material;
    GameObject plane;
    // Start is called before the first frame update
    void Start()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        Mesh mesh = meshFilter.mesh;
        Renderer renderer = GetComponent<Renderer>();

        plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        Renderer planeRenderer = plane.GetComponent<Renderer>();


        Quaternion originalRotation = transform.rotation;
        transform.SetPositionAndRotation(transform.position, Quaternion.Euler(0,0,0));
        plane.transform.localScale = new Vector3(renderer.bounds.extents.x / planeRenderer.bounds.extents.x, 0.1f, renderer.bounds.extents.z / planeRenderer.bounds.extents.z);
        transform.SetPositionAndRotation(transform.position, originalRotation);
        plane.transform.rotation = transform.rotation;
        plane.transform.position = new Vector3(transform.position.x, transform.position.y + 100, transform.position.z);

        if (texture != null)
        {
            planeRenderer.material.mainTexture = texture;
        }
        else
        {
            planeRenderer.material = material;
        }

        plane.layer = 7;
        plane.transform.parent = transform;
    }
}
