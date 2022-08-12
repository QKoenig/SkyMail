using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(GrapplingGun))]
public class PlayerControl : MonoBehaviour
{
    public Collider playerCollider;
    public float jumpVelocity = 5f;
    public float acceleration = 1f;
    public float runSpeed = 10f;
    public float dragCoef = 1f;

    public Vector3 gravity = new Vector3(0, -9.81f, 0);

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    bool isGrounded;


    private PlayerInputActions playerInputActions;

    public CinemachineVirtualCamera cinemachineCam;
    private Transform cameraTransform;

    private Rigidbody rb;
    private GrapplingGun grapplingGun;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        grapplingGun = GetComponent<GrapplingGun>();
        cameraTransform = Camera.main.transform;
    }

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        playerInputActions = new PlayerInputActions();
        playerInputActions.PlayerControl.Enable();
        playerInputActions.PlayerControl.Jump.performed += Jump;
        playerInputActions.PlayerControl.Grapple.performed += StartGrapple;
        playerInputActions.PlayerControl.Grapple.canceled += StopGrapple;
    }

    private void OnEnable()
    {
        playerInputActions.Enable();
    }
    
    private void OnDisable()
    {
        playerInputActions.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        //Rotate with camera
        transform.rotation = Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0);

        Vector2 inputVector = playerInputActions.PlayerControl.Movement.ReadValue<Vector2>();
        Vector3 move = transform.right * inputVector.x + transform.forward * inputVector.y;

        if(rb.velocity.magnitude < runSpeed)
        {
            rb.AddForce(move * acceleration * Time.deltaTime);
        }


        //Gravity
        rb.velocity += gravity * Time.deltaTime;
    }

    public void AddVelocity(Vector3 deltaVel)
    {
        rb.AddForce(deltaVel);
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if(isGrounded)
        {
            rb.AddForce(jumpVelocity * - gravity.normalized);
        }
    }

    public void StartGrapple(InputAction.CallbackContext context)
    {
        grapplingGun.StartGrapple();
    }

    public void StopGrapple(InputAction.CallbackContext context)
    {
        grapplingGun.StopGrapple();
    }
}
