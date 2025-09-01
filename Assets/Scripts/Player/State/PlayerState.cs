using UnityEngine;

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





    // ���¿� ������ �� �� �� ȣ��Ǵ� �߻� �޼����Դϴ�.
    public abstract void OnEnter();
    // ���¿��� ��� �� �� �� ȣ��Ǵ� �߻� �޼����Դϴ�.

    public abstract void OnExit();
    // �� ������ ȣ��Ǵ� �߻� �޼����Դϴ�. (�Է�, �ִϸ��̼� ���� ��)

    public abstract void OnUpdate();
    // ������ �ð� �������� ȣ��Ǵ� �߻� �޼����Դϴ�. (���� ���� ��꿡 ���)

    public abstract void OnFixedUpdate();
}