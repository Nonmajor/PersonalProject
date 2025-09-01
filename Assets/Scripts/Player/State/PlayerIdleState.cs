using UnityEngine;

// 대기 상태
public class PlayerIdleState : PlayerState
{
    
    public PlayerIdleState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    
    public override void OnEnter()
    {
        
        Debug.Log("Idle 상태 진입");
        // 플레이어의 이동 속도를 0으로 설정하여 움직임 정지
        stateMachine.playerController.moveSpeed = 0f;
        // 스테미나 재생 쿨다운을 비활성화하여 스테미나 재생
        stateMachine.playerController.staminaRegenCooldown = false;
    }

    
    public override void OnExit()
    {
        Debug.Log("Idle 상태 종료");
    }

   
    public override void OnUpdate()
    {
        // 'moveInput'의 크기(magnitude)가 0보다 크면 (즉, WASD 입력이 있으면)
        if (stateMachine.playerController.moveInput.magnitude > 0)
        {
            // 'Walk' 상태로 전환
            stateMachine.SwitchState(stateMachine.WalkState);
        }
        // 아이템 사용 버튼(좌클릭)이 눌렸으면
        else if (stateMachine.playerController.isUsingItem)
        {
            // 'UseItem' 상태로 전환
            stateMachine.SwitchState(stateMachine.UseItemState);
        }
        // 아이템 획득 버튼(E)이 눌렸으면
        else if (stateMachine.playerController.isPickingUpItem)
        {
            // 'PickUpItem' 상태로 전환
            stateMachine.SwitchState(stateMachine.PickUpItemState);
        }
    }

    
    public override void OnFixedUpdate()
    {
        // 스테미나를 재생하는 함수를 호출
        stateMachine.playerController.RegenerateStamina();
    }
}