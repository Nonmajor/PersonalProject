using UnityEngine;

namespace Player.State
{
    // ��� �÷��̾� ������ �⺻�� �Ǵ� �߻� Ŭ����
    // �� Ŭ������ ��ӹ޴� �ٸ� ��ũ��Ʈ���� OnEnter, OnExit, OnUpdate, OnFixedUpdate �Լ��� �ݵ�� �����ؾ���
    public abstract class PlayerState
    {

        // �� ���°� PlayerStateMachine�� ������ �� �ֵ��� ������ ��ȣ
        protected PlayerStateMachine stateMachine;


        // ������: PlayerState�� ������ �� ���� �ӽ� �ν��Ͻ��� �޾Ƽ� �ʱ�ȭ
        public PlayerState(PlayerStateMachine stateMachine)
        {
            this.stateMachine = stateMachine;
        }


        // ���¿� ������ �� �� �� ȣ��Ǵ� �߻� �޼���
        public abstract void OnEnter();
        // ���¿��� ��� �� �� �� ȣ��Ǵ� �߻� �޼���
        public abstract void OnExit();
        // �� ������ ȣ��Ǵ� �߻� �޼��� (�Է�, �ִϸ��̼� ���� ��)
        public abstract void OnUpdate();
        // ������ �ð� �������� ȣ��Ǵ� �߻� �޼��� (���� ���� ��꿡 ���)
        public abstract void OnFixedUpdate();
    }
} 