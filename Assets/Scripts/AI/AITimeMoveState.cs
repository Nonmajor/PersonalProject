using UnityEngine;
using UnityEngine.AI;

// AI가 특정 목표 지점(플레이어의 마지막 위치)으로 이동하는 상태
public class AITimedMoveState : AIState
{
    public AITimedMoveState(AIStateMachine stateMachine) : base(stateMachine) { }

    
    public override void OnEnter()
    {

        Debug.Log("TimedMove 상태 진입: 플레이어 마지막 위치로 이동");

        // 이동 속도를 순찰 속도로 설정
        stateMachine.controller.agent.speed = stateMachine.controller.patrolSpeed;

        // 마지막으로 저장된 플레이어의 위치로 이동 목표를 설정
        stateMachine.controller.MoveTo(stateMachine.controller.lastKnownPlayerPosition);
    }

    
    public override void OnExit()
    {
    }

    
    public override void OnUpdate()
    {

        // 1. 이동 중 플레이어를 감지하면 즉시 추격 상태로 전환
        if (stateMachine.controller.IsPlayerInVision())
        {
            stateMachine.SwitchState(stateMachine.ChaseState);
            return; // 상태 전환 후 추가 로직을 실행하지 않음
        }

        // 2. AI가 목표 지점에 거의 도달했으면 순찰 상태로 전환
        if (stateMachine.controller.agent.remainingDistance <= stateMachine.controller.agent.stoppingDistance)
        {
            stateMachine.SwitchState(stateMachine.PatrolState);
        }
    }

    
    public override void OnFixedUpdate() { }
}