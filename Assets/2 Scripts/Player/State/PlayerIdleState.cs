using UnityEngine;
using Player.State;

namespace Player.State
{
    // 플레이어의 '대기' 상태를 정의하는 스크립트
    // 플레이어가 움직이지 않고 있을 때와 아이템을 줍지 않을 때의 로직을 담당
    public class PlayerIdleState : PlayerState
    {
        
        public PlayerIdleState(PlayerStateMachine stateMachine) : base(stateMachine) { }

        
        public override void OnEnter()
        {
            Debug.Log("Idle 상태 진입");

            // 플레이어의 이동 속도를 0으로 설정하여 움직임을 정지
            stateMachine.playerController.moveSpeed = 0f;

            // 스테미나 재생 쿨다운을 비활성화하여 스테미나 재생이 가능하도록 설정
            stateMachine.playerController.staminaRegenCooldown = false;
        }

        
        public override void OnExit()
        {
            Debug.Log("Idle 상태 종료");
        }

        
        public override void OnUpdate()
        {
            // 플레이어의 이동 입력이 감지되면
            if (stateMachine.playerController.moveInput.magnitude > 0)
            {
                // '걷기' 상태로 전환
                stateMachine.SwitchState(stateMachine.WalkState);
            }
            // 플레이어가 아이템을 줍는 중이라면
            else if (stateMachine.playerController.isPickingUpItem)
            {
                // '아이템 줍기' 상태로 전환
                stateMachine.SwitchState(stateMachine.PickUpItemState);
            }
        }

        
        public override void OnFixedUpdate()
        {
            // 스테미나를 재생
            stateMachine.playerController.RegenerateStamina();
        }
    }
}