using UnityEngine;

// ��� ����
public class PlayerIdleState : PlayerState
{
    
    public PlayerIdleState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    
    public override void OnEnter()
    {
        
        Debug.Log("Idle ���� ����");
        // �÷��̾��� �̵� �ӵ��� 0���� �����Ͽ� ������ ����
        stateMachine.playerController.moveSpeed = 0f;
        // ���׹̳� ��� ��ٿ��� ��Ȱ��ȭ�Ͽ� ���׹̳� ���
        stateMachine.playerController.staminaRegenCooldown = false;
    }

    
    public override void OnExit()
    {
        Debug.Log("Idle ���� ����");
    }

   
    public override void OnUpdate()
    {
        // 'moveInput'�� ũ��(magnitude)�� 0���� ũ�� (��, WASD �Է��� ������)
        if (stateMachine.playerController.moveInput.magnitude > 0)
        {
            // 'Walk' ���·� ��ȯ
            stateMachine.SwitchState(stateMachine.WalkState);
        }
        // ������ ��� ��ư(��Ŭ��)�� ��������
        else if (stateMachine.playerController.isUsingItem)
        {
            // 'UseItem' ���·� ��ȯ
            stateMachine.SwitchState(stateMachine.UseItemState);
        }
        // ������ ȹ�� ��ư(E)�� ��������
        else if (stateMachine.playerController.isPickingUpItem)
        {
            // 'PickUpItem' ���·� ��ȯ
            stateMachine.SwitchState(stateMachine.PickUpItemState);
        }
    }

    
    public override void OnFixedUpdate()
    {
        // ���׹̳��� ����ϴ� �Լ��� ȣ��
        stateMachine.playerController.RegenerateStamina();
    }
}