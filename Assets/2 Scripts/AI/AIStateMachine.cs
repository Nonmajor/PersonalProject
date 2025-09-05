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
    public AITimeMoveState TimeMoveState;

    // AI�� ��ɰ� �����͸� ��� �ִ� ��Ʈ�ѷ��� ���� ����
    [HideInInspector] public AIController controller;

    private void Awake()
    {
        // ���� ���� �� TimeScale�� 1�� �ʱ�ȭ�Ͽ� �ð��� �帣�� ��
        Time.timeScale = 1f;
    }


    // AIStateMachine�� �ʱ�ȭ�ϴ� �޼��� (AIController���� ȣ��)
    public void Initialize(AIState startingState)
    {
        PatrolState = new AIPatrolState(this);
        ChaseState = new AIChaseState(this);
        AlertState = new AIAlertState(this);
        TimeMoveState = new AITimeMoveState(this);

        // AI�� �ʱ� ���¸� ����
        SwitchState(startingState);
    }

    private void Update()
    {
        // ���� ������ OnUpdate() �Լ��� ȣ���Ͽ� ���º� ������ ����
        currentState?.OnUpdate();
    }


    private void FixedUpdate()
    {
        // ���� ������ OnFixedUpdate() �Լ��� ȣ���Ͽ� ���º� ���� ������ ����
        currentState?.OnFixedUpdate();
    }


    // ���ο� ���·� ��ȯ�ϴ� �޼���
    public void SwitchState(AIState newState)
    {
        if (currentState != null)
        {
            // ���� ������ OnExit() �Լ� ȣ��
            currentState.OnExit();
        }

        // �� ���·� ��ȯ�ϰ� OnEnter() �Լ� ȣ��
        currentState = newState;
        currentState.OnEnter();
    }
}