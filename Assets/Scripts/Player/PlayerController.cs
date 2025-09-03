using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections; // Coroutine을 사용하기 위해 추가

public class PlayerController : MonoBehaviour
{
    private PlayerControls playerControls;
    private PlayerStateMachine stateMachine;
    private CharacterController characterController;

    // Movement
    [Header("Movement")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float moveSpeed = 0f; // 플레이어의 현재 이동 속도 (상태에 따라 변함)
    [HideInInspector] public Vector2 moveInput; // WASD 키 입력 값 (Vector2)

    // Booster
    [Header("Booster")]
    [HideInInspector] public bool isBoosterActive = false; // 부스터 활성화 여부
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
    [HideInInspector] public bool staminaRegenCooldown = false; // 스테미나 재생 쿨다운 여부

    // Exhausted (스테미나 고갈 시 탈진)
    [Header("Exhausted State")]
    public float exhaustedDuration = 5f; // 탈진 상태 지속 시간 (초)
    [Range(0.1f, 1f)] // 인스펙터에서 조절 가능
    public float exhaustedSpeedMultiplier = 0.5f; // 탈진 상태에서 이동 속도 배율

    // UI
    [Header("UI")]
    public Slider staminaSlider; // 스테미나 게이지 UI 슬라이더

    // Actions
    [Header("Actions")]
    [HideInInspector] public bool isRunning = false; // 달리기 중인지 여부
    [HideInInspector] public bool isUsingItem = false; // 아이템 사용 중인지 여부
    [HideInInspector] public bool isPickingUpItem = false; // 아이템 획득 중인지 여부

    // Camera Look
    [Header("Camera Look")]
    public Transform cameraFollowTarget; // 카메라가 따라갈 빈 오브젝트
    public float mouseSensitivity = 100f; // 마우스 감도
    private float xRotation = 0f; // 상하 회전 값
    private Vector2 lookInput; // 마우스 이동 입력 값 (Delta)

    private void Awake()
    {
        // 필요한 컴포넌트 및 Input System 인스턴스 초기화
        playerControls = new PlayerControls();
        stateMachine = GetComponent<PlayerStateMachine>();
        characterController = GetComponent<CharacterController>();

        // 현재 스테미나를 최대로 설정
        currentStamina = maxStamina;

        // Input System 이벤트 등록
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

        // 슬라이더 UI의 최대값 및 초기값 설정
        if (staminaSlider != null)
        {
            staminaSlider.maxValue = maxStamina;
            staminaSlider.value = currentStamina;
        }

        // 부스터 효과를 위한 원래 속도 저장
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

    // 이동 처리
    public void HandleMovement()
    {
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        forward.y = 0;
        right.y = 0;

        Vector3 desiredMoveDirection = forward * moveInput.y + right * moveInput.x;

        // 부스터가 활성화된 경우, moveSpeed를 직접 설정
        if (isBoosterActive)
        {
            moveSpeed = isRunning ? runSpeed * speedMultiplier : walkSpeed * speedMultiplier;
        }
        else
        {
            // 부스터가 비활성화된 경우, 원래 속도로 moveSpeed를 설정
            moveSpeed = isRunning ? runSpeed : walkSpeed;
        }

        characterController.Move(desiredMoveDirection * moveSpeed * Time.fixedDeltaTime);
    }

    // 부스터 효과를 활성화하는 함수
    public void ActivateBoosterEffect(float duration, float multiplier)
    {
        if (isBoosterActive) return;

        isBoosterActive = true;
        boosterDuration = duration;
        speedMultiplier = multiplier;
        StartCoroutine(BoosterTimer(duration));

        Debug.Log("부스터 효과가 시작되었습니다. 이동 속도 2배!");
    }

    // 부스터 지속 시간 코루틴
    private IEnumerator BoosterTimer(float duration)
    {
        yield return new WaitForSeconds(duration);

        isBoosterActive = false;

        Debug.Log("부스터 효과가 종료되었습니다. 원래 속도로 복귀합니다.");
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