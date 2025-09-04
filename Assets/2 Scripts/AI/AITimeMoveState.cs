using UnityEngine;
using UnityEngine.AI;

// AI�� Ư�� ��ǥ ����(�÷��̾��� ������ ��ġ)���� �̵��ϴ� ����
public class AITimeMoveState : AIState
{
    private float moveDuration = 10f;
    private float timer;

    public AITimeMoveState(AIStateMachine stateMachine) : base(stateMachine) { }

    public override void OnEnter()
    {
        Debug.Log("TimeMove ���� ����");
        timer = moveDuration;
        stateMachine.controller.agent.isStopped = false;

        // lastKnownPlayerPosition�� ����Ͽ� ��ǥ ���� ����
        if (stateMachine.controller.lastKnownPlayerPosition != Vector3.zero)
        {
            stateMachine.controller.MoveTo(stateMachine.controller.lastKnownPlayerPosition);
        }
        else
        {
            // lastKnownPlayerPosition�� �������� �ʾ��� ���, ���� ��ġ���� ������ ��ġ�� �̵�
            stateMachine.controller.MoveTo(stateMachine.controller.transform.position);
        }

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