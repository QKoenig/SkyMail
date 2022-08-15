using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CompasManager : MonoBehaviour
{
    public GameObject iconPrefab;
    List<CompassMarker> compassMarkers = new List<CompassMarker>();

    public Image compassImage;
    public Transform player;

    float compassUnit;
    // Start is called before the first frame update
    void Start()
    {
        compassUnit = compassImage.rectTransform.rect.width / 360f;
    }

    // Update is called once per frame
    void Update()
    {
        foreach(CompassMarker marker in compassMarkers)
        {
            if(marker.enabled)
            {
                marker.image.rectTransform.anchoredPosition = GetPosOnCompass(marker);
                marker.image.enabled = true;
            } else
            {
                marker.image.enabled = false;
            }
        }
    }

    public void AddCompassMarker(CompassMarker marker)
    {
        GameObject newMarker = Instantiate(iconPrefab, compassImage.transform);
        marker.image = newMarker.GetComponent<Image>();
        marker.image.sprite = marker.icon;

        compassMarkers.Add(marker);
    }

    Vector2 GetPosOnCompass(CompassMarker marker)
    {
        Vector2 playerPos = new Vector2(player.transform.position.x, player.transform.position.z);
        Vector2 playerFwd = new Vector2(player.transform.forward.x, player.transform.forward.z);

        float angle = Vector2.SignedAngle(marker.position - playerPos, playerFwd);

        return new Vector2(compassUnit * angle, 0f);
    }
}
