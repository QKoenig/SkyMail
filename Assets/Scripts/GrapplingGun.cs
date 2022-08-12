using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(PlayerControl))]

public class GrapplingGun : MonoBehaviour
{

    private LineRenderer lr;
    private PlayerControl player;
    private Vector3 grapplePoint;
    public LayerMask whatIsGrappleable;
    public Transform gunTip, camera;
    public float grappleSpeed = 1;
    private float maxDistance = 100f;

    private bool isGrappling = false;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        player = GetComponent<PlayerControl>();
    }

    private void Update()
    {
        if (isGrappling)
        {
            player.AddVelocity((grapplePoint - player.transform.position).normalized * grappleSpeed * Time.deltaTime);
        }
    }

    //Called after Update
    void LateUpdate()
    {
        DrawRope();
    }

    /// <summary>
    /// Call whenever we want to start a grapple
    /// </summary>
    public void StartGrapple()
    {
        RaycastHit hit;
        if (Physics.Raycast(camera.position, camera.forward, out hit, maxDistance, whatIsGrappleable))
        {
            grapplePoint = hit.point;

            float distanceFromPoint = Vector3.Distance(transform.position, grapplePoint);

            lr.positionCount = 2;
            currentGrapplePosition = gunTip.position;
            isGrappling=true;
        }
    }


    /// <summary>
    /// Call whenever we want to stop a grapple
    /// </summary>
    public void StopGrapple()
    {
        lr.positionCount = 0;
        isGrappling = false;
    }

    private Vector3 currentGrapplePosition;

    void DrawRope()
    {
        //If not grappling, don't draw rope
        if(isGrappling)
        {
            currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, grapplePoint, Time.deltaTime * 8f);

            lr.SetPosition(0, gunTip.position);
            lr.SetPosition(1, currentGrapplePosition);
        }

    }

    public bool IsGrappling()
    {
        return isGrappling;
    }

    public Vector3 GetGrapplePoint()
    {
        return grapplePoint;
    }
}