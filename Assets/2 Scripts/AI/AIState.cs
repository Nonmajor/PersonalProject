using UnityEngine;

// 모든 AI 상태의 기본이 되는 추상 클래스
public abstract class AIState
{
    // 각 상태가 AIStateMachine에 접근할 수 있도록 참조
    protected AIStateMachine stateMachine;

    
    public AIState(AIStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    // 각 상태에 진입할 때 한 번 호출
    public abstract void OnEnter();


    // 상태에서 벗어날 때 한 번 호출
    public abstract void OnExit();


    // 매 프레임 호출
    public abstract void OnUpdate();


    // 일정한 주기로 호출
    public abstract void OnFixedUpdate();
}