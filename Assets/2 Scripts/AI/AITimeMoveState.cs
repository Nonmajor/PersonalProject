using UnityEngine;
using UnityEngine.AI;

// AI가 특정 목표 지점(플레이어의 마지막 위치)으로 이동하는 상태
public class AITimeMoveState : AIState
{
    private float moveDuration = 10f;
    private float timer;

    public AITimeMoveState(AIStateMachine stateMachine) : base(stateMachine) { }

    public override void OnEnter()
    {
        Debug.Log("TimeMove 상태 진입");
        timer = moveDuration;
        stateMachine.controller.agent.isStopped = false;

        // lastKnownPlayerPosition을 사용하여 목표 지점 설정
        if (stateMachine.controller.lastKnownPlayerPosition != Vector3.zero)
        {
            stateMachine.controller.MoveTo(stateMachine.controller.lastKnownPlayerPosition);
        }
        else
        {
            // lastKnownPlayerPosition이 설정되지 않았을 경우, 현재 위치에서 임의의 위치로 이동
            stateMachine.controller.MoveTo(stateMachine.controller.transform.position);
        }

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