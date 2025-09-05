using UnityEngine;
using UnityEngine.AI;

// AI가 일정 시간마다 플레이어의 현재 위치로 이동하는 상태
public class AITimeMoveState : AIState
{
    private float timer;

    public AITimeMoveState(AIStateMachine stateMachine) : base(stateMachine) { }

    public override void OnEnter()
    {
        Debug.Log("TimeMove 상태 진입");
        stateMachine.controller.agent.isStopped = false;
        stateMachine.controller.agent.speed = stateMachine.controller.moveSpeed;
        timer = stateMachine.controller.timedMoveInterval; // 타이머 초기화
    }

    public override void OnExit()
    {
        Debug.Log("TimeMove 상태 종료");
    }

    public override void OnUpdate()
    {
        // 1. 플레이어를 감지하면 즉시 추격 상태로 전환
        if (stateMachine.controller.IsPlayerInVision())
        {
            stateMachine.SwitchState(stateMachine.ChaseState);
            return;
        }

        // 2. 타이머를 사용하여 일정 시간마다 플레이어의 현재 위치로 이동
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            Debug.Log("플레이어의 위치를 갱신합니다.");
            stateMachine.controller.MoveTo(stateMachine.controller.playerTransform.position);
            timer = stateMachine.controller.timedMoveInterval; // 타이머 재설정
        }
    }

    public override void OnFixedUpdate() { }
}