using UnityEngine;

// AI의 상태를 관리
// 모든 AI 상태를 제어하고, 상태 전환을 담당
public class AIStateMachine : MonoBehaviour
{
    // 현재 AI의 상태
    public AIState currentState;

    // AI가 가질 수 있는 모든 상태 인스턴스
    public AIPatrolState PatrolState;
    public AIChaseState ChaseState;
    public AIAlertState AlertState;
    public AITimeMoveState TimeMoveState;

    // AI의 기능과 데이터를 담고 있는 컨트롤러에 대한 참조
    [HideInInspector] public AIController controller;

    private void Awake()
    {
        // 게임 시작 시 TimeScale을 1로 초기화하여 시간이 흐르게 함
        Time.timeScale = 1f;
    }


    // AIStateMachine을 초기화하는 메서드 (AIController에서 호출)
    public void Initialize(AIState startingState)
    {
        PatrolState = new AIPatrolState(this);
        ChaseState = new AIChaseState(this);
        AlertState = new AIAlertState(this);
        TimeMoveState = new AITimeMoveState(this);

        // AI의 초기 상태를 설정
        SwitchState(startingState);
    }

    private void Update()
    {
        // 현재 상태의 OnUpdate() 함수를 호출하여 상태별 로직을 실행
        currentState?.OnUpdate();
    }


    private void FixedUpdate()
    {
        // 현재 상태의 OnFixedUpdate() 함수를 호출하여 상태별 물리 로직을 실행
        currentState?.OnFixedUpdate();
    }


    // 새로운 상태로 전환하는 메서드
    public void SwitchState(AIState newState)
    {
        if (currentState != null)
        {
            // 현재 상태의 OnExit() 함수 호출
            currentState.OnExit();
        }

        // 새 상태로 전환하고 OnEnter() 함수 호출
        currentState = newState;
        currentState.OnEnter();
    }
}