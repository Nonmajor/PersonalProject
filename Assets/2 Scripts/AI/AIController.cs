using System.Collections;
using UnityEngine;
using UnityEngine.AI;

// AI의 기능(이동, 감지, 상태 변수 등)을 제어하는 스크립트
// AI의 상태 머신이 사용하는 모든 데이터를 관리
public class AIController : MonoBehaviour
{

    // 컴포넌트 및 변수
    [HideInInspector] public AIStateMachine stateMachine;
    [HideInInspector] public NavMeshAgent agent;


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


    [Header("Alert")]
    public float alertRotationSpeed = 30f; // 경계 상태에서 회전하는 속도
    public float alertDuration = 5f; // 경계 상태를 유지하는 시간


    [Header("Chase")]
    public float chaseSpeed = 7f; // 추격 상태의 이동 속도


    [Header("Timed Location Sync")]
    public float syncPlayerLocationInterval = 60f; // 1분마다 위치를 동기화할 주기
    public Vector3 lastKnownPlayerPosition; // 마지막으로 저장된 플레이어의 위치


    [Header("Animation")]
    public Animator animator;



    // 참조 변수
    public Transform player; // 플레이어 오브젝트에 대한 참조
    public GameObject deadUI; // 게임 오버 UI에 대한 참조



    
    private void Awake()
    {
        stateMachine = GetComponent<AIStateMachine>();
        agent = GetComponent<NavMeshAgent>();

        // "Player" 태그를 가진 오브젝트를 찾아 참조
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogWarning("'Player' 태그를 가진 오브젝트를 찾을 수 없습니다.");
        }

        // 타이머 초기화
        alertCheckTimer = alertCheckInterval;
        animator = GetComponent<Animator>();
    }

    
    private void Start()
    {
        // 1분마다 플레이어 위치를 동기화하는 코루틴 시작
        StartCoroutine(SyncPlayerLocationRoutine());
    }

    
    

    // 1분마다 플레이어의 위치를 동기화하는 코루틴
    private IEnumerator SyncPlayerLocationRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(syncPlayerLocationInterval);

            // 현재 상태가 순찰(Patrol) 또는 경계(Alert) 상태일 때만 위치를 동기화
            // 이미 플레이어를 추격하거나 데드 상태일 때는 건너뜀
            if (stateMachine.currentState == stateMachine.PatrolState ||
                stateMachine.currentState == stateMachine.AlertState)
            {
                lastKnownPlayerPosition = player.position;
                stateMachine.SwitchState(stateMachine.TimedMoveState);
            }
        }
    }

    
    // 플레이어가 AI의 시야 범위에 있는지 확인하는 함수
    public bool IsPlayerInVision()
    {
        if (player == null) return false;

        Vector3 directionToPlayer = player.position - transform.position;
        float distance = directionToPlayer.magnitude;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);

        // 거리가 시야 범위 내이고, 각도가 시야각 내에 있으면 true를 반환
        if (distance <= visionRange && angle <= visionAngle / 2f)
        {
            // Raycast를 사용하여 시야를 가로막는 장애물이 있는지 확인
            if (Physics.Raycast(transform.position, directionToPlayer, out RaycastHit hit, distance))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    return true;
                }
            }
        }
        return false;
    }

    // AI를 특정 목표 지점으로 이동시키는 함수
    public void MoveTo(Vector3 targetPosition)
    {
        // NavMeshAgent가 활성화된 경우에만 목적지를 설정
        if (agent != null && agent.isActiveAndEnabled)
        {
            agent.SetDestination(targetPosition);
        }
    }

    // AI가 다른 오브젝트와 충돌했을 때 호출되는 함수 (Is Trigger가 활성화된 콜라이더)
    void OnTriggerEnter(Collider other)
    {
        // 충돌한 오브젝트의 태그가 "Player"인지 확인
        if (other.CompareTag("Player"))
        {
            // GameManager의 Die() 함수를 직접 호출
            if (GameManager.Instance != null)
            {
                GameManager.Instance.Die();
            }

            
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

    private void ResetAllAnimationBools()
    {
        // 모든 bool 파라미터를 false로 초기화
        animator.SetBool("IsPatrolling", false);
        animator.SetBool("IsAlert", false);
        animator.SetBool("IsChasing", false);
    }

    public void SetPatrolAnimation()
    {
        ResetAllAnimationBools();
        if (animator != null)
        {
            animator.SetBool("IsPatrolling", true);
        }
    }

    public void SetAlertAnimation()
    {
        ResetAllAnimationBools();
        if (animator != null)
        {
            animator.SetBool("IsAlert", true);
        }
    }

    public void SetChaseAnimation()
    {
        ResetAllAnimationBools();
        if (animator != null)
        {
            animator.SetBool("IsChasing", true);
        }
    }
}