using UnityEngine;

// �ȱ� ����
public class PlayerWalkState : PlayerState
{
    
    public PlayerWalkState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    
    public override void OnEnter()
    {
        Debug.Log("Walk ���� ����");


        // �÷��̾��� �̵� �ӵ��� �ȱ� �ӵ��� ����
        stateMachine.playerController.moveSpeed = stateMachine.playerController.walkSpeed;

        // ���׹̳� ��� ��ٿ��� ��Ȱ��ȭ�Ͽ� ���׹̳��� ����ǵ�����
        stateMachine.playerController.staminaRegenCooldown = false;

    }

    
    public override void OnExit()
    {
        Debug.Log("Walk ���� ����");
    }

    

    public override void OnUpdate()
    {

        // '�޸���' ��ư�� ������ �ְ� ���׹̳��� ����������
        if (stateMachine.playerController.isRunning && stateMachine.playerController.currentStamina > 0)
        {
            // 'Run(�޸���)' ���·� ��ȯ
            stateMachine.SwitchState(stateMachine.RunState);
        }
        // �׷��� �ʰ�, �̵� �Է��� ������ (WASD Ű�� ���� ���)
        else if (stateMachine.playerController.moveInput.magnitude == 0)
        {
            // 'Idle(���)' ���·� ��ȯ
            stateMachine.SwitchState(stateMachine.IdleState);
        }
        // �� ��, ������ ��� ��ư�� ��������
        else if (stateMachine.playerController.isUsingItem)
        {
            // 'UseItem' ���·� ��ȯ
            stateMachine.SwitchState(stateMachine.UseItemState);
        }
        // �� ��, ������ ȹ�� ��ư�� ��������
        else if (stateMachine.playerController.isPickingUpItem)
        {
            // 'PickUpItem' ���·� ��ȯ
            stateMachine.SwitchState(stateMachine.PickUpItemState);
        }
    }

    
    public override void OnFixedUpdate()
    {
        // �÷��̾��� ������(�ȱ�)�� ó��
        stateMachine.playerController.HandleMovement();


        // ���׹̳��� ����ϴ� �Լ��� ȣ��
        stateMachine.playerController.RegenerateStamina();
    }
}