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
        stateMachine.controller.agent.speed = stateMachine.controller.chaseSpeed;
        stateMachine.controller.pursuitTimer = 0f;
        stateMachine.controller.SetChaseAnimation();

        // AudioManager.Instance가 null인지 확인
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayChaseMusic();
        }
    }

    public override void OnExit()
    {
        Debug.Log("Chase 상태 종료");
        // AudioManager.Instance가 null인지 확인
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopChaseMusic();
        }
    }


    public override void OnUpdate()
    {
        // 플레이어의 현재 위치로 이동 목표를 계속 업데이트.
        
        if (stateMachine.controller.playerTransform != null)
        {
            stateMachine.controller.MoveTo(stateMachine.controller.playerTransform.position);
        }


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

    public override void OnFixedUpdate() { }
}