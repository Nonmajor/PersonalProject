using UnityEngine;
using System.Collections;
using Player.State;

namespace Player.State
{
    // �÷��̾ �Ȱ� �ִ� ����
    public class PlayerWalkState : PlayerState
    {
        public PlayerWalkState(PlayerStateMachine stateMachine) : base(stateMachine) { }

        // �߼Ҹ� ��� �ڷ�ƾ
        private IEnumerator PlayFootstepSounds(float interval)
        {
            while (true)
            {
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlayFootstepSound();
                }
                yield return new WaitForSeconds(interval);
            }
        }

        public override void OnEnter()
        {
            Debug.Log("Walk ���� ����");
            stateMachine.playerController.moveSpeed = stateMachine.playerController.walkSpeed;

            // �߼Ҹ� ��� �ڷ�ƾ ����
            stateMachine.playerController.StartCoroutine(PlayFootstepSounds(stateMachine.playerController.walkFootstepInterval));

        }

        public override void OnUpdate()
        {
            // ������ �Է��� ������ Idle ���·� ��ȯ
            if (stateMachine.playerController.moveInput == Vector2.zero)
            {
                stateMachine.SwitchState(stateMachine.IdleState);
                return;
            }

            // �޸��� ��ư�� ������ Run ���·� ��ȯ
            if (stateMachine.playerController.isRunning)
            {
                stateMachine.SwitchState(stateMachine.RunState);
            }

            // ������ �ݱ� �� PickUpItem ���·� ��ȯ
            if (stateMachine.playerController.isPickingUpItem)
            {
                stateMachine.SwitchState(stateMachine.PickUpItemState);
                return;
            }
        }

        public override void OnFixedUpdate()
        {
            stateMachine.playerController.HandleMovement();
            stateMachine.playerController.RegenerateStamina();
        }

        public override void OnExit()
        {
            Debug.Log("Walk ���� ����");

            // �߼Ҹ� �ڷ�ƾ ����
            stateMachine.playerController.StopAllCoroutines();
        }
    }
}