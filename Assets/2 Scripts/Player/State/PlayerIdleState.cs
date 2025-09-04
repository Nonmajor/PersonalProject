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
            // ������ �Է��� ������ Walk ���·� ��ȯ
            if (stateMachine.playerController.moveInput != Vector2.zero)
            {
                stateMachine.SwitchState(stateMachine.WalkState);
                return;
            }

            // ������ �ݱ� �� PickUpItem ���·� ��ȯ
            if (stateMachine.playerController.isPickingUpItem)
            {
                stateMachine.SwitchState(stateMachine.PickUpItemState);
                return;
            }
        }


        public override void OnFixedUpdate()
        {
            // ���׹̳��� ���
            stateMachine.playerController.RegenerateStamina();
        }
    }
}