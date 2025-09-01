using UnityEngine;

// ������ ���
public class PlayerUseItemState : PlayerState
{
    
    public PlayerUseItemState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    
    public override void OnEnter()
    {
        
        Debug.Log("UseItem ���� ����");
        // ������ ���� ���õ� ���� ���� ���� ����
        // ��: ������ �Ҹ�, �ִϸ��̼� ��� ��
        Debug.Log("������ ���!");
    }

    
    public override void OnExit()
    {
        Debug.Log("UseItem ���� ����");
    }

   
    public override void OnUpdate()
    {
        // 'isUsingItem' ������ false�� �Ǹ� (������ ��� ��ư���� ���� ����)
        if (!stateMachine.playerController.isUsingItem)
        {
            // 'Idle' ���·� ��ȯ�Ͽ� ���� �ൿ�� �غ�
            stateMachine.SwitchState(stateMachine.IdleState);
        }
    }

    // ���� ���� ����
    public override void OnFixedUpdate() { }
}