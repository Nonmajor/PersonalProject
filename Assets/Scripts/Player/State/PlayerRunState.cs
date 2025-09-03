using Player.State;
using UnityEngine;

// 달리기 상태
public class PlayerRunState : PlayerState
{
    // 생성자: 상태 머신 참조를 초기화
    public PlayerRunState(PlayerStateMachine stateMachine) : base(stateMachine) { }


    public override void OnEnter()
    {

        Debug.Log("Run 상태 진입");

        // 플레이어의 이동 속도를 달리기 속도로 설정
        stateMachine.playerController.moveSpeed = stateMachine.playerController.runSpeed;
        // 스테미나 재생 쿨다운을 활성화하여 스테미나가 재생되지 않도록 설정
        stateMachine.playerController.staminaRegenCooldown = true;
    }


    public override void OnExit()
    {
        Debug.Log("Run 상태 종료");
    }


    public override void OnUpdate()
    {
        // 달리기 상태에 머무는 동안 현재 속도 값을 계속 적용
        stateMachine.playerController.moveSpeed = stateMachine.playerController.runSpeed;

        // 스테미나 소모
        stateMachine.playerController.UseStamina();

        // 현재 스테미나가 0 이하인지 확인
        if (stateMachine.playerController.currentStamina <= 0)
        {
            // 스테미나가 고갈되면 즉시 'Exhausted(탈진)' 상태로 전환
            stateMachine.SwitchState(stateMachine.ExhaustedState);
            return;
        }

        // '달리기' 버튼을 놓았거나, 이동 입력(WASD)이 없으면
        if (!stateMachine.playerController.isRunning || stateMachine.playerController.moveInput.magnitude == 0)
        {
            // 이동 입력이 있으면 'Walk(걷기)' 상태로 전환
            if (stateMachine.playerController.moveInput.magnitude > 0)
            {
                stateMachine.SwitchState(stateMachine.WalkState);
            }
            else
            {
                // 이동 입력이 없으면 'Idle(대기)' 상태로 전환
                stateMachine.SwitchState(stateMachine.IdleState);
            }
        }

        // '달리기' 중에도 아이템 사용 입력이 감지되면
        else if (stateMachine.playerController.isUsingItem)
        {
            // 'UseItem' 상태로 전환
            stateMachine.SwitchState(stateMachine.UseItemState);
        }
        // '달리기' 중에도 아이템 획득 입력이 감지되면
        else if (stateMachine.playerController.isPickingUpItem)
        {
            // 'PickUpItem' 상태로 전환
            stateMachine.SwitchState(stateMachine.PickUpItemState);
        }
    }


    public override void OnFixedUpdate()
    {
        // 플레이어의 움직임 처리
        stateMachine.playerController.HandleMovement();
        stateMachine.playerController.UseStamina();
    }
}