using UnityEngine;
using UnityEngine.AI;

// AI의 '순찰' 상태를 정의하는 클래스
// AI는 무작위로 이동하며 플레이어를 찾음
public class AIPatrolState : AIState
{
    // 순찰 목표 지점
    private Vector3 patrolTarget;

    // 새로운 목표 지점을 설정할 타이머
    private float newTargetTimer;

    // AITimeMoveState로 전환할 타이머
    private float timedMoveTimer;

    // 새로운 목표를 설정할 대기 시간 (5초)
    private const float newTargetDelay = 5.0f;

    public AIPatrolState(AIStateMachine stateMachine) : base(stateMachine) { }



    public override void OnEnter()
    {
        Debug.Log("Patrol 상태 진입");
        stateMachine.controller.agent.speed = stateMachine.controller.patrolSpeed;
        newTargetTimer = newTargetDelay;
        timedMoveTimer = stateMachine.controller.timedMoveInterval; // 타이머 초기화
        stateMachine.controller.agent.isStopped = false;
        stateMachine.controller.SetPatrolAnimation();
    }

    public override void OnExit()
    {
        Debug.Log("Patrol 상태 종료");
    }


    public override void OnUpdate()
    {

        // 1. 순찰 중 플레이어를 감지하면 Chase 상태로 전환
        // 플레이어가 시야 범위 내에 들어왔는지 확인
        if (stateMachine.controller.IsPlayerInVision())
        {
            // 플레이어를 마지막으로 본 위치를 저장
            stateMachine.controller.lastKnownPlayerPosition = stateMachine.controller.playerTransform.position;
            stateMachine.SwitchState(stateMachine.ChaseState);
            return;
        }

        // 2. 일정 시간마다 AITimeMoveState로 전환
        timedMoveTimer -= Time.deltaTime;
        if (timedMoveTimer <= 0)
        {
            stateMachine.SwitchState(stateMachine.TimeMoveState);
            timedMoveTimer = stateMachine.controller.timedMoveInterval; // 타이머 재설정
            return;
        }


        // 3. 목표 지점에 도착했거나, 일정 시간이 지났으면 새로운 목표를 설정.
        if (stateMachine.controller.agent.remainingDistance <= stateMachine.controller.agent.stoppingDistance || newTargetTimer <= 0)
        {
            SetNewPatrolTarget();
            newTargetTimer = newTargetDelay; // 타이머 재설정
        }

        // 4. 새로운 목표를 설정할 타이머 감소
        newTargetTimer -= Time.deltaTime;
    }

    // 물리 업데이트는 NavMeshAgent에 맡기므로 공백
    public override void OnFixedUpdate() { }


    // 무작위 순찰 목표 지점을 설정하는 함수
    private void SetNewPatrolTarget()
    {
        Vector3 randomDirection = Random.insideUnitSphere * stateMachine.controller.patrolRadius;
        randomDirection += stateMachine.controller.transform.position;
        NavMeshHit hit;

        // NavMesh 위에서 유효한 위치를 찾아 AI의 목적지로 설정
        if (NavMesh.SamplePosition(randomDirection, out hit, stateMachine.controller.patrolRadius, NavMesh.AllAreas))
        {
            stateMachine.controller.MoveTo(hit.position);
        }
    }
}