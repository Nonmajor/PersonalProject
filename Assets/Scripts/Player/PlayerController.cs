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
    public float moveSpeed = 0f; // 플레이어의 현재 이동 속도 (상태에 따라 변함)
    [HideInInspector] public Vector2 moveInput; // WASD 키 입력 값 (Vector2)


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
        // 입력이 들어왔을 때 각 함수가 호출되도록 설정
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
    }


    
    private void Start()
    {
        // 마우스 커서를 게임 화면에 고정하고 숨김
        Cursor.lockState = CursorLockMode.Locked;
    }

   
    private void OnEnable()
    {
        playerControls.Enable(); // 입력 시스템 활성화
    }

    private void OnDisable()
    {
        playerControls.Disable(); // 입력 시스템 비활성화
    }

   




    public void OnMovePerformed(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>(); // WASD 입력 값 읽기
    }

    public void OnMoveCanceled(InputAction.CallbackContext context)
    {
        moveInput = Vector2.zero; // 입력이 없으면 0으로 설정
    }

    public void OnRunPerformed(InputAction.CallbackContext context)
    {
        isRunning = true; // 달리기 버튼 누름
    }

    public void OnRunCanceled(InputAction.CallbackContext context)
    {
        isRunning = false; // 달리기 버튼 뗌
    }

    public void OnUseItemPerformed(InputAction.CallbackContext context)
    {
        isUsingItem = true; // 아이템 사용 버튼 누름
    }

    public void OnUseItemCanceled(InputAction.CallbackContext context)
    {
        isUsingItem = false; // 아이템 사용 버튼 뗌
    }

    public void OnPickUpItemPerformed(InputAction.CallbackContext context)
    {
        isPickingUpItem = true; // 아이템 획득 버튼 누름
    }

    public void OnPickUpItemCanceled(InputAction.CallbackContext context)
    {
        isPickingUpItem = false; // 아이템 획득 버튼 뗌
    }

    private void OnLookPerformed(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>(); // 마우스 이동 값 읽기
    }

    private void OnLookCanceled(InputAction.CallbackContext context)
    {
        lookInput = Vector2.zero; // 마우스 입력이 없으면 0으로 설정
    }

    





    
    private void Update()
    {
        stateMachine.currentState?.OnUpdate(); // 현재 상태의 Update 함수 호출
        HandleLook(); // 카메라 회전 처리
        UpdateStaminaUI(); // UI 업데이트
    }






    // 일정한 주기로 호출
    private void FixedUpdate()
    {
        stateMachine.currentState?.OnFixedUpdate(); // 현재 상태의 FixedUpdate 함수 호출
    }

    


    public void HandleMovement()
    {
        // 플레이어가 바라보는 방향을 기준으로 이동 방향 계산
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        // Y축 이동 방지
        forward.y = 0;
        right.y = 0;

        // 입력 값에 따라 최종 이동 방향 결정
        Vector3 desiredMoveDirection = forward * moveInput.y + right * moveInput.x;

        // CharacterController를 사용하여 실제 이동 적용
        characterController.Move(desiredMoveDirection * moveSpeed * Time.fixedDeltaTime);
    }




    // 카메라 회전 처리
    private void HandleLook()
    {
        // 마우스 입력이 미세하게라도 있을 때만 회전 로직 실행
        if (lookInput.sqrMagnitude > 0.001f)
        {
            float mouseX = lookInput.x * mouseSensitivity * Time.deltaTime;
            float mouseY = lookInput.y * mouseSensitivity * Time.deltaTime;

            // 플레이어 오브젝트의 Y축(좌우) 회전
            transform.Rotate(Vector3.up * mouseX);

            // 카메라의 X축(상하) 회전
            if (cameraFollowTarget != null)
            {
                xRotation -= mouseY;
                xRotation = Mathf.Clamp(xRotation, -90f, 90f); // 상하 회전 범위 제한
                cameraFollowTarget.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            }
        }
    }




    // 스테미나
    public void UseStamina()
    {
        currentStamina -= staminaDrainRate * Time.fixedDeltaTime;
        if (currentStamina < 0)
        {
            currentStamina = 0; // 스테미나가 0보다 작아지지 않도록 함
        }
    }

    // 스테미나 재생
    public void RegenerateStamina()
    {
        // 스테미나 최대치보다 작을 때만 재생
        if (!staminaRegenCooldown && currentStamina < maxStamina)
        {
            currentStamina += staminaRegenRate * Time.fixedDeltaTime;

            if (currentStamina > maxStamina)
            {
                currentStamina = maxStamina; // 최대치를 초과하지 않도록 함
            }
        }
    }

    // UI 슬라이더를 업데이트하는 함수
    private void UpdateStaminaUI()
    {
        // 슬라이더가 연결되어 있을 경우에만 값 업데이트
        if (staminaSlider != null)
        {
            staminaSlider.value = currentStamina;
        }
    }
}