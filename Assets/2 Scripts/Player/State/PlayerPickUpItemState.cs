using UnityEngine;

// ������ ȹ�� ����


namespace Player.State
{
    public class PlayerPickUpItemState : PlayerState
    {

        public PlayerPickUpItemState(PlayerStateMachine stateMachine) : base(stateMachine) { }



        public override void OnEnter()
        {
            Debug.Log("PickUpItem ���� ����");

            // ������ ȹ��� ���õ� ���� ���� ���� ���� (����� ��Ī)
            Debug.Log("������ ȹ��!");
        }


        public override void OnExit()
        {
            Debug.Log("PickUpItem ���� ����");
        }


        public override void OnUpdate()
        {
            // 'isPickingUpItem' ������ false�� �Ǹ� (E Ű�� ����)
            if (!stateMachine.playerController.isPickingUpItem)
            {
                // 'Idle' ���·� ��ȯ�Ͽ� ���� �ൿ�� �غ�
                stateMachine.SwitchState(stateMachine.IdleState);
            }
        }

        // ���� ���� ����
        public override void OnFixedUpdate() { }
    }
}
