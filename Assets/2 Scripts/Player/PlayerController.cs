using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;
using Player.State;

// 플레이어의 이동, 조작, 스테미나, 부스터, 카메라 등을 총괄
public class PlayerController : MonoBehaviour
{

    // 손전등 스크립트 참조 변수
    [Header("Item")]
    public FlashlightHandler flashlight;

    // === 추가: 발소리 관련 변수 ===
    [Header("Footsteps")]
    public float walkFootstepInterval = 0.5f; // 걷기 발소리 재생 간격 (초)
    public float runFootstepInterval = 0.3f; // 달리기 발소리 재생 간격 (초)
    [HideInInspector] public bool canPlayFootstep = true; // 발소리 재생 가능 여부
    // ===

    // 컴포넌트 및 참조 변수
    private PlayerControls playerControls; // 유니티 Input System을 통한 플레이어 입력 제어
    private PlayerStateMachine stateMachine; // 플레이어 상태 관리 (State Machine)
    private CharacterController characterController; // 플레이어 이동 처리를 위한 캐릭터 컨트롤러 컴포넌트


    // 이동 관련 변수
    [Header("Movement")]
    public float walkSpeed = 5f; // 걷기 속도
    public float runSpeed = 10f; // 달리기 속도
    public float moveSpeed = 0f; // 현재 이동 속도 (상태에 따라 변경됨)
    [HideInInspector] public Vector2 moveInput; // WASD 키의 이동 입력값


    // 부스터 관련 변수
    [Header("Booster")]
    [HideInInspector] public bool isBoosterActive = false; // 부스터 활성화 여부
    private float originalWalkSpeed; // 부스터 사용 전 원래 걷기 속도
    private float originalRunSpeed; // 부스터 사용 전 원래 달리기 속도


    // 스테미나 관련 변수
    [Header("Stamina")]
    public float maxStamina = 100f; // 최대 스테미나
    public float currentStamina; // 현재 스테미나
    public float staminaDrainRate = 10f; // 초당 스테미나 소모량
    public float staminaRegenRate = 5f; // 초당 스테미나 재생량
    [HideInInspector] public bool staminaRegenCooldown = false; // 스테미나 재생 쿨다운 여부


    // 탈진 상태 관련 변수
    [Header("Exhausted State")]
    public float exhaustedDuration = 5f; // 탈진 상태 유지 시간 (초)
    [Range(0.1f, 1f)]
    public float exhaustedSpeedMultiplier = 0.5f; // 탈진 상태 시 이동 속도 배율


    // UI 관련 변수
    [Header("UI")]
    public Slider staminaSlider; // 스테미나 게이지를 표시하는 UI 슬라이더


    // 액션 상태 변수
    [Header("Actions")]
    [HideInInspector] public bool isRunning = false; // 달리기 중인지 여부
    [HideInInspector] public bool isUsingItem = false; // 아이템 사용 중인지 여부
    [HideInInspector] public bool isPickingUpItem = false; // 아이템 줍기 중인지 여부
    [HideInInspector] public bool isHoldingTool = false; // 손전등(도구)를 들고 있는지 여부


    // 카메라 조작 관련 변수
    [Header("Camera Look")]
    public Transform cameraFollowTarget; // 카메라가 회전할 목표 지점
    public float mouseSensitivity = 100f; // 마우스 감도
    private float xRotation = 0f; // 카메라의 상하 회전 값
    private Vector2 lookInput; // 마우스 입력값




