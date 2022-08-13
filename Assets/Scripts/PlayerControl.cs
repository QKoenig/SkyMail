using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//[RequireComponent(typeof(Rigidbody))]
//[RequireComponent(typeof(GrapplingGun))]
public class PlayerControl : MonoBehaviour
{
    public Collider playerCollider;
    public float jumpVelocity = 5f;
    public float acceleration = 1f;
    public float slideAcceleration = 1f;
    public float runSpeed = 10f;
    public float maxVelocity = 100f;

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

    bool isGrounded;


    private PlayerInputActions playerInputActions;

    public CinemachineVirtualCamera cinemachineCam;
    private Transform cameraTransform;

    private Rigidbody rb;
    private GrapplingGun grapplingGun;

    private float scaleVelocity;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        grapplingGun = GetComponent<GrapplingGun>();
        cameraTransform = Camera.main.transform;
    }

    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
        lr.positionCount = 0;

        Cursor.lockState = CursorLockMode.Locked;
        playerInputActions = new PlayerInputActions();
        playerInputActions.PlayerControl.Enable();
        playerInputActions.PlayerControl.Jump.performed += Jump;
        playerInputActions.PlayerControl.Grapple.performed += StartGrapple;
        playerInputActions.PlayerControl.Grapple.canceled += StopGrapple;
        playerInputActions.PlayerControl.Slide.performed += StartSlide;
        playerInputActions.PlayerControl.Slide.canceled += StopSlide;
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
}
