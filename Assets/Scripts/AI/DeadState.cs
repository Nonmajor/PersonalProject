using UnityEngine;

// AI�� �÷��̾�� �������� �� ������ ���� �� ���ӿ�����Ű�� ����
public class DeadState : AIState
{
    
    public DeadState(AIStateMachine stateMachine) : base(stateMachine) { }

    
    public override void OnEnter()
    {
        Debug.Log("Dead ���� ����: ���� ����");

        // 1. ���� �ð� ���� (��� ���� ���� �ߴ�)
        Time.timeScale = 0f;

        // 2. ���� ���� UI Ȱ��ȭ
        // AIController�� ����� UI�� ���
        if (stateMachine.controller.deadUI != null)
        {
            stateMachine.controller.deadUI.SetActive(true);
            Debug.Log("Dead UI�� Ȱ��ȭ�߽��ϴ�.");
        }
        else
        {
            Debug.LogWarning("DeadUI ������Ʈ�� AIController�� ����Ǿ� ���� �ʽ��ϴ�.");
        }
    }

    // ������ �����ǹǷ� �� �Լ��� ȣ���������
    public override void OnExit() { }

    // ������ �����ǹǷ� �� �Լ��� ȣ���������
    public override void OnUpdate() { }

    // ������ �����ǹǷ� �� �Լ��� ȣ���������
    public override void OnFixedUpdate() { }
}