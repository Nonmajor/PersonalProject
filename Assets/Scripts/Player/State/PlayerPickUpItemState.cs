using UnityEngine;

// 아이템 획득 상태


namespace Player.State
{
    public class PlayerPickUpItemState : PlayerState
    {

        public PlayerPickUpItemState(PlayerStateMachine stateMachine) : base(stateMachine) { }



        public override void OnEnter()
        {
            Debug.Log("PickUpItem 상태 진입");

            // 아이템 획득과 관련된 실제 로직 구현 예정 (현재는 가칭)
            Debug.Log("아이템 획득!");
        }


        public override void OnExit()
        {
            Debug.Log("PickUpItem 상태 종료");
        }


        public override void OnUpdate()
        {
            // 'isPickingUpItem' 변수가 false가 되면 (E 키를 떼면)
            if (!stateMachine.playerController.isPickingUpItem)
            {
                // 'Idle' 상태로 전환하여 다음 행동을 준비
                stateMachine.SwitchState(stateMachine.IdleState);
            }
        }

        // 추후 수정 예정
        public override void OnFixedUpdate() { }
    }
}
