
using UnityEngine;
using Player.State;

public class PlayerStateMachine : MonoBehaviour
{
    public PlayerState currentState;
    public PlayerIdleState IdleState;
    public PlayerWalkState WalkState;
    public PlayerRunState RunState;
    public PlayerUseItemState UseItemState;
    public PlayerPickUpItemState PickUpItemState;
    public PlayerExhaustedState ExhaustedState;
    public PlayerController playerController;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();

        IdleState = new PlayerIdleState(this);
        WalkState = new PlayerWalkState(this);
        RunState = new PlayerRunState(this);
        UseItemState = new PlayerUseItemState(this);
        PickUpItemState = new PlayerPickUpItemState(this);
        ExhaustedState = new PlayerExhaustedState(this);
    }

    private void Start()
    {
        SwitchState(IdleState);
    }

    private void Update()
    {
        currentState?.OnUpdate();
    }

    private void FixedUpdate()
    {
        currentState?.OnFixedUpdate();
    }

    public void SwitchState(PlayerState newState)
    {
        currentState?.OnExit();
        currentState = newState;
        currentState?.OnEnter();
    }
}