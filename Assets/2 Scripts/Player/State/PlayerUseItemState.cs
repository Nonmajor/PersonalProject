using UnityEngine;
using Player.State;

namespace Player.State
{
    // ������ ���
    public class PlayerUseItemState : PlayerState
    {
        public PlayerUseItemState(PlayerStateMachine stateMachine) : base(stateMachine) { }

        public override void OnEnter()
        {
            Debug.Log("UseItem ���� ����");
            Debug.Log("�Ҹ� ������ ���!");
        }

        public override void OnExit()
        {
            Debug.Log("UseItem ���� ����");
        }

        public override void OnUpdate()
        {
            // �Ҹ� �������� �� �� ��� �� �ٷ� Idle ���·� ���ư�
            stateMachine.SwitchState(stateMachine.IdleState);
        }

        public override void OnFixedUpdate() { }
    }
}