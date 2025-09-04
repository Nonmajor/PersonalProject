using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;
using Player.State;

// �÷��̾��� �̵�, ����, ���׹̳�, �ν���, ī�޶� ���� �Ѱ�
public class PlayerController : MonoBehaviour
{

    // ������ ��ũ��Ʈ ���� ����
    [Header("Item")]
    public FlashlightHandler flashlight;

    // === �߰�: �߼Ҹ� ���� ���� ===
    [Header("Footsteps")]
    public float walkFootstepInterval = 0.5f; // �ȱ� �߼Ҹ� ��� ���� (��)
    public float runFootstepInterval = 0.3f; // �޸��� �߼Ҹ� ��� ���� (��)
    [HideInInspector] public bool canPlayFootstep = true; // �߼Ҹ� ��� ���� ����
    // ===

    // ������Ʈ �� ���� ����
    private PlayerControls playerControls; // ����Ƽ Input System�� ���� �÷��̾� �Է� ����
    private PlayerStateMachine stateMachine; // �÷��̾� ���� ���� (State Machine)
    private CharacterController characterController; // �÷��̾� �̵� ó���� ���� ĳ���� ��Ʈ�ѷ� ������Ʈ


    // �̵� ���� ����
    [Header("Movement")]
    public float walkSpeed = 5f; // �ȱ� �ӵ�
    public float runSpeed = 10f; // �޸��� �ӵ�
    public float moveSpeed = 0f; // ���� �̵� �ӵ� (���¿� ���� �����)
    [HideInInspector] public Vector2 moveInput; // WASD Ű�� �̵� �Է°�


    // �ν��� ���� ����
    [Header("Booster")]
    [HideInInspector] public bool isBoosterActive = false; // �ν��� Ȱ��ȭ ����
    private float originalWalkSpeed; // �ν��� ��� �� ���� �ȱ� �ӵ�
    private float originalRunSpeed; // �ν��� ��� �� ���� �޸��� �ӵ�


    // ���׹̳� ���� ����
    [Header("Stamina")]
    public float maxStamina = 100f; // �ִ� ���׹̳�
    public float currentStamina; // ���� ���׹̳�
    public float staminaDrainRate = 10f; // �ʴ� ���׹̳� �Ҹ�
    public float staminaRegenRate = 5f; // �ʴ� ���׹̳� �����
    [HideInInspector] public bool staminaRegenCooldown = false; // ���׹̳� ��� ��ٿ� ����


    // Ż�� ���� ���� ����
    [Header("Exhausted State")]
    public float exhaustedDuration = 5f; // Ż�� ���� ���� �ð� (��)
    [Range(0.1f, 1f)]
    public float exhaustedSpeedMultiplier = 0.5f; // Ż�� ���� �� �̵� �ӵ� ����


    // UI ���� ����
    [Header("UI")]
    public Slider staminaSlider; // ���׹̳� �������� ǥ���ϴ� UI �����̴�


    // �׼� ���� ����
    [Header("Actions")]
    [HideInInspector] public bool isRunning = false; // �޸��� ������ ����
    [HideInInspector] public bool isUsingItem = false; // ������ ��� ������ ����
    [HideInInspector] public bool isPickingUpItem = false; // ������ �ݱ� ������ ����
    [HideInInspector] public bool isHoldingTool = false; // ������(����)�� ��� �ִ��� ����


    // ī�޶� ���� ���� ����
    [Header("Camera Look")]
    public Transform cameraFollowTarget; // ī�޶� ȸ���� ��ǥ ����
    public float mouseSensitivity = 100f; // ���콺 ����
    private float xRotation = 0f; // ī�޶��� ���� ȸ�� ��
    private Vector2 lookInput; // ���콺 �Է°�




    // �ʱ�ȭ
    private void Awake()
    {
        playerControls = new PlayerControls();
        stateMachine = GetComponent<PlayerStateMachine>();
        characterController = GetComponent<CharacterController>();
        currentStamina = maxStamina;
        SetupInputActions();

        if (staminaSlider != null)
        {
            staminaSlider.maxValue = maxStamina;
            staminaSlider.value = currentStamina;
        }

        originalWalkSpeed = walkSpeed;
        originalRunSpeed = runSpeed;
    }





    private void Start()
    {
        // ���� ���� �� ���콺 Ŀ���� ȭ�� �߾ӿ� �����ϰ� ����
        Cursor.lockState = CursorLockMode.Locked;
    }



    private void OnEnable()
    {
        // ��ũ��Ʈ Ȱ��ȭ �� �Է� �ý����� Ȱ��ȭ
        playerControls.Enable();
    }



    private void OnDisable()
    {
        // ��ũ��Ʈ ��Ȱ��ȭ �� �Է� �ý����� ��Ȱ��ȭ
        playerControls.Disable();
    }




