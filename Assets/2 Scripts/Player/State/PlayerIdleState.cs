using UnityEngine;
using Player.State;

namespace Player.State
{
    // �÷��̾��� '���' ���¸� �����ϴ� ��ũ��Ʈ
    // �÷��̾ �������� �ʰ� ���� ���� �������� ���� ���� ���� ������ ���
    public class PlayerIdleState : PlayerState
    {
        
        public PlayerIdleState(PlayerStateMachine stateMachine) : base(stateMachine) { }

        
        public override void OnEnter()
        {
            Debug.Log("Idle ���� ����");

            // �÷��̾��� �̵� �ӵ��� 0���� �����Ͽ� �������� ����
            stateMachine.playerController.moveSpeed = 0f;

            // ���׹̳� ��� ��ٿ��� ��Ȱ��ȭ�Ͽ� ���׹̳� ����� �����ϵ��� ����
            stateMachine.playerController.staminaRegenCooldown = false;
        }

        
        public override void OnExit()
        {
            Debug.Log("Idle ���� ����");
        }

        
        public override void OnUpdate()
        {
            // �÷��̾��� �̵� �Է��� �����Ǹ�
            if (stateMachine.playerController.moveInput.magnitude > 0)
            {
                // '�ȱ�' ���·� ��ȯ
                stateMachine.SwitchState(stateMachine.WalkState);
            }
            // �÷��̾ �������� �ݴ� ���̶��
            else if (stateMachine.playerController.isPickingUpItem)
            {
                // '������ �ݱ�' ���·� ��ȯ
                stateMachine.SwitchState(stateMachine.PickUpItemState);
            }
        }

        
        public override void OnFixedUpdate()
        {
            // ���׹̳��� ���
            stateMachine.playerController.RegenerateStamina();
        }
    }
}