using System.Collections;
using UnityEngine;
using UnityEngine.AI;

// AI�� ���(�̵�, ����, ���� ���� ��)�� �����ϴ� ��ũ��Ʈ
// AI�� ���� �ӽ��� ����ϴ� ��� �����͸� ����
public class AIController : MonoBehaviour
{

    // ������Ʈ �� ����
    [HideInInspector] public AIStateMachine stateMachine;
    [HideInInspector] public NavMeshAgent agent;


    [Header("Vision")]
    public float visionRange = 10f; // AI�� �þ�(����) ����
    public float visionAngle = 90f; // AI�� �þ߰�(���� ���)
    public float pursuitTime = 10f; // �÷��̾ ��ģ �� �߰��� �����ϴ� �ð�
    [HideInInspector] public float pursuitTimer; // ���� �ð��� �����ϴ� Ÿ�̸�


    [Header("Patrol")]
    public float patrolSpeed = 3.5f; // ���� ������ �̵� �ӵ�
    public float patrolRadius = 20f; // ������ �� �ִ� �ִ� �ݰ�
    public float alertCheckInterval = 10f; // ��� ���·� ��ȯ�� �ֱ�
    public float alertCheckTimer; // ��� ���� ��ȯ �ֱ⸦ �����ϴ� Ÿ�̸�


    [Header("Alert")]
    public float alertRotationSpeed = 30f; // ��� ���¿��� ȸ���ϴ� �ӵ�
    public float alertDuration = 5f; // ��� ���¸� �����ϴ� �ð�


    [Header("Chase")]
    public float chaseSpeed = 7f; // �߰� ������ �̵� �ӵ�


    [Header("Timed Location Sync")]
    public float syncPlayerLocationInterval = 60f; // 1�и��� ��ġ�� ����ȭ�� �ֱ�
    public Vector3 lastKnownPlayerPosition; // ���������� ����� �÷��̾��� ��ġ


    [Header("Animation")]
    public Animator animator;



    // ���� ����
    public Transform player; // �÷��̾� ������Ʈ�� ���� ����
    public GameObject deadUI; // ���� ���� UI�� ���� ����



    
    private void Awake()
    {
        stateMachine = GetComponent<AIStateMachine>();
        agent = GetComponent<NavMeshAgent>();

        // "Player" �±׸� ���� ������Ʈ�� ã�� ����
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogWarning("'Player' �±׸� ���� ������Ʈ�� ã�� �� �����ϴ�.");
        }

        // Ÿ�̸� �ʱ�ȭ
        alertCheckTimer = alertCheckInterval;
        animator = GetComponent<Animator>();
    }

    
    private void Start()
    {
        // 1�и��� �÷��̾� ��ġ�� ����ȭ�ϴ� �ڷ�ƾ ����
        StartCoroutine(SyncPlayerLocationRoutine());
    }

    
    

    // 1�и��� �÷��̾��� ��ġ�� ����ȭ�ϴ� �ڷ�ƾ
    private IEnumerator SyncPlayerLocationRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(syncPlayerLocationInterval);

            // ���� ���°� ����(Patrol) �Ǵ� ���(Alert) ������ ���� ��ġ�� ����ȭ
            // �̹� �÷��̾ �߰��ϰų� ���� ������ ���� �ǳʶ�
            if (stateMachine.currentState == stateMachine.PatrolState ||
                stateMachine.currentState == stateMachine.AlertState)
            {
                lastKnownPlayerPosition = player.position;
                stateMachine.SwitchState(stateMachine.TimedMoveState);
            }
        }
    }

    
    // �÷��̾ AI�� �þ� ������ �ִ��� Ȯ���ϴ� �Լ�
    public bool IsPlayerInVision()
    {
        if (player == null) return false;

        Vector3 directionToPlayer = player.position - transform.position;
        float distance = directionToPlayer.magnitude;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);

        // �Ÿ��� �þ� ���� ���̰�, ������ �þ߰� ���� ������ true�� ��ȯ
        if (distance <= visionRange && angle <= visionAngle / 2f)
        {
            // Raycast�� ����Ͽ� �þ߸� ���θ��� ��ֹ��� �ִ��� Ȯ��
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

    // AI�� Ư�� ��ǥ �������� �̵���Ű�� �Լ�
    public void MoveTo(Vector3 targetPosition)
    {
        // NavMeshAgent�� Ȱ��ȭ�� ��쿡�� �������� ����
        if (agent != null && agent.isActiveAndEnabled)
        {
            agent.SetDestination(targetPosition);
        }
    }

    // AI�� �ٸ� ������Ʈ�� �浹���� �� ȣ��Ǵ� �Լ� (Is Trigger�� Ȱ��ȭ�� �ݶ��̴�)
    void OnTriggerEnter(Collider other)
    {
        // �浹�� ������Ʈ�� �±װ� "Player"���� Ȯ��
        if (other.CompareTag("Player"))
        {
            // GameManager�� Die() �Լ��� ���� ȣ��
            if (GameManager.Instance != null)
            {
                GameManager.Instance.Die();
            }

            
        }
    }

    // AI�� �þ� ������ �� �信�� �ð������� �����ִ� ����� �Լ�
    private void OnDrawGizmos()
    {
        // �þ� �ݰ��� ����� ���̾� ���� ǥ��
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRange);

        // �þ߰��� û�ϻ� ������ ǥ��
        Gizmos.color = Color.cyan;
        Vector3 forward = transform.forward;
        Vector3 leftDirection = Quaternion.Euler(0, -visionAngle / 2f, 0) * forward;
        Vector3 rightDirection = Quaternion.Euler(0, visionAngle / 2f, 0) * forward;

        Gizmos.DrawRay(transform.position, leftDirection * visionRange);
        Gizmos.DrawRay(transform.position, rightDirection * visionRange);
    }

    private void ResetAllAnimationBools()
    {
        // ��� bool �Ķ���͸� false�� �ʱ�ȭ
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