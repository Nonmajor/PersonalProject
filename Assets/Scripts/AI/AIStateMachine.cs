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
    public AITimedMoveState TimedMoveState;
    

    // AI의 기능과 데이터를 담고 있는 컨트롤러에 대한 참조
    [HideInInspector] public AIController controller;

   
    private void Awake()
    {
        // 동일 오브젝트에 있는 AIController 컴포넌트를 가져옴
        controller = GetComponent<AIController>();


        // 각 상태 클래스의 인스턴스를 생성하고 상태 머신에 대한 참조를 전달
        PatrolState = new AIPatrolState(this);
        ChaseState = new AIChaseState(this);
        AlertState = new AIAlertState(this);
        TimedMoveState = new AITimedMoveState(this);
        
    }

    
    private void Start()
    {
        // 게임 시작 시 TimeScale을 1로 초기화하여 시간이 흐르게 함
        Time.timeScale = 1f;

        // AI의 초기 상태를 순찰(Patrol) 상태로 설정
        SwitchState(PatrolState);
    }

    
    private void Update()
    {

        // 현재 상태의 OnUpdate() 함수를 호출하여 상태별 로직을 실행
        // `?.` 연산자는 null 체크를 통해 안전하게 함수를 호출
        currentState?.OnUpdate();

    }

    
    private void FixedUpdate()
    {
        // 현재 상태의 OnFixedUpdate() 함수를 호출
        currentState?.OnFixedUpdate();
    }


    // 새로운 상태로 전환하는 핵심 함수
    public void SwitchState(AIState newState)
    {
        // 현재 상태가 있다면, 해당 상태의 OnExit() 함수를 호출하여 상태를 종료
        currentState?.OnExit();

        // 현재 상태를 새로운 상태로 교체
        currentState = newState;

        // 새로운 상태의 OnEnter() 함수를 호출하여 상태를 시작
        currentState?.OnEnter();
    }
}