    // 초기화
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
        // 게임 시작 시 마우스 커서를 화면 중앙에 고정하고 숨김
        Cursor.lockState = CursorLockMode.Locked;
    }



    private void OnEnable()
    {
        // 스크립트 활성화 시 입력 시스템을 활성화
        playerControls.Enable();
    }



    private void OnDisable()
    {
        // 스크립트 비활성화 시 입력 시스템을 비활성화
        playerControls.Disable();
    }




    // 입력 이벤트 함수
    // Input Action에 대한 콜백 함수들을 등록
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
        // === 수정: 상태 전환 없이 바로 아이템 사용 로직 실행 ===
        isUsingItem = true;

        if (isHoldingTool && flashlight != null)
        {
            flashlight.Use();
        }
    }

    public void OnUseItemCanceled(InputAction.CallbackContext context)
    {
        // 아이템 사용 상태 변수만 업데이트
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



    //  프레임 업데이트 함수
    private void Update()
    {

        // 현재 상태의 OnUpdate() 함수를 호출하여 상태별 로직을 실행
        stateMachine.currentState?.OnUpdate();


        // 카메라 회전을 처리
        HandleLook();


        // 스테미나 UI를 업데이트
        UpdateStaminaUI();


        // 게임 오버 상태일 경우 더 이상 업데이트를 진행하지 않음
        if (GameManager.isGameOver)
        {
            return;
        }

    }


    // 고정된 시간 간격으로 호출 (물리 업데이트)
    private void FixedUpdate()
    {
        // 현재 상태의 OnFixedUpdate() 함수를 호출하여 상태별 물리 로직을 실행
        stateMachine.currentState?.OnFixedUpdate();
    }



    // 기능 함수
    // 플레이어 이동을 처리합니다.
    public void HandleMovement()
    {
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;
        forward.y = 0; // Y축 이동을 막아 공중으로 뜨는 것을 방지
        right.y = 0; // Y축 이동을 막아 공중으로 뜨는 것을 방지
        Vector3 desiredMoveDirection = forward * moveInput.y + right * moveInput.x;
        characterController.Move(desiredMoveDirection.normalized * moveSpeed * Time.fixedDeltaTime);
    }



    // 부스터 효과를 활성화하고 속도를 증가시킴
    public void ActivateBoosterEffect(float duration, float multiplier)
    {
        if (!isBoosterActive)
        {
            isBoosterActive = true;
            Debug.Log("부스터 효과가 활성화되었습니다. 속도가 " + multiplier + "배 빨라집니다.");

            // 걷기 및 달리기 속도 자체를 수정
            walkSpeed *= multiplier;
            runSpeed *= multiplier;

            // 부스터 지속 시간을 위한 코루틴 시작
            StartCoroutine(BoosterTimer(duration));
        }
    }


    // 부스터 효과의 타이머를 관리하고, 시간이 지나면 원래 속도로 되돌림
    private IEnumerator BoosterTimer(float duration)
    {
        yield return new WaitForSeconds(duration);

        isBoosterActive = false;
        Debug.Log("부스터 효과가 종료되었습니다. 원래 속도로 복귀합니다.");

        // 부스터 효과 종료 후 원래 속도로 되돌림
        walkSpeed = originalWalkSpeed;
        runSpeed = originalRunSpeed;
    }


    // 카메라 회전을 처리
    private void HandleLook()
    {
        // 마우스 입력이 있을 경우에만 회전
        if (lookInput.sqrMagnitude > 0.001f)
        {
            float mouseX = lookInput.x * mouseSensitivity * Time.deltaTime;
            float mouseY = lookInput.y * mouseSensitivity * Time.deltaTime;

            // Y축(상하) 회전은 플레이어 카메라에만 적용
            transform.Rotate(Vector3.up * mouseX);

            if (cameraFollowTarget != null)
            {
                xRotation -= mouseY;
                xRotation = Mathf.Clamp(xRotation, -90f, 90f); // 카메라 회전 범위 제한
                cameraFollowTarget.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            }
        }
    }


    // 스테미나를 소모하는 함수
    public void UseStamina()
    {
        currentStamina -= staminaDrainRate * Time.fixedDeltaTime;
        if (currentStamina < 0)
        {
            currentStamina = 0;
        }
    }


    // 스테미나를 재생하는 함수
    public void RegenerateStamina()
    {
        // 쿨다운 상태가 아니고, 스테미나가 최대치보다 적을 경우에만 재생
        if (!staminaRegenCooldown && currentStamina < maxStamina)
        {
            currentStamina += staminaRegenRate * Time.fixedDeltaTime;

            if (currentStamina > maxStamina)
            {
                currentStamina = maxStamina;
            }
        }
    }

    // 스테미나 UI 슬라이더를 업데이트하는 함수
    private void UpdateStaminaUI()
    {
        if (staminaSlider != null)
        {
            staminaSlider.value = currentStamina;
        }
    }
}