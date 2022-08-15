using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PackageHolder))]
public class PlayerControl : MonoBehaviour
{
    public Collider playerCollider;
    public float jumpVelocity = 5f;
    public float acceleration = 1f;
    public float slideAcceleration = 1f;
    public float runSpeed = 10f;
    public float maxVelocity = 100f;

    public float throwMinVel = 5f;
    public float throwMaxVel = 10f;
    public float throwChargeTime = 2f;
    public float aimAdjustFactor = .3f;
    public Transform throwPosition;
    public int throwLineAccuracy = 10;
    public GameObject throwPackage;

    public Vector3 gravity = new Vector3(0, -9.81f, 0);

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    private LineRenderer lr;
    private Vector3 grapplePoint;
    public float grappleForce = 100;
    public LayerMask whatIsGrappleable;
    public Transform gunTip;
    public float grappleDistance = 200f;

    private bool isGrappling = false;
    private bool isSliding = false;
    private bool isThrowing = false;

    private float throwStartTime = 0f;

    bool isGrounded;

    public Transform packageHoldPoint;

    private PlayerInputActions playerInputActions;

    public CinemachineVirtualCamera cinemachineCam;
    private Transform cameraTransform;

    private Rigidbody rb;

    private float scaleVelocity;

    private Vector3 throwVelocity;
    private LineRenderer throwLine;
    private List<Vector3> throwLinePoints = new List<Vector3>();
    private PackageHolder packageHolder;

