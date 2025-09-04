using UnityEngine;
using System.Collections;
using Player.State;

namespace Player.State
{
    // 플레이어가 달리고 있는 상태
    public class PlayerRunState : PlayerState
    {
        public PlayerRunState(PlayerStateMachine stateMachine) : base(stateMachine) { }

        // 발소리 재생 코루틴
        private IEnumerator PlayFootstepSounds(float interval)
        {
            while (true)
            {
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlayFootstepSound();
                }
                yield return new WaitForSeconds(interval);
            }
        }

        public override void OnEnter()
        {
            Debug.Log("Run 상태 진입");
            stateMachine.playerController.moveSpeed = stateMachine.playerController.runSpeed;

            // 발소리 재생 코루틴 시작
            stateMachine.playerController.StartCoroutine(PlayFootstepSounds(stateMachine.playerController.runFootstepInterval));
        }

        public override void OnUpdate()
        {
            // 움직임 입력이 없거나 달리기 버튼이 해제되면 Walk 또는 Idle 상태로 전환
            if (stateMachine.playerController.moveInput == Vector2.zero)
            {
                stateMachine.SwitchState(stateMachine.IdleState);
                return;
            }

            if (!stateMachine.playerController.isRunning)
            {
                stateMachine.SwitchState(stateMachine.WalkState);
                return;
            }

            // 스테미나를 계속 소모
            stateMachine.playerController.UseStamina();

            // 스테미나가 0이 되면 탈진 상태로 전환
            if (stateMachine.playerController.currentStamina <= 0)
            {
                stateMachine.playerController.staminaRegenCooldown = true;
                stateMachine.SwitchState(stateMachine.ExhaustedState);
                return;
            }

            // 아이템 줍기 시 PickUpItem 상태로 전환
            if (stateMachine.playerController.isPickingUpItem)
            {
                stateMachine.SwitchState(stateMachine.PickUpItemState);
                return;
            }
        }

        public override void OnFixedUpdate()
        {
            stateMachine.playerController.HandleMovement();
        }

        public override void OnExit()
        {
            Debug.Log("Run 상태 종료");

            // 발소리 코루틴 정지
            stateMachine.playerController.StopAllCoroutines();
        }
    }
}