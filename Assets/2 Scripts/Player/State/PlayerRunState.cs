using Player.State;
using UnityEngine;

// �޸��� ����
public class PlayerRunState : PlayerState
{
    // ������: ���� �ӽ� ������ �ʱ�ȭ
    public PlayerRunState(PlayerStateMachine stateMachine) : base(stateMachine) { }


    public override void OnEnter()
    {

        Debug.Log("Run ���� ����");

        // �÷��̾��� �̵� �ӵ��� �޸��� �ӵ��� ����
        stateMachine.playerController.moveSpeed = stateMachine.playerController.runSpeed;
        // ���׹̳� ��� ��ٿ��� Ȱ��ȭ�Ͽ� ���׹̳��� ������� �ʵ��� ����
        stateMachine.playerController.staminaRegenCooldown = true;
    }


    public override void OnExit()
    {
        Debug.Log("Run ���� ����");
    }


    public override void OnUpdate()
    {
        // �޸��� ���¿� �ӹ��� ���� ���� �ӵ� ���� ��� ����
        stateMachine.playerController.moveSpeed = stateMachine.playerController.runSpeed;

        // ���׹̳� �Ҹ�
        stateMachine.playerController.UseStamina();

        // ���� ���׹̳��� 0 �������� Ȯ��
        if (stateMachine.playerController.currentStamina <= 0)
        {
            // ���׹̳��� ���Ǹ� ��� 'Exhausted(Ż��)' ���·� ��ȯ
            stateMachine.SwitchState(stateMachine.ExhaustedState);
            return;
        }

        // '�޸���' ��ư�� ���Ұų�, �̵� �Է�(WASD)�� ������
        if (!stateMachine.playerController.isRunning || stateMachine.playerController.moveInput.magnitude == 0)
        {
            // �̵� �Է��� ������ 'Walk(�ȱ�)' ���·� ��ȯ
            if (stateMachine.playerController.moveInput.magnitude > 0)
            {
                stateMachine.SwitchState(stateMachine.WalkState);
            }
            else
            {
                // �̵� �Է��� ������ 'Idle(���)' ���·� ��ȯ
                stateMachine.SwitchState(stateMachine.IdleState);
            }
        }

        // '�޸���' �߿��� ������ ��� �Է��� �����Ǹ�
        else if (stateMachine.playerController.isUsingItem)
        {
            // 'UseItem' ���·� ��ȯ
            stateMachine.SwitchState(stateMachine.UseItemState);
        }
        // '�޸���' �߿��� ������ ȹ�� �Է��� �����Ǹ�
        else if (stateMachine.playerController.isPickingUpItem)
        {
            // 'PickUpItem' ���·� ��ȯ
            stateMachine.SwitchState(stateMachine.PickUpItemState);
        }
    }


    public override void OnFixedUpdate()
    {
        // �÷��̾��� ������ ó��
        stateMachine.playerController.HandleMovement();
        stateMachine.playerController.UseStamina();
    }
}