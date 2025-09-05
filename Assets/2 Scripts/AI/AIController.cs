using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

// AI의 기능(이동, 감지, 상태 변수 등)을 제어하는 스크립트
// AI의 상태 머신이 사용하는 모든 데이터를 관리
public class AIController : MonoBehaviour
{

    // 컴포넌트 및 변수
    [HideInInspector] public AIStateMachine stateMachine;
    [HideInInspector] public NavMeshAgent agent;
    [HideInInspector] public Animator animator;
    [HideInInspector] public Transform playerTransform; // 플레이어의 Transform

    // AI가 플레이어를 마지막으로 본 위치를 저장
    [HideInInspector] public Vector3 lastKnownPlayerPosition;

    [Header("AI States")]
    public AIPatrolState patrolState;
    public AIChaseState chaseState;
    public AIAlertState alertState;
    public AITimeMoveState timeMoveState;


    [Header("Vision")]
    public float visionRange = 10f; // AI의 시야(감지) 범위
    public float visionAngle = 90f; // AI의 시야각(원뿔 모양)
    public float pursuitTime = 10f; // 플레이어를 놓친 후 추격을 지속하는 시간
    [HideInInspector] public float pursuitTimer; // 추적 시간을 측정하는 타이머


    [Header("Patrol")]
    public float patrolSpeed = 3.5f; // 순찰 상태의 이동 속도
    public float patrolRadius = 20f; // 순찰할 수 있는 최대 반경
    public float alertCheckInterval = 10f; // 경계 상태로 전환할 주기
    public float alertCheckTimer; // 경계 상태 전환 주기를 측정하는 타이머
    public float patrolAnimSpeed = 1f; // 순찰 애니메이션 속도


    [Header("Alert")]
    public float alertRotationSpeed = 30f; // 경계 상태에서 회전하는 속도
    public float alertDuration = 5f; // 경계 상태를 유지하는 시간
    public float alertAnimSpeed = 0.5f; // 경계 애니메이션 속도


    [Header("Chase")]
    public float chaseSpeed = 5f; // 추격 상태의 이동 속도
    public float chaseAnimSpeed = 1.5f; // 추격 애니메이션 속도


    [Header("Move")]
    public float moveSpeed = 4f; // 이동 상태의 이동 속도
    public float timedMoveInterval = 5f; // 일정 시간 이동의 주기


    private void Awake()
    {
        // Awake()에서 필수 컴포넌트들을 가져와 변수에 할당
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        // 'Player' 태그를 가진 게임 오브젝트를 찾아 Transform을 할당
        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }

        // 컴포넌트가 할당되었는지 확인 후 속도 설정
        if (agent != null)
        {
            agent.speed = patrolSpeed;
        }

        alertCheckTimer = alertCheckInterval;

        // 상태 머신을 초기화합니다.
        stateMachine = GetComponent<AIStateMachine>();
        if (stateMachine == null)
        {
            stateMachine = gameObject.AddComponent<AIStateMachine>();
        }
    }

    private void Start()
    {
        // Start()에서 상태 머신을 초기화하고 첫 상태를 설정합니다.
        if (stateMachine != null)
        {
            stateMachine.controller = this;
            stateMachine.PatrolState = new AIPatrolState(stateMachine);
            stateMachine.ChaseState = new AIChaseState(stateMachine);
            stateMachine.AlertState = new AIAlertState(stateMachine);
            stateMachine.TimeMoveState = new AITimeMoveState(stateMachine);

            // 초기 상태 설정
            stateMachine.SwitchState(stateMachine.PatrolState);
        }
    }


    private void Update()
    {
        // 매 프레임마다 현재 상태의 Update 함수를 호출합니다.
        if (stateMachine != null && stateMachine.currentState != null)
        {
            stateMachine.currentState.OnUpdate();
        }
    }


    // 플레이어가 시야 범위 내에 있는지 확인하는 함수
    public bool IsPlayerInVision()
    {
        // 플레이어 트랜스폼이 할당되지 않았으면 false 반환
        if (playerTransform == null)
        {
            return false;
        }

        Vector3 directionToPlayer = playerTransform.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        // 거리가 시야 범위 내에 있는지 확인
        if (distanceToPlayer <= visionRange)
        {
            // 시야각 내에 있는지 확인
            float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
            if (angleToPlayer <= visionAngle / 2)
            {
                // 장애물로 가려져 있는지 Raycast를 통해 확인
                RaycastHit hit;
                if (Physics.Raycast(transform.position, directionToPlayer, out hit, visionRange))
                {
                    if (hit.collider.CompareTag("Player"))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        // 게임오버 상태가 아닐 때만 실행
        if (GameManager.isGameOver) return;

        // 충돌한 오브젝트의 태그가 "Player"인지 확인
        if (other.CompareTag("Player"))
        {
            Debug.Log("AI가 플레이어와 충돌했습니다. 게임 오버!");
            // GameManager 싱글톤 인스턴스를 통해 DieGame() 메서드 호출
            // GameManager.Instance가 null이 아닐 때만 Die() 함수를 호출합니다.
            if (GameManager.Instance != null)
            {
                GameManager.Instance.Die();
            }
        }
    }

    // AI가 특정 목표 지점으로 이동하도록 하는 함수
    public void MoveTo(Vector3 destination)
    {
        if (agent != null && agent.isActiveAndEnabled)
        {
            agent.SetDestination(destination);
        }
    }


    public void ResetAllAnimationBools()
    {
        if (animator != null)
        {
            animator.SetBool("IsPatrolling", false);
            animator.SetBool("IsAlert", false);
            animator.SetBool("IsChasing", false);
        }
    }

    public void SetPatrolAnimation()
    {
        ResetAllAnimationBools();
        if (animator != null)
        {
            animator.SetBool("IsPatrolling", true);
            animator.speed = patrolAnimSpeed; // 애니메이션 속도 조절
        }
    }

    public void SetAlertAnimation()
    {
        ResetAllAnimationBools();
        if (animator != null)
        {
            animator.SetBool("IsAlert", true);
            animator.speed = alertAnimSpeed; // 애니메이션 속도 조절
        }
    }

    public void SetChaseAnimation()
    {
        ResetAllAnimationBools();
        if (animator != null)
        {
            animator.SetBool("IsChasing", true);
            animator.speed = chaseAnimSpeed; // 애니메이션 속도 조절
        }
    }


    // AI의 시야 범위를 씬 뷰에서 시각적으로 보여주는 디버깅 함수
    private void OnDrawGizmos()
    {
        // 시야 반경을 노란색 와이어 구로 표시
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRange);

        // 시야각을 청록색 선으로 표시
        Gizmos.color = Color.cyan;
        Vector3 forward = transform.forward;
        Vector3 leftDirection = Quaternion.Euler(0, -visionAngle / 2f, 0) * forward;
        Vector3 rightDirection = Quaternion.Euler(0, visionAngle / 2f, 0) * forward;

        Gizmos.DrawRay(transform.position, leftDirection * visionRange);
        Gizmos.DrawRay(transform.position, rightDirection * visionRange);
    }
}