using UnityEngine;
using System.Collections;
using Player.State;

namespace Player.State
{
    // 플레이어가 걷고 있는 상태
    public class PlayerWalkState : PlayerState
    {
        public PlayerWalkState(PlayerStateMachine stateMachine) : base(stateMachine) { }

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
            Debug.Log("Walk 상태 진입");
            stateMachine.playerController.moveSpeed = stateMachine.playerController.walkSpeed;

            // 발소리 재생 코루틴 시작
            stateMachine.playerController.StartCoroutine(PlayFootstepSounds(stateMachine.playerController.walkFootstepInterval));

        }

        public override void OnUpdate()
        {
            // 움직임 입력이 없으면 Idle 상태로 전환
            if (stateMachine.playerController.moveInput == Vector2.zero)
            {
                stateMachine.SwitchState(stateMachine.IdleState);
                return;
            }

            // 달리기 버튼이 눌리면 Run 상태로 전환
            if (stateMachine.playerController.isRunning)
            {
                stateMachine.SwitchState(stateMachine.RunState);
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
            stateMachine.playerController.RegenerateStamina();
        }

        public override void OnExit()
        {
            Debug.Log("Walk 상태 종료");

            // 발소리 코루틴 정지
            stateMachine.playerController.StopAllCoroutines();
        }
    }
}