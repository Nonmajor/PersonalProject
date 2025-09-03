using UnityEngine;
using UnityEngine.AI;

// AI�� '�߰�' ���¸� �����ϴ� Ŭ����
// AI�� �÷��̾ �߰���
public class AIChaseState : AIState
{
    public AIChaseState(AIStateMachine stateMachine) : base(stateMachine) { }

    
    public override void OnEnter()
    {

        Debug.Log("Chase ���� ����");

        // AI�� �ӵ��� �߰� �ӵ��� ����
        stateMachine.controller.agent.speed = stateMachine.controller.chaseSpeed;

        // �÷��̾ ��ģ �� ���� �ð��� ������ Ÿ�̸Ӹ� 0���� �ʱ�ȭ
        stateMachine.controller.pursuitTimer = 0f;
    }

    
    public override void OnExit()
    {
        Debug.Log("Chase ���� ����");
    }

    
    public override void OnUpdate()
    {
        // �÷��̾��� ���� ��ġ�� �̵� ��ǥ�� ��� ������Ʈ.
        stateMachine.controller.MoveTo(stateMachine.controller.player.position);

        // �÷��̾ �þ� ���� ���� �ִ��� Ȯ��
        if (stateMachine.controller.IsPlayerInVision())
        {
            // �÷��̾ ��� ���� ������ �߰� Ÿ�̸Ӹ� �ʱ�ȭ
            stateMachine.controller.pursuitTimer = 0f;
        }

        else
        {
            // �÷��̾ ���ƴٸ� �߰� Ÿ�̸Ӹ� ������Ŵ
            stateMachine.controller.pursuitTimer += Time.deltaTime;

            // Ÿ�̸Ӱ� �߰� ���� �ð��� �ʰ��ϸ� ��� ���·� ��ȯ
            if (stateMachine.controller.pursuitTimer >= stateMachine.controller.pursuitTime)
            {
                stateMachine.SwitchState(stateMachine.AlertState);
            }
        }
    }

    // ���� ������Ʈ�� NavMeshAgent���� ó���ϹǷ� ����
    public override void OnFixedUpdate() { }
}