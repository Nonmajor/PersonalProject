using UnityEngine;

// ��� AI ������ �⺻�� �Ǵ� �߻� Ŭ����
public abstract class AIState
{
    // �� ���°� AIStateMachine�� ������ �� �ֵ��� ����
    protected AIStateMachine stateMachine;

    
    public AIState(AIStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    // �� ���¿� ������ �� �� �� ȣ��
    public abstract void OnEnter();


    // ���¿��� ��� �� �� �� ȣ��
    public abstract void OnExit();


    // �� ������ ȣ��
    public abstract void OnUpdate();


    // ������ �ֱ�� ȣ��
    public abstract void OnFixedUpdate();
}