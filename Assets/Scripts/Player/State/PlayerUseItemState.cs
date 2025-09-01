using UnityEngine;

// 아이템 사용
public class PlayerUseItemState : PlayerState
{
    
    public PlayerUseItemState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    
    public override void OnEnter()
    {
        
        Debug.Log("UseItem 상태 진입");
        // 아이템 사용과 관련된 실제 로직 구현 예정
        // 예: 아이템 소모, 애니메이션 재생 등
        Debug.Log("아이템 사용!");
    }

    
    public override void OnExit()
    {
        Debug.Log("UseItem 상태 종료");
    }

   
    public override void OnUpdate()
    {
        // 'isUsingItem' 변수가 false가 되면 (아이템 사용 버튼에서 손을 떼면)
        if (!stateMachine.playerController.isUsingItem)
        {
            // 'Idle' 상태로 전환하여 다음 행동을 준비
            stateMachine.SwitchState(stateMachine.IdleState);
        }
    }

    // 추후 수정 예정
    public override void OnFixedUpdate() { }
}