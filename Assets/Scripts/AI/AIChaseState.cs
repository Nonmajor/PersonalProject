using UnityEngine;
using UnityEngine.AI;

// AI의 '추격' 상태를 정의하는 클래스
// AI는 플레이어를 추격함
public class AIChaseState : AIState
{
    public AIChaseState(AIStateMachine stateMachine) : base(stateMachine) { }

    
    public override void OnEnter()
    {

        Debug.Log("Chase 상태 진입");

        // AI의 속도를 추격 속도로 설정
        stateMachine.controller.agent.speed = stateMachine.controller.chaseSpeed;

        // 플레이어를 놓친 후 추적 시간을 측정할 타이머를 0으로 초기화
        stateMachine.controller.pursuitTimer = 0f;
    }

    
    public override void OnExit()
    {
        Debug.Log("Chase 상태 종료");
    }

    
    public override void OnUpdate()
    {
        // 플레이어의 현재 위치로 이동 목표를 계속 업데이트.
        stateMachine.controller.MoveTo(stateMachine.controller.player.position);

        // 플레이어가 시야 범위 내에 있는지 확인
        if (stateMachine.controller.IsPlayerInVision())
        {
            // 플레이어를 계속 보고 있으면 추격 타이머를 초기화
            stateMachine.controller.pursuitTimer = 0f;
        }

        else
        {
            // 플레이어를 놓쳤다면 추격 타이머를 증가시킴
            stateMachine.controller.pursuitTimer += Time.deltaTime;

            // 타이머가 추격 지속 시간을 초과하면 경계 상태로 전환
            if (stateMachine.controller.pursuitTimer >= stateMachine.controller.pursuitTime)
            {
                stateMachine.SwitchState(stateMachine.AlertState);
            }
        }
    }

    // 물리 업데이트는 NavMeshAgent에서 처리하므로 공백
    public override void OnFixedUpdate() { }
}