    // �Է� �̺�Ʈ �Լ�
    // Input Action�� ���� �ݹ� �Լ����� ���
    private void SetupInputActions()
    {
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
        // === ����: ���� ��ȯ ���� �ٷ� ������ ��� ���� ���� ===
        isUsingItem = true;

        if (isHoldingTool && flashlight != null)
        {
            flashlight.Use();
        }
    }

    public void OnUseItemCanceled(InputAction.CallbackContext context)
    {
        // ������ ��� ���� ������ ������Ʈ
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



    //  ������ ������Ʈ �Լ�
    private void Update()
    {

        // ���� ������ OnUpdate() �Լ��� ȣ���Ͽ� ���º� ������ ����
        stateMachine.currentState?.OnUpdate();


        // ī�޶� ȸ���� ó��
        HandleLook();


        // ���׹̳� UI�� ������Ʈ
        UpdateStaminaUI();


        // ���� ���� ������ ��� �� �̻� ������Ʈ�� �������� ����
        if (GameManager.isGameOver)
        {
            return;
        }

    }


    // ������ �ð� �������� ȣ�� (���� ������Ʈ)
    private void FixedUpdate()
    {
        // ���� ������ OnFixedUpdate() �Լ��� ȣ���Ͽ� ���º� ���� ������ ����
        stateMachine.currentState?.OnFixedUpdate();
    }



    // ��� �Լ�
    // �÷��̾� �̵��� ó���մϴ�.
    public void HandleMovement()
    {
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;
        forward.y = 0; // Y�� �̵��� ���� �������� �ߴ� ���� ����
        right.y = 0; // Y�� �̵��� ���� �������� �ߴ� ���� ����
        Vector3 desiredMoveDirection = forward * moveInput.y + right * moveInput.x;
        characterController.Move(desiredMoveDirection.normalized * moveSpeed * Time.fixedDeltaTime);
    }



    // �ν��� ȿ���� Ȱ��ȭ�ϰ� �ӵ��� ������Ŵ
    public void ActivateBoosterEffect(float duration, float multiplier)
    {
        if (!isBoosterActive)
        {
            isBoosterActive = true;
            Debug.Log("�ν��� ȿ���� Ȱ��ȭ�Ǿ����ϴ�. �ӵ��� " + multiplier + "�� �������ϴ�.");

            // �ȱ� �� �޸��� �ӵ� ��ü�� ����
            walkSpeed *= multiplier;
            runSpeed *= multiplier;

            // �ν��� ���� �ð��� ���� �ڷ�ƾ ����
            StartCoroutine(BoosterTimer(duration));
        }
    }


    // �ν��� ȿ���� Ÿ�̸Ӹ� �����ϰ�, �ð��� ������ ���� �ӵ��� �ǵ���
    private IEnumerator BoosterTimer(float duration)
    {
        yield return new WaitForSeconds(duration);

        isBoosterActive = false;
        Debug.Log("�ν��� ȿ���� ����Ǿ����ϴ�. ���� �ӵ��� �����մϴ�.");

        // �ν��� ȿ�� ���� �� ���� �ӵ��� �ǵ���
        walkSpeed = originalWalkSpeed;
        runSpeed = originalRunSpeed;
    }


    // ī�޶� ȸ���� ó��
    private void HandleLook()
    {
        // ���콺 �Է��� ���� ��쿡�� ȸ��
        if (lookInput.sqrMagnitude > 0.001f)
        {
            float mouseX = lookInput.x * mouseSensitivity * Time.deltaTime;
            float mouseY = lookInput.y * mouseSensitivity * Time.deltaTime;

            // Y��(����) ȸ���� �÷��̾� ī�޶󿡸� ����
            transform.Rotate(Vector3.up * mouseX);

            if (cameraFollowTarget != null)
            {
                xRotation -= mouseY;
                xRotation = Mathf.Clamp(xRotation, -90f, 90f); // ī�޶� ȸ�� ���� ����
                cameraFollowTarget.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            }
        }
    }


    // ���׹̳��� �Ҹ��ϴ� �Լ�
    public void UseStamina()
    {
        currentStamina -= staminaDrainRate * Time.fixedDeltaTime;
        if (currentStamina < 0)
        {
            currentStamina = 0;
        }
    }


    // ���׹̳��� ����ϴ� �Լ�
    public void RegenerateStamina()
    {
        // ��ٿ� ���°� �ƴϰ�, ���׹̳��� �ִ�ġ���� ���� ��쿡�� ���
        if (!staminaRegenCooldown && currentStamina < maxStamina)
        {
            currentStamina += staminaRegenRate * Time.fixedDeltaTime;

            if (currentStamina > maxStamina)
            {
                currentStamina = maxStamina;
            }
        }
    }

    // ���׹̳� UI �����̴��� ������Ʈ�ϴ� �Լ�
    private void UpdateStaminaUI()
    {
        if (staminaSlider != null)
        {
            staminaSlider.value = currentStamina;
        }
    }
}