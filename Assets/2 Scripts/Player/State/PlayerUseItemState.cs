using UnityEngine;
using Player.State;

namespace Player.State
{
    // 아이템 사용
    public class PlayerUseItemState : PlayerState
    {
        public PlayerUseItemState(PlayerStateMachine stateMachine) : base(stateMachine) { }

        public override void OnEnter()
        {
            Debug.Log("UseItem 상태 진입");
            Debug.Log("소모성 아이템 사용!");
        }

        public override void OnExit()
        {
            Debug.Log("UseItem 상태 종료");
        }

        public override void OnUpdate()
        {
            // 소모성 아이템은 한 번 사용 후 바로 Idle 상태로 돌아감
            stateMachine.SwitchState(stateMachine.IdleState);
        }

        public override void OnFixedUpdate() { }
    }
}