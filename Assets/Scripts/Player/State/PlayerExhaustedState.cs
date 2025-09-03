using UnityEngine;


// 탈진 상태
namespace Player.State
{
    public class PlayerExhaustedState : PlayerState
    {
        // 탈진 상태의 남은 시간을 계산할 타이머
        private float timer;


        public PlayerExhaustedState(PlayerStateMachine stateMachine) : base(stateMachine) { }


        public override void OnEnter()
        {

            Debug.Log("Exhausted 상태 진입");
            // 플레이어의 이동 속도를 탈진 속도로 설정
            stateMachine.playerController.moveSpeed = stateMachine.playerController.walkSpeed * stateMachine.playerController.exhaustedSpeedMultiplier;
            // 타이머를 탈진 지속 시간으로 초기화
            timer = stateMachine.playerController.exhaustedDuration;
            // 스테미나 재생 쿨다운을 활성화 (재생 중지)
            stateMachine.playerController.staminaRegenCooldown = true;
        }


        public override void OnExit()
        {
            Debug.Log("Exhausted 상태 종료");
        }




        public override void OnUpdate()
        {

            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                stateMachine.SwitchState(stateMachine.IdleState);
            }
        }







        public override void OnFixedUpdate()
        {
            // 플레이어의 움직임 처리 (감속된 속도로 이동)
            stateMachine.playerController.HandleMovement();
            // 스테미나 재생 처리 (쿨다운 상태이므로 재생되지 않음)
            stateMachine.playerController.RegenerateStamina();
        }
    }
}
