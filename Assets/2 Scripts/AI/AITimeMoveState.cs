using UnityEngine;
using UnityEngine.AI;

// AI�� Ư�� ��ǥ ����(�÷��̾��� ������ ��ġ)���� �̵��ϴ� ����
public class AITimedMoveState : AIState
{
    public AITimedMoveState(AIStateMachine stateMachine) : base(stateMachine) { }

    
    public override void OnEnter()
    {

        Debug.Log("TimedMove ���� ����: �÷��̾� ������ ��ġ�� �̵�");

        // �̵� �ӵ��� ���� �ӵ��� ����
        stateMachine.controller.agent.speed = stateMachine.controller.patrolSpeed;

        // ���������� ����� �÷��̾��� ��ġ�� �̵� ��ǥ�� ����
        stateMachine.controller.MoveTo(stateMachine.controller.lastKnownPlayerPosition);
    }

    
    public override void OnExit()
    {
    }

    
    public override void OnUpdate()
    {

        // 1. �̵� �� �÷��̾ �����ϸ� ��� �߰� ���·� ��ȯ
        if (stateMachine.controller.IsPlayerInVision())
        {
            stateMachine.SwitchState(stateMachine.ChaseState);
            return; // ���� ��ȯ �� �߰� ������ �������� ����
        }

        // 2. AI�� ��ǥ ������ ���� ���������� ���� ���·� ��ȯ
        if (stateMachine.controller.agent.remainingDistance <= stateMachine.controller.agent.stoppingDistance)
        {
            stateMachine.SwitchState(stateMachine.PatrolState);
        }
    }

    
    public override void OnFixedUpdate() { }
}