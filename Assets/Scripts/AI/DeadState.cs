using UnityEngine;

// AI가 플레이어와 접촉했을 때 게임을 정지 및 게임오버시키는 상태
public class DeadState : AIState
{
    
    public DeadState(AIStateMachine stateMachine) : base(stateMachine) { }

    
    public override void OnEnter()
    {
        Debug.Log("Dead 상태 진입: 게임 정지");

        // 1. 게임 시간 정지 (모든 게임 로직 중단)
        Time.timeScale = 0f;

        // 2. 게임 오버 UI 활성화
        // AIController에 연결된 UI를 사용
        if (stateMachine.controller.deadUI != null)
        {
            stateMachine.controller.deadUI.SetActive(true);
            Debug.Log("Dead UI를 활성화했습니다.");
        }
        else
        {
            Debug.LogWarning("DeadUI 오브젝트가 AIController에 연결되어 있지 않습니다.");
        }
    }

    // 게임이 정지되므로 이 함수는 호출되지않음
    public override void OnExit() { }

    // 게임이 정지되므로 이 함수는 호출되지않음
    public override void OnUpdate() { }

    // 게임이 정지되므로 이 함수는 호출되지않음
    public override void OnFixedUpdate() { }
}