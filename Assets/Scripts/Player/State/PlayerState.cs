using UnityEngine;

namespace Player.State
{
    // 모든 플레이어 상태의 기본이 되는 추상 클래스
    // 이 클래스를 상속받는 다른 스크립트들은 OnEnter, OnExit, OnUpdate, OnFixedUpdate 함수를 반드시 구현해야함
    public abstract class PlayerState
    {

        // 각 상태가 PlayerStateMachine에 접근할 수 있도록 참조를 보호
        protected PlayerStateMachine stateMachine;


        // 생성자: PlayerState를 생성할 때 상태 머신 인스턴스를 받아서 초기화
        public PlayerState(PlayerStateMachine stateMachine)
        {
            this.stateMachine = stateMachine;
        }


        // 상태에 진입할 때 한 번 호출되는 추상 메서드
        public abstract void OnEnter();
        // 상태에서 벗어날 때 한 번 호출되는 추상 메서드
        public abstract void OnExit();
        // 매 프레임 호출되는 추상 메서드 (입력, 애니메이션 제어 등)
        public abstract void OnUpdate();
        // 일정한 시간 간격으로 호출되는 추상 메서드 (물리 관련 계산에 사용)
        public abstract void OnFixedUpdate();
    }
} 