    GameObject movingGrappledObject;
    Vector3 grappleOffset;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        throwLine = throwPosition.GetComponent<LineRenderer>();
        cameraTransform = Camera.main.transform;
    }

    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
        packageHolder = GetComponent<PackageHolder>();
        lr.positionCount = 0;

        Cursor.lockState = CursorLockMode.Locked;
        playerInputActions = new PlayerInputActions();
        playerInputActions.PlayerControl.Enable();
        playerInputActions.PlayerControl.Jump.performed += Jump;
        playerInputActions.PlayerControl.Grapple.performed += StartGrapple;
        playerInputActions.PlayerControl.Grapple.canceled += StopGrapple;
        playerInputActions.PlayerControl.Slide.performed += StartSlide;
        playerInputActions.PlayerControl.Slide.canceled += StopSlide;
        playerInputActions.PlayerControl.Throw.performed += StartThrow;
        playerInputActions.PlayerControl.Throw.canceled += StopThrow;
        playerInputActions.PlayerControl.ChangeSelection.performed += ChangeSelection;
    }

    private void OnEnable()
    {
        playerInputActions.Enable();
    }
    
    private void OnDisable()
    {
        playerInputActions.Disable();
    }

    private void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if(isThrowing)
        {
            float chargePercent = Mathf.Clamp((Time.time - throwStartTime) / throwChargeTime, 0, 1);
/*            RaycastHit hit;

            if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f)), out hit, grappleDistance, whatIsGrappleable))
            {
                //((hit.point + new Vector3(0, (hit.point - throwPosition.position).magnitude * aimAdjustFactor, 0)) - throwPosition.position).normalized
                throwVelocity = (hit.point - throwPosition.position).normalized * ((chargePercent * (throwMaxVel - throwMinVel)) + throwMinVel);
            } else
            {
                throwVelocity = cameraTransform.forward * ((chargePercent * (throwMaxVel - throwMinVel)) + throwMinVel);
            }*/
            throwVelocity = cameraTransform.forward * ((chargePercent * (throwMaxVel - throwMinVel)) + throwMinVel);

            /*throwLinePoints.Clear();

            for(int i = 0; i < throwLineAccuracy; i++)
            {
                float t = ((float)i/throwLineAccuracy) * 2f;
                Vector3 position = new Vector3(
                    throwPosition.position.x + (throwVelocity.x * t) - .5f * -Physics.gravity.x * t * t,
                    throwPosition.position.y + (throwVelocity.y * t) - .5f * -Physics.gravity.y * t * t,
                    throwPosition.position.z + (throwVelocity.z * t) - .5f * -Physics.gravity.z * t * t
                ) ;

                throwLinePoints.Add(position);
            }

            throwLine.positionCount = throwLinePoints.Count;
            throwLine.SetPositions(throwLinePoints.ToArray());*/
        } else
        {
            //throwLine.positionCount = 0;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
    }

    private void LateUpdate()
    {
        transform.rotation = Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0);
        DrawRope();
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (movingGrappledObject != null)
        {
            grapplePoint = movingGrappledObject.transform.TransformPoint(grappleOffset);
        }

        //Rotate with camera

        Vector2 inputVector = playerInputActions.PlayerControl.Movement.ReadValue<Vector2>();
        Vector3 move = transform.right * inputVector.x + transform.forward * inputVector.y;

        if (rb.velocity.magnitude < runSpeed ||  (rb.velocity + move * Time.fixedDeltaTime).magnitude < rb.velocity.magnitude)
        {
            rb.AddForce(move * (isSliding ? slideAcceleration : acceleration) * Time.fixedDeltaTime);
        }

        if(isGrounded && !isGrappling && !isSliding && (inputVector.magnitude < .01 || rb.velocity.magnitude > runSpeed))
        {
            rb.drag = 5;
        } else
        {
            rb.drag = 0;
        }

        //Gravity
        rb.AddForce(gravity * Time.fixedDeltaTime, ForceMode.Acceleration);

        //Grapple
        if(isGrappling)
        {
            rb.AddForce((grapplePoint - transform.position) * grappleForce * Time.fixedDeltaTime, ForceMode.VelocityChange);
        }
        if(rb.velocity.magnitude > maxVelocity)
        {
            rb.velocity = rb.velocity.normalized * maxVelocity;
        }

        //slide
        if(isSliding)
        {
            transform.localScale = new Vector3(1, Mathf.SmoothDamp(transform.localScale.y, .2f, ref scaleVelocity, .1f), 1);
        } else
        {
            transform.localScale = new Vector3(1, Mathf.SmoothDamp(transform.localScale.y, 1f, ref scaleVelocity, .1f), 1);
        }
    }

    public void AddVelocity(Vector3 deltaVel)
    {
        rb.AddForce(deltaVel, ForceMode.VelocityChange);
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if(isGrounded)
        {
            rb.AddForce(jumpVelocity * - gravity.normalized, ForceMode.Impulse);
        }
    }

    /// <summary>
    /// Call whenever we want to start a grapple
    /// </summary>
    void StartGrapple(InputAction.CallbackContext context)
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f)), out hit, grappleDistance, whatIsGrappleable))
        {
            grapplePoint = hit.point;

            movingGrappledObject = hit.collider.gameObject;
            grappleOffset = movingGrappledObject.transform.InverseTransformPoint(grapplePoint);

            lr.positionCount = 2;
            currentGrapplePosition = gunTip.position;
            isGrappling = true;
        }
    }


    /// <summary>
    /// Call whenever we want to stop a grapple
    /// </summary>
    void StopGrapple(InputAction.CallbackContext context)
    {
        movingGrappledObject = null;
        lr.positionCount = 0;
        isGrappling = false;
    }

    private Vector3 currentGrapplePosition;

    void DrawRope()
    {
        //If not grappling, don't draw rope
        if (!isGrappling) return;
        currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, grapplePoint, Time.deltaTime * 8f);

        lr.SetPosition(0, gunTip.position);
        lr.SetPosition(1, currentGrapplePosition);
    }

    public bool IsGrappling()
    {
        return isGrappling;
    }

    public Vector3 GetGrapplePoint()
    {
        return grapplePoint;
    }

    public void StartSlide(InputAction.CallbackContext context)
    {
        isSliding = true;
    }
    public void StopSlide(InputAction.CallbackContext context)
    {
        isSliding = false;
    }
    public void StartThrow(InputAction.CallbackContext context)
    {
        isThrowing = true;
        throwStartTime = Time.time;
    }
    public void StopThrow(InputAction.CallbackContext context)
    {
        isThrowing = false;
        //GameObject package = Instantiate(throwPackage, throwPosition.position, throwPosition.rotation);
        //package.GetComponent<Rigidbody>().velocity = throwVelocity;
        packageHolder.LaunchCurrentPackage(throwVelocity + rb.velocity);
    }
    
    public void ChangeSelection(InputAction.CallbackContext context)
    {
        float value = context.ReadValue<float>();
        if (value < 0)
        {
            packageHolder.DecreaseSelection();
        } else if(value > 0)
        {
            packageHolder.IncreaseSelection();
        }
    }
}
