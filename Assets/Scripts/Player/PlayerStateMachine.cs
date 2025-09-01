using UnityEngine;

// 플레이어의 상태를 관리하는 FSM(Finite State Machine) 클래스
public class PlayerStateMachine : MonoBehaviour
{
    // 현재 플레이어의 상태를 나타내는 변수
    public PlayerState currentState;

    // 플레이어가 가질 수 있는 모든 상태들
    public PlayerIdleState IdleState;
    public PlayerWalkState WalkState;
    public PlayerRunState RunState;
    public PlayerUseItemState UseItemState;
    public PlayerPickUpItemState PickUpItemState;
    public PlayerExhaustedState ExhaustedState;


    // PlayerController에 대한 참조
    public PlayerController playerController;


    

    
    private void Awake()
    {

        // PlayerController 컴포넌트 가져오기
        playerController = GetComponent<PlayerController>();


        // 각 상태 클래스의 인스턴스를 생성
        IdleState = new PlayerIdleState(this);
        WalkState = new PlayerWalkState(this);
        RunState = new PlayerRunState(this);
        UseItemState = new PlayerUseItemState(this);
        PickUpItemState = new PlayerPickUpItemState(this);
        ExhaustedState = new PlayerExhaustedState(this);
    }

    

    private void Start()
    {
        // 게임 시작 시 초기 상태를 IdleState로 설정하고 전환
        SwitchState(IdleState);
    }


    
    private void Update()
    {
        // 현재 상태의 OnUpdate() 함수 호출 (null 체크를 통해 안전하게 호출)
        currentState?.OnUpdate();
    }


    
    private void FixedUpdate()
    {
        // 현재 상태의 OnFixedUpdate() 함수 호출 (null 체크를 통해 안전하게 호출)
        currentState?.OnFixedUpdate();
    }

    

    // 새로운 상태로 전환하는 함수
    public void SwitchState(PlayerState newState)
    {
        // 현재 상태가 있다면, 현재 상태의 OnExit() 함수를 호출하여 상태를 종료
        currentState?.OnExit();

        // 현재 상태를 새로운 상태로 변경
        currentState = newState;

        // 새로운 상태의 OnEnter() 함수를 호출하여 상태를 시작
        currentState?.OnEnter();
    }
}