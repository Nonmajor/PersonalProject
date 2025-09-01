using UnityEngine;

// �÷��̾��� ���¸� �����ϴ� FSM(Finite State Machine) Ŭ����
public class PlayerStateMachine : MonoBehaviour
{
    // ���� �÷��̾��� ���¸� ��Ÿ���� ����
    public PlayerState currentState;

    // �÷��̾ ���� �� �ִ� ��� ���µ�
    public PlayerIdleState IdleState;
    public PlayerWalkState WalkState;
    public PlayerRunState RunState;
    public PlayerUseItemState UseItemState;
    public PlayerPickUpItemState PickUpItemState;
    public PlayerExhaustedState ExhaustedState;


    // PlayerController�� ���� ����
    public PlayerController playerController;


    

    
    private void Awake()
    {

        // PlayerController ������Ʈ ��������
        playerController = GetComponent<PlayerController>();


        // �� ���� Ŭ������ �ν��Ͻ��� ����
        IdleState = new PlayerIdleState(this);
        WalkState = new PlayerWalkState(this);
        RunState = new PlayerRunState(this);
        UseItemState = new PlayerUseItemState(this);
        PickUpItemState = new PlayerPickUpItemState(this);
        ExhaustedState = new PlayerExhaustedState(this);
    }

    

    private void Start()
    {
        // ���� ���� �� �ʱ� ���¸� IdleState�� �����ϰ� ��ȯ
        SwitchState(IdleState);
    }


    
    private void Update()
    {
        // ���� ������ OnUpdate() �Լ� ȣ�� (null üũ�� ���� �����ϰ� ȣ��)
        currentState?.OnUpdate();
    }


    
    private void FixedUpdate()
    {
        // ���� ������ OnFixedUpdate() �Լ� ȣ�� (null üũ�� ���� �����ϰ� ȣ��)
        currentState?.OnFixedUpdate();
    }

    

    // ���ο� ���·� ��ȯ�ϴ� �Լ�
    public void SwitchState(PlayerState newState)
    {
        // ���� ���°� �ִٸ�, ���� ������ OnExit() �Լ��� ȣ���Ͽ� ���¸� ����
        currentState?.OnExit();

        // ���� ���¸� ���ο� ���·� ����
        currentState = newState;

        // ���ο� ������ OnEnter() �Լ��� ȣ���Ͽ� ���¸� ����
        currentState?.OnEnter();
    }
}