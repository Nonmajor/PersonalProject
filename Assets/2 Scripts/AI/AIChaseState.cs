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
        stateMachine.controller.agent.speed = stateMachine.controller.chaseSpeed;
        stateMachine.controller.pursuitTimer = 0f;
        stateMachine.controller.SetChaseAnimation();

        // AudioManager.Instance�� null���� Ȯ��
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayChaseMusic();
        }
    }

    public override void OnExit()
    {
        Debug.Log("Chase ���� ����");
        // AudioManager.Instance�� null���� Ȯ��
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopChaseMusic();
        }
    }


    public override void OnUpdate()
    {
        // �÷��̾��� ���� ��ġ�� �̵� ��ǥ�� ��� ������Ʈ.
        
        if (stateMachine.controller.playerTransform != null)
        {
            stateMachine.controller.MoveTo(stateMachine.controller.playerTransform.position);
        }


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

    public override void OnFixedUpdate() { }
}