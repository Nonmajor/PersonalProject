using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections; // Coroutine�� ����ϱ� ���� �߰�

public class PlayerController : MonoBehaviour
{
    private PlayerControls playerControls;
    private PlayerStateMachine stateMachine;
    private CharacterController characterController;

    // Movement
    [Header("Movement")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float moveSpeed = 0f; // �÷��̾��� ���� �̵� �ӵ� (���¿� ���� ����)
    [HideInInspector] public Vector2 moveInput; // WASD Ű �Է� �� (Vector2)

    // Booster
    [Header("Booster")]
    [HideInInspector] public bool isBoosterActive = false; // �ν��� Ȱ��ȭ ����
    private float boosterDuration = 10f;
    private float speedMultiplier = 2f;
    private float originalWalkSpeed;
    private float originalRunSpeed;

    // Stamina
    [Header("Stamina")]
    public float maxStamina = 100f;
    public float currentStamina;
    public float staminaDrainRate = 10f;
    public float staminaRegenRate = 5f;
    [HideInInspector] public bool staminaRegenCooldown = false; // ���׹̳� ��� ��ٿ� ����

    // Exhausted (���׹̳� �� �� Ż��)
    [Header("Exhausted State")]
    public float exhaustedDuration = 5f; // Ż�� ���� ���� �ð� (��)
    [Range(0.1f, 1f)] // �ν����Ϳ��� ���� ����
    public float exhaustedSpeedMultiplier = 0.5f; // Ż�� ���¿��� �̵� �ӵ� ����

    // UI
    [Header("UI")]
    public Slider staminaSlider; // ���׹̳� ������ UI �����̴�

    // Actions
    [Header("Actions")]
    [HideInInspector] public bool isRunning = false; // �޸��� ������ ����
    [HideInInspector] public bool isUsingItem = false; // ������ ��� ������ ����
    [HideInInspector] public bool isPickingUpItem = false; // ������ ȹ�� ������ ����

    // Camera Look
    [Header("Camera Look")]
    public Transform cameraFollowTarget; // ī�޶� ���� �� ������Ʈ
    public float mouseSensitivity = 100f; // ���콺 ����
    private float xRotation = 0f; // ���� ȸ�� ��
    private Vector2 lookInput; // ���콺 �̵� �Է� �� (Delta)

    private void Awake()
    {
        // �ʿ��� ������Ʈ �� Input System �ν��Ͻ� �ʱ�ȭ
        playerControls = new PlayerControls();
        stateMachine = GetComponent<PlayerStateMachine>();
        characterController = GetComponent<CharacterController>();

        // ���� ���׹̳��� �ִ�� ����
        currentStamina = maxStamina;

        // Input System �̺�Ʈ ���
        playerControls.Player.Move.performed += OnMovePerformed;
        playerControls.Player.Move.canceled += OnMoveCanceled;
        playerControls.Player.Run.performed += OnRunPerformed;
        playerControls.Player.Run.canceled += OnRunCanceled;
        playerControls.Player.UseItem.performed += OnUseItemPerformed;
        playerControls.Player.UseItem.canceled += OnUseItemCanceled;
        playerControls.Player.PickUpItem.performed += OnPickUpItemPerformed;
        playerControls.Player.PickUpItem.canceled += OnPickUpItemCanceled;
        playerControls.Player.Look.performed += OnLookPerformed;
        playerControls.Player.Look.canceled += OnLookCanceled;

        // �����̴� UI�� �ִ밪 �� �ʱⰪ ����
        if (staminaSlider != null)
        {
            staminaSlider.maxValue = maxStamina;
            staminaSlider.value = currentStamina;
        }

        // �ν��� ȿ���� ���� ���� �ӵ� ����
        originalWalkSpeed = walkSpeed;
        originalRunSpeed = runSpeed;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    public void OnMovePerformed(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnMoveCanceled(InputAction.CallbackContext context)
    {
        moveInput = Vector2.zero;
    }

    public void OnRunPerformed(InputAction.CallbackContext context)
    {
        isRunning = true;
    }

    public void OnRunCanceled(InputAction.CallbackContext context)
    {
        isRunning = false;
    }

    public void OnUseItemPerformed(InputAction.CallbackContext context)
    {
        isUsingItem = true;
    }

    public void OnUseItemCanceled(InputAction.CallbackContext context)
    {
        isUsingItem = false;
    }

    public void OnPickUpItemPerformed(InputAction.CallbackContext context)
    {
        isPickingUpItem = true;
    }

    public void OnPickUpItemCanceled(InputAction.CallbackContext context)
    {
        isPickingUpItem = false;
    }

    private void OnLookPerformed(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    private void OnLookCanceled(InputAction.CallbackContext context)
    {
        lookInput = Vector2.zero;
    }

    private void Update()
    {
        stateMachine.currentState?.OnUpdate();
        HandleLook();
        UpdateStaminaUI();
    }

    private void FixedUpdate()
    {
        stateMachine.currentState?.OnFixedUpdate();
    }

    // �̵� ó��
    public void HandleMovement()
    {
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        forward.y = 0;
        right.y = 0;

        Vector3 desiredMoveDirection = forward * moveInput.y + right * moveInput.x;

        // �ν��Ͱ� Ȱ��ȭ�� ���, moveSpeed�� ���� ����
        if (isBoosterActive)
        {
            moveSpeed = isRunning ? runSpeed * speedMultiplier : walkSpeed * speedMultiplier;
        }
        else
        {
            // �ν��Ͱ� ��Ȱ��ȭ�� ���, ���� �ӵ��� moveSpeed�� ����
            moveSpeed = isRunning ? runSpeed : walkSpeed;
        }

        characterController.Move(desiredMoveDirection * moveSpeed * Time.fixedDeltaTime);
    }

    // �ν��� ȿ���� Ȱ��ȭ�ϴ� �Լ�
    public void ActivateBoosterEffect(float duration, float multiplier)
    {
        if (isBoosterActive) return;

        isBoosterActive = true;
        boosterDuration = duration;
        speedMultiplier = multiplier;
        StartCoroutine(BoosterTimer(duration));

        Debug.Log("�ν��� ȿ���� ���۵Ǿ����ϴ�. �̵� �ӵ� 2��!");
    }

    // �ν��� ���� �ð� �ڷ�ƾ
    private IEnumerator BoosterTimer(float duration)
    {
        yield return new WaitForSeconds(duration);

        isBoosterActive = false;

        Debug.Log("�ν��� ȿ���� ����Ǿ����ϴ�. ���� �ӵ��� �����մϴ�.");
    }

    private void HandleLook()
    {
        if (lookInput.sqrMagnitude > 0.001f)
        {
            float mouseX = lookInput.x * mouseSensitivity * Time.deltaTime;
            float mouseY = lookInput.y * mouseSensitivity * Time.deltaTime;

            transform.Rotate(Vector3.up * mouseX);

            if (cameraFollowTarget != null)
            {
                xRotation -= mouseY;
                xRotation = Mathf.Clamp(xRotation, -90f, 90f);
                cameraFollowTarget.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            }
        }
    }

    public void UseStamina()
    {
        currentStamina -= staminaDrainRate * Time.fixedDeltaTime;
        if (currentStamina < 0)
        {
            currentStamina = 0;
        }
    }

    public void RegenerateStamina()
    {
        if (!staminaRegenCooldown && currentStamina < maxStamina)
        {
            currentStamina += staminaRegenRate * Time.fixedDeltaTime;

            if (currentStamina > maxStamina)
            {
                currentStamina = maxStamina;
            }
        }
    }

    private void UpdateStaminaUI()
    {
        if (staminaSlider != null)
        {
            staminaSlider.value = currentStamina;
        }
    }
}