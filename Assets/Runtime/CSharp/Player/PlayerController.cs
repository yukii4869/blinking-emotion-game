using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float gravity = -9.81f;
    public float jumpHeight = 1.5f;

    [Header("Camera Settings")]
    public Transform cameraTransform;
    public float mouseSensitivity = 1.5f;
    public float maxLookAngle = 80f;

    private Vector3 knockbackVelocity;
    public float knockbackDecay = 10f;


    private CharacterController controller;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private float verticalVelocity;
    private float cameraPitch = 0f;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }
    private void Start()
    {
        GameStateManager.Instance.OnStateChanged += HandleStateChanged;

        // Initialer Zustand
        HandleStateChanged(GameStateManager.Instance.CurrentState);
    }

    private void Update()
    {
        HandleMovement();
        HandleCamera();
    }
    private void HandleStateChanged(GameState state)
    {
        bool isGameplay = (state == GameState.Gameplay);

        // PlayerController aktivieren/deaktivieren
        this.enabled = isGameplay;

        // Cursor sperren oder freigeben
        Cursor.lockState = isGameplay ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !isGameplay;
    }
    private void OnDestroy()
    {
        GameStateManager.Instance.OnStateChanged -= HandleStateChanged;
    }


    // -----------------------------
    // Input System Callbacks
    // -----------------------------

    public void OnMove(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext ctx)
    {
        lookInput = ctx.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && controller.isGrounded)
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    // -----------------------------
    // Movement
    // -----------------------------

    private void HandleMovement()
    {
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        move *= moveSpeed;

        // Gravity
        if (controller.isGrounded && verticalVelocity < 0)
            verticalVelocity = -2f;

        verticalVelocity += gravity * Time.deltaTime;
        move.y = verticalVelocity;

        //  Knockback hinzufügen
        if (knockbackVelocity.magnitude > 0.1f)
        {
            move += knockbackVelocity;
            knockbackVelocity = Vector3.Lerp(knockbackVelocity, Vector3.zero, knockbackDecay * Time.deltaTime);
        }

        // Finaler Move
        controller.Move(move * Time.deltaTime);
    }


    // -----------------------------
    // Camera
    // -----------------------------

    private void HandleCamera()
    {
        // Horizontal rotation (player body)
        transform.Rotate(Vector3.up * lookInput.x * mouseSensitivity);

        // Vertical rotation (camera pitch)
        cameraPitch -= lookInput.y * mouseSensitivity;
        cameraPitch = Mathf.Clamp(cameraPitch, -maxLookAngle, maxLookAngle);

        cameraTransform.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
    }

    public void ApplyKnockback(Vector3 direction, float strength, float upward)
    {
        direction.Normalize();

        Vector3 horizontal = new Vector3(direction.x, 0, direction.z) * strength;
        Vector3 vertical = Vector3.up * upward;

        knockbackVelocity = horizontal + vertical;
    }

}
