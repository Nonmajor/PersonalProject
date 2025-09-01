using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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
        // �Է��� ������ �� �� �Լ��� ȣ��ǵ��� ����
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
    }


    
    private void Start()
    {
        // ���콺 Ŀ���� ���� ȭ�鿡 �����ϰ� ����
        Cursor.lockState = CursorLockMode.Locked;
    }

   
    private void OnEnable()
    {
        playerControls.Enable(); // �Է� �ý��� Ȱ��ȭ
    }

    private void OnDisable()
    {
        playerControls.Disable(); // �Է� �ý��� ��Ȱ��ȭ
    }

   




    public void OnMovePerformed(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>(); // WASD �Է� �� �б�
    }

    public void OnMoveCanceled(InputAction.CallbackContext context)
    {
        moveInput = Vector2.zero; // �Է��� ������ 0���� ����
    }

    public void OnRunPerformed(InputAction.CallbackContext context)
    {
        isRunning = true; // �޸��� ��ư ����
    }

    public void OnRunCanceled(InputAction.CallbackContext context)
    {
        isRunning = false; // �޸��� ��ư ��
    }

    public void OnUseItemPerformed(InputAction.CallbackContext context)
    {
        isUsingItem = true; // ������ ��� ��ư ����
    }

    public void OnUseItemCanceled(InputAction.CallbackContext context)
    {
        isUsingItem = false; // ������ ��� ��ư ��
    }

    public void OnPickUpItemPerformed(InputAction.CallbackContext context)
    {
        isPickingUpItem = true; // ������ ȹ�� ��ư ����
    }

    public void OnPickUpItemCanceled(InputAction.CallbackContext context)
    {
        isPickingUpItem = false; // ������ ȹ�� ��ư ��
    }

    private void OnLookPerformed(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>(); // ���콺 �̵� �� �б�
    }

    private void OnLookCanceled(InputAction.CallbackContext context)
    {
        lookInput = Vector2.zero; // ���콺 �Է��� ������ 0���� ����
    }

    





    
    private void Update()
    {
        stateMachine.currentState?.OnUpdate(); // ���� ������ Update �Լ� ȣ��
        HandleLook(); // ī�޶� ȸ�� ó��
        UpdateStaminaUI(); // UI ������Ʈ
    }






    // ������ �ֱ�� ȣ��
    private void FixedUpdate()
    {
        stateMachine.currentState?.OnFixedUpdate(); // ���� ������ FixedUpdate �Լ� ȣ��
    }

    


    public void HandleMovement()
    {
        // �÷��̾ �ٶ󺸴� ������ �������� �̵� ���� ���
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        // Y�� �̵� ����
        forward.y = 0;
        right.y = 0;

        // �Է� ���� ���� ���� �̵� ���� ����
        Vector3 desiredMoveDirection = forward * moveInput.y + right * moveInput.x;

        // CharacterController�� ����Ͽ� ���� �̵� ����
        characterController.Move(desiredMoveDirection * moveSpeed * Time.fixedDeltaTime);
    }




    // ī�޶� ȸ�� ó��
    private void HandleLook()
    {
        // ���콺 �Է��� �̼��ϰԶ� ���� ���� ȸ�� ���� ����
        if (lookInput.sqrMagnitude > 0.001f)
        {
            float mouseX = lookInput.x * mouseSensitivity * Time.deltaTime;
            float mouseY = lookInput.y * mouseSensitivity * Time.deltaTime;

            // �÷��̾� ������Ʈ�� Y��(�¿�) ȸ��
            transform.Rotate(Vector3.up * mouseX);

            // ī�޶��� X��(����) ȸ��
            if (cameraFollowTarget != null)
            {
                xRotation -= mouseY;
                xRotation = Mathf.Clamp(xRotation, -90f, 90f); // ���� ȸ�� ���� ����
                cameraFollowTarget.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            }
        }
    }




    // ���׹̳�
    public void UseStamina()
    {
        currentStamina -= staminaDrainRate * Time.fixedDeltaTime;
        if (currentStamina < 0)
        {
            currentStamina = 0; // ���׹̳��� 0���� �۾����� �ʵ��� ��
        }
    }

    // ���׹̳� ���
    public void RegenerateStamina()
    {
        // ���׹̳� �ִ�ġ���� ���� ���� ���
        if (!staminaRegenCooldown && currentStamina < maxStamina)
        {
            currentStamina += staminaRegenRate * Time.fixedDeltaTime;

            if (currentStamina > maxStamina)
            {
                currentStamina = maxStamina; // �ִ�ġ�� �ʰ����� �ʵ��� ��
            }
        }
    }

    // UI �����̴��� ������Ʈ�ϴ� �Լ�
    private void UpdateStaminaUI()
    {
        // �����̴��� ����Ǿ� ���� ��쿡�� �� ������Ʈ
        if (staminaSlider != null)
        {
            staminaSlider.value = currentStamina;
        }
    }
}