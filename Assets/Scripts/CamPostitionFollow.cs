using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamPostitionFollow : MonoBehaviour
{
    public Transform follow;
    private void LateUpdate()
    {
        transform.position = follow.position;
        transform.rotation = follow.rotation;
    }
}
