using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementNetwork : NetworkBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public float lookSensitivity = 1f;
    public float maxPitchAngle = 80f; // Limit camera pitch rotation
    private float pitch = 0f;

    private Rigidbody rb;
    private Vector2 movementInput, lookInput;
    
    private InputMap controls;

    [SerializeField] private GameObject cameraPrefab;
    [SerializeField] private GameObject pauseMenuPrefab;
    private bool paused = false;

    private Transform cameraHolder;

    private void Awake()
    {
        controls = new InputMap();
    }

    private void OnEnable()
    {
        controls.Enable();
        controls.Movement.Move.performed += OnMovePerformed;
        controls.Movement.Move.canceled += OnMoveCanceled;

        controls.Movement.Look.performed += OnLookPerformed;
        controls.Movement.Look.canceled += OnLookCanceled;

        controls.UI.Pause.performed += TogglePause;
    }

    private void OnDisable()
    {
        controls.Movement.Move.performed -= OnMovePerformed;
        controls.Movement.Move.canceled -= OnMoveCanceled;

        controls.Movement.Look.performed -= OnLookPerformed;
        controls.Movement.Look.canceled -= OnLookCanceled;

        controls.UI.Pause.performed -= TogglePause;

        controls.Disable();
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        if(IsOwner)
        {
            SetupCamera();
        }
    }

    private void SetupCamera()
    {
        GameObject oldCam = GameObject.Find("MenuCamera");
        Destroy(oldCam);

        cameraHolder = transform.Find("CameraHolder")?.transform;
        GameObject playerCam = Instantiate(cameraPrefab, cameraHolder);

        playerCam.transform.localPosition = new Vector3(0, 0, 0);
        playerCam.transform.localRotation = Quaternion.Euler(0, 0, 0);
    }

    private void Update()
    {
        // Only allow the player who owns this object to control it
        if (IsOwner && !paused)
        {
            Move();
            Look();
        }
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        // Get movement input as a Vector2
        movementInput = context.ReadValue<Vector2>();
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        // Stop movement when input is canceled
        movementInput = Vector2.zero;
    }

    private void OnLookPerformed(InputAction.CallbackContext context)
    {
        // Get look input as a Vector2
        lookInput = context.ReadValue<Vector2>();
    }

    private void OnLookCanceled(InputAction.CallbackContext context)
    {
        // Stop looking when input is canceled
        lookInput = Vector2.zero;
    }

    private void Move()
    {
        float verticalVelocity = rb.velocity.y;

        Vector3 moveDirection = new Vector3(movementInput.x, 0, movementInput.y);
        moveDirection = cameraHolder.transform.TransformDirection(moveDirection);
        moveDirection.y = 0;

        rb.velocity = moveDirection * moveSpeed + Vector3.up * verticalVelocity;
    }

    private void Look()
    {
        if(cameraHolder == null)
        {
            return;
        }

        cameraHolder.transform.Rotate(Vector3.up * lookInput.x * lookSensitivity, Space.World);

        pitch -= lookInput.y * lookSensitivity;
        pitch = Mathf.Clamp(pitch, -maxPitchAngle, maxPitchAngle);
        cameraHolder.transform.localEulerAngles = new Vector3(pitch, cameraHolder.transform.localEulerAngles.y, 0);
    }

    public void TogglePause(InputAction.CallbackContext context = default)
    {
        if (IsOwner)
        {
            if(!paused){
                GameObject pauseMenu = Instantiate(pauseMenuPrefab);
                Cursor.lockState = CursorLockMode.None;
            } else {
                Cursor.lockState = CursorLockMode.Locked;
            }
            paused = !paused;
        }
    }
}
