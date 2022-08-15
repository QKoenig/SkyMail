using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CompassMarker : MonoBehaviour
{
    public Sprite icon;
    public Image image;
    public bool enabled = true;

    private void Awake()
    {
        FindObjectOfType<CompasManager>().AddCompassMarker(this);
    }

    public Vector2 position
    {
        get { return new Vector2(transform.position.x, transform.position.z); }
    }

    void OnDestroy()
    {
        if(!(image == null)) {
            Destroy(image.gameObject);
        }
    }
}
