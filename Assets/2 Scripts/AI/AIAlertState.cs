using UnityEngine;

// AI�� ���缭 ������ �ѷ����� '���' ����
public class AIAlertState : AIState
{
    private float timer; // ��� ���� ���� �ð��� �� Ÿ�̸�
    private const float alertDuration = 10f; // ��� ���¸� ������ �ð�

    public AIAlertState(AIStateMachine stateMachine) : base(stateMachine) { }


    public override void OnEnter()
    {
        Debug.Log("Alert ���� ����");
        stateMachine.controller.agent.isStopped = true;
        timer = alertDuration;
        stateMachine.controller.SetAlertAnimation(); // ������ �κ�
    }


    public override void OnExit()
    {

        Debug.Log("Alert ���� ����");
        // AI�� �̵��� �ٽ� ���

        stateMachine.controller.agent.isStopped = false;

    }

    
    public override void OnUpdate()
    {

        // 1. ��� �� �÷��̾ �����ϸ� ��� �߰� ���·� ��ȯ
        if (stateMachine.controller.IsPlayerInVision())
        {
            stateMachine.SwitchState(stateMachine.ChaseState);
            return;
        }

        // 2. AI�� Y�� �������� ȸ������ ������ �ѷ�������
        stateMachine.controller.transform.Rotate(Vector3.up * stateMachine.controller.alertRotationSpeed * Time.deltaTime);

        // 3. Ÿ�̸Ӹ� ���ҽ�Ŵ
        timer -= Time.deltaTime;

        // 4. Ÿ�̸Ӱ� 0 ���ϰ� �Ǹ� ���� ���·� ��ȯ
        if (timer <= 0)
        {
            stateMachine.SwitchState(stateMachine.PatrolState);
        }
    }

    
    public override void OnFixedUpdate() { }
}