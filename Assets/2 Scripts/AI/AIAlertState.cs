using UnityEngine;

// AI가 멈춰서 주위를 둘러보는 '경계' 상태
public class AIAlertState : AIState
{
    private float timer; // 경계 상태 지속 시간을 잴 타이머
    private const float alertDuration = 10f; // 경계 상태를 유지할 시간

    public AIAlertState(AIStateMachine stateMachine) : base(stateMachine) { }


    public override void OnEnter()
    {
        Debug.Log("Alert 상태 진입");
        stateMachine.controller.agent.isStopped = true;
        timer = alertDuration;
        stateMachine.controller.SetAlertAnimation(); // 수정된 부분
    }


    public override void OnExit()
    {

        Debug.Log("Alert 상태 종료");
        // AI의 이동을 다시 허용

        stateMachine.controller.agent.isStopped = false;

    }

    
    public override void OnUpdate()
    {

        // 1. 경계 중 플레이어를 감지하면 즉시 추격 상태로 전환
        if (stateMachine.controller.IsPlayerInVision())
        {
            stateMachine.SwitchState(stateMachine.ChaseState);
            return;
        }

        // 2. AI를 Y축 기준으로 회전시켜 주위를 둘러보게함
        stateMachine.controller.transform.Rotate(Vector3.up * stateMachine.controller.alertRotationSpeed * Time.deltaTime);

        // 3. 타이머를 감소시킴
        timer -= Time.deltaTime;

        // 4. 타이머가 0 이하가 되면 순찰 상태로 전환
        if (timer <= 0)
        {
            stateMachine.SwitchState(stateMachine.PatrolState);
        }
    }

    
    public override void OnFixedUpdate() { }
}