using UnityEngine;
using UnityEngine.AI;

// AI�� ���� �ð����� �÷��̾��� ���� ��ġ�� �̵��ϴ� ����
public class AITimeMoveState : AIState
{
    private float timer;

    public AITimeMoveState(AIStateMachine stateMachine) : base(stateMachine) { }

    public override void OnEnter()
    {
        Debug.Log("TimeMove ���� ����");
        stateMachine.controller.agent.isStopped = false;
        stateMachine.controller.agent.speed = stateMachine.controller.moveSpeed;
        timer = stateMachine.controller.timedMoveInterval; // Ÿ�̸� �ʱ�ȭ
    }

    public override void OnExit()
    {
        Debug.Log("TimeMove ���� ����");
    }

    public override void OnUpdate()
    {
        // 1. �÷��̾ �����ϸ� ��� �߰� ���·� ��ȯ
        if (stateMachine.controller.IsPlayerInVision())
        {
            stateMachine.SwitchState(stateMachine.ChaseState);
            return;
        }

        // 2. Ÿ�̸Ӹ� ����Ͽ� ���� �ð����� �÷��̾��� ���� ��ġ�� �̵�
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            Debug.Log("�÷��̾��� ��ġ�� �����մϴ�.");
            stateMachine.controller.MoveTo(stateMachine.controller.playerTransform.position);
            timer = stateMachine.controller.timedMoveInterval; // Ÿ�̸� �缳��
        }
    }

    public override void OnFixedUpdate() { }
}