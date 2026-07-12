using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float gravity = -9.81f;
    public float jumpHeight = 1.5f;

    [Header("Stamina Settings")]
    public float sprintMultiplier = 2.2f;
    public float maxStamina = 2f;
    public float staminaDrain = 1f;      // pro Sekunde
    public float staminaRegen = 0.8f;    // pro Sekunde
    public float stamina;                // aktueller Wert
    public StaminaUI staminaUI;

    private bool isSprinting;

    [Header("Crouch Settings")]
    public float crouchHeight = 1.0f;
    public float standingHeight = 2.0f;
    public float crouchSpeedMultiplier = 0.5f;

    private bool isCrouching;

    [Header("Camera Settings")]
    public Transform cameraTransform;
    public float mouseSensitivity = 1.5f;
    public float maxLookAngle = 60;

    private Vector3 knockbackVelocity;
    public float knockbackDecay = 10f;


    private CharacterController controller;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private float verticalVelocity;
    private float cameraPitch = 0f;

    public bool IsOnMovingPlatform { get; set; }
    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }
    private void Start()
    {
        GameStateManager.Instance.OnStateChanged += HandleStateChanged;

        // Initialer Zustand
        HandleStateChanged(GameStateManager.Instance.CurrentState);
        stamina = maxStamina;
        staminaUI.SetStamina(stamina, maxStamina);
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
    public void OnSprint(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
            isSprinting = true;

        if (ctx.canceled)
            isSprinting = false;
    }
    public void OnCrouch(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
            StartCrouch();

        if (ctx.canceled)
            StopCrouch();
    }

    // -----------------------------
    // Movement
    // -----------------------------

    private void HandleMovement()
    {
        bool wantsToSprint = isSprinting && !isCrouching;

        // Spieler KANN sprinten, wenn er will UND genug Stamina hat
        bool canSprint = wantsToSprint && stamina > 0f;

        // Wenn Stamina leer → Sprint sofort stoppen
        if (!canSprint && isSprinting)
        {
            isSprinting = false;
        }

        float speed = moveSpeed;

        // Sprint-Speed
        if (canSprint)
        {
            speed *= sprintMultiplier;
        }

        // Crouch-Speed
        if (isCrouching)
        {
            speed *= crouchSpeedMultiplier;
        }

        // -----------------------------
        // Stamina-Logik
        // -----------------------------

        if (canSprint)
        {
            // Sprint verbraucht Stamina
            stamina -= staminaDrain * Time.deltaTime;
            stamina = Mathf.Clamp(stamina, 0f, maxStamina);
        }
        else
        {
            // WICHTIG:
            // Nur regenerieren, wenn der Spieler NICHT sprinten will
            if (!wantsToSprint)
            {
                stamina += staminaRegen * Time.deltaTime;
                stamina = Mathf.Clamp(stamina, 0f, maxStamina);
            }
        }

        // UI Update
        staminaUI.SetStamina(stamina, maxStamina);



        // Movement
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        move *= speed;

        // Gravity
        if (!IsOnMovingPlatform)
        {
            if (controller.isGrounded && verticalVelocity < 0)
                verticalVelocity = -2f;
        }

        verticalVelocity += gravity * Time.deltaTime;
        move.y = verticalVelocity;

        // Knockback
        if (knockbackVelocity.magnitude > 0.1f)
        {
            move += knockbackVelocity;
            knockbackVelocity = Vector3.Lerp(knockbackVelocity, Vector3.zero, knockbackDecay * Time.deltaTime);
        }

        controller.Move(move * Time.deltaTime);
    }

    private void StartCrouch()
    {
        isCrouching = true;
        controller.height = crouchHeight;
    }

    private void StopCrouch()
    {
        isCrouching = false;
        controller.height = standingHeight;
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
