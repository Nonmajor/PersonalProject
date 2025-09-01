using UnityEngine;

// 걷기 상태
public class PlayerWalkState : PlayerState
{
    
    public PlayerWalkState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    
    public override void OnEnter()
    {
        Debug.Log("Walk 상태 진입");


        // 플레이어의 이동 속도를 걷기 속도로 설정
        stateMachine.playerController.moveSpeed = stateMachine.playerController.walkSpeed;

        // 스테미나 재생 쿨다운을 비활성화하여 스테미나가 재생되도록함
        stateMachine.playerController.staminaRegenCooldown = false;

    }

    
    public override void OnExit()
    {
        Debug.Log("Walk 상태 종료");
    }

    

    public override void OnUpdate()
    {

        // '달리기' 버튼을 누르고 있고 스테미나가 남아있으면
        if (stateMachine.playerController.isRunning && stateMachine.playerController.currentStamina > 0)
        {
            // 'Run(달리기)' 상태로 전환
            stateMachine.SwitchState(stateMachine.RunState);
        }
        // 그렇지 않고, 이동 입력이 없으면 (WASD 키를 뗐을 경우)
        else if (stateMachine.playerController.moveInput.magnitude == 0)
        {
            // 'Idle(대기)' 상태로 전환
            stateMachine.SwitchState(stateMachine.IdleState);
        }
        // 그 외, 아이템 사용 버튼이 눌렸으면
        else if (stateMachine.playerController.isUsingItem)
        {
            // 'UseItem' 상태로 전환
            stateMachine.SwitchState(stateMachine.UseItemState);
        }
        // 그 외, 아이템 획득 버튼이 눌렸으면
        else if (stateMachine.playerController.isPickingUpItem)
        {
            // 'PickUpItem' 상태로 전환
            stateMachine.SwitchState(stateMachine.PickUpItemState);
        }
    }

    
    public override void OnFixedUpdate()
    {
        // 플레이어의 움직임(걷기)을 처리
        stateMachine.playerController.HandleMovement();


        // 스테미나를 재생하는 함수를 호출
        stateMachine.playerController.RegenerateStamina();
    }
}