using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

// AI�� ���(�̵�, ����, ���� ���� ��)�� �����ϴ� ��ũ��Ʈ
// AI�� ���� �ӽ��� ����ϴ� ��� �����͸� ����
public class AIController : MonoBehaviour
{

    // ������Ʈ �� ����
    [HideInInspector] public AIStateMachine stateMachine;
    [HideInInspector] public NavMeshAgent agent;
    [HideInInspector] public Animator animator;
    [HideInInspector] public Transform playerTransform; // �÷��̾��� Transform

    // AI�� �÷��̾ ���������� �� ��ġ�� ����
    [HideInInspector] public Vector3 lastKnownPlayerPosition;

    [Header("AI States")]
    public AIPatrolState patrolState;
    public AIChaseState chaseState;
    public AIAlertState alertState;
    public AITimeMoveState timeMoveState;


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
    public float patrolAnimSpeed = 1f; // ���� �ִϸ��̼� �ӵ�


    [Header("Alert")]
    public float alertRotationSpeed = 30f; // ��� ���¿��� ȸ���ϴ� �ӵ�
    public float alertDuration = 5f; // ��� ���¸� �����ϴ� �ð�
    public float alertAnimSpeed = 0.5f; // ��� �ִϸ��̼� �ӵ�


    [Header("Chase")]
    public float chaseSpeed = 5f; // �߰� ������ �̵� �ӵ�
    public float chaseAnimSpeed = 1.5f; // �߰� �ִϸ��̼� �ӵ�


    [Header("Move")]
    public float moveSpeed = 4f; // �̵� ������ �̵� �ӵ�
    public float timedMoveInterval = 5f; // ���� �ð� �̵��� �ֱ�


    private void Awake()
    {
        // Awake()���� �ʼ� ������Ʈ���� ������ ������ �Ҵ�
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        // 'Player' �±׸� ���� ���� ������Ʈ�� ã�� Transform�� �Ҵ�
        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }

        // ������Ʈ�� �Ҵ�Ǿ����� Ȯ�� �� �ӵ� ����
        if (agent != null)
        {
            agent.speed = patrolSpeed;
        }

        alertCheckTimer = alertCheckInterval;

        // ���� �ӽ��� �ʱ�ȭ�մϴ�.
        stateMachine = GetComponent<AIStateMachine>();
        if (stateMachine == null)
        {
            stateMachine = gameObject.AddComponent<AIStateMachine>();
        }
    }

    private void Start()
    {
        // Start()���� ���� �ӽ��� �ʱ�ȭ�ϰ� ù ���¸� �����մϴ�.
        if (stateMachine != null)
        {
            stateMachine.controller = this;
            stateMachine.PatrolState = new AIPatrolState(stateMachine);
            stateMachine.ChaseState = new AIChaseState(stateMachine);
            stateMachine.AlertState = new AIAlertState(stateMachine);
            stateMachine.TimeMoveState = new AITimeMoveState(stateMachine);

            // �ʱ� ���� ����
            stateMachine.SwitchState(stateMachine.PatrolState);
        }
    }


    private void Update()
    {
        // �� �����Ӹ��� ���� ������ Update �Լ��� ȣ���մϴ�.
        if (stateMachine != null && stateMachine.currentState != null)
        {
            stateMachine.currentState.OnUpdate();
        }
    }


    // �÷��̾ �þ� ���� ���� �ִ��� Ȯ���ϴ� �Լ�
    public bool IsPlayerInVision()
    {
        // �÷��̾� Ʈ�������� �Ҵ���� �ʾ����� false ��ȯ
        if (playerTransform == null)
        {
            return false;
        }

        Vector3 directionToPlayer = playerTransform.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        // �Ÿ��� �þ� ���� ���� �ִ��� Ȯ��
        if (distanceToPlayer <= visionRange)
        {
            // �þ߰� ���� �ִ��� Ȯ��
            float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
            if (angleToPlayer <= visionAngle / 2)
            {
                // ��ֹ��� ������ �ִ��� Raycast�� ���� Ȯ��
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
        // ���ӿ��� ���°� �ƴ� ���� ����
        if (GameManager.isGameOver) return;

        // �浹�� ������Ʈ�� �±װ� "Player"���� Ȯ��
        if (other.CompareTag("Player"))
        {
            Debug.Log("AI�� �÷��̾�� �浹�߽��ϴ�. ���� ����!");
            // GameManager �̱��� �ν��Ͻ��� ���� DieGame() �޼��� ȣ��
            // GameManager.Instance�� null�� �ƴ� ���� Die() �Լ��� ȣ���մϴ�.
            if (GameManager.Instance != null)
            {
                GameManager.Instance.Die();
            }
        }
    }

    // AI�� Ư�� ��ǥ �������� �̵��ϵ��� �ϴ� �Լ�
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
            animator.speed = patrolAnimSpeed; // �ִϸ��̼� �ӵ� ����
        }
    }

    public void SetAlertAnimation()
    {
        ResetAllAnimationBools();
        if (animator != null)
        {
            animator.SetBool("IsAlert", true);
            animator.speed = alertAnimSpeed; // �ִϸ��̼� �ӵ� ����
        }
    }

    public void SetChaseAnimation()
    {
        ResetAllAnimationBools();
        if (animator != null)
        {
            animator.SetBool("IsChasing", true);
            animator.speed = chaseAnimSpeed; // �ִϸ��̼� �ӵ� ����
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
}