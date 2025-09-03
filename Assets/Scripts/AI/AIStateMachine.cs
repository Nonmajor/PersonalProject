using UnityEngine;

// AI�� ���¸� ����
// ��� AI ���¸� �����ϰ�, ���� ��ȯ�� ���
public class AIStateMachine : MonoBehaviour
{
    // ���� AI�� ����
    public AIState currentState;

    // AI�� ���� �� �ִ� ��� ���� �ν��Ͻ�
    public AIPatrolState PatrolState;
    public AIChaseState ChaseState;
    public AIAlertState AlertState;
    public AITimedMoveState TimedMoveState;
    

    // AI�� ��ɰ� �����͸� ��� �ִ� ��Ʈ�ѷ��� ���� ����
    [HideInInspector] public AIController controller;

   
    private void Awake()
    {
        // ���� ������Ʈ�� �ִ� AIController ������Ʈ�� ������
        controller = GetComponent<AIController>();


        // �� ���� Ŭ������ �ν��Ͻ��� �����ϰ� ���� �ӽſ� ���� ������ ����
        PatrolState = new AIPatrolState(this);
        ChaseState = new AIChaseState(this);
        AlertState = new AIAlertState(this);
        TimedMoveState = new AITimedMoveState(this);
        
    }

    
    private void Start()
    {
        // ���� ���� �� TimeScale�� 1�� �ʱ�ȭ�Ͽ� �ð��� �帣�� ��
        Time.timeScale = 1f;

        // AI�� �ʱ� ���¸� ����(Patrol) ���·� ����
        SwitchState(PatrolState);
    }

    
    private void Update()
    {

        // ���� ������ OnUpdate() �Լ��� ȣ���Ͽ� ���º� ������ ����
        // `?.` �����ڴ� null üũ�� ���� �����ϰ� �Լ��� ȣ��
        currentState?.OnUpdate();

    }

    
    private void FixedUpdate()
    {
        // ���� ������ OnFixedUpdate() �Լ��� ȣ��
        currentState?.OnFixedUpdate();
    }


    // ���ο� ���·� ��ȯ�ϴ� �ٽ� �Լ�
    public void SwitchState(AIState newState)
    {
        // ���� ���°� �ִٸ�, �ش� ������ OnExit() �Լ��� ȣ���Ͽ� ���¸� ����
        currentState?.OnExit();

        // ���� ���¸� ���ο� ���·� ��ü
        currentState = newState;

        // ���ο� ������ OnEnter() �Լ��� ȣ���Ͽ� ���¸� ����
        currentState?.OnEnter();
    }
}