using UnityEngine;
using System.Collections;
using Player.State;

namespace Player.State
{
    // �÷��̾ �޸��� �ִ� ����
    public class PlayerRunState : PlayerState
    {
        public PlayerRunState(PlayerStateMachine stateMachine) : base(stateMachine) { }

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
            Debug.Log("Run ���� ����");
            stateMachine.playerController.moveSpeed = stateMachine.playerController.runSpeed;

            // �߼Ҹ� ��� �ڷ�ƾ ����
            stateMachine.playerController.StartCoroutine(PlayFootstepSounds(stateMachine.playerController.runFootstepInterval));
        }

        public override void OnUpdate()
        {
            // ������ �Է��� ���ų� �޸��� ��ư�� �����Ǹ� Walk �Ǵ� Idle ���·� ��ȯ
            if (stateMachine.playerController.moveInput == Vector2.zero)
            {
                stateMachine.SwitchState(stateMachine.IdleState);
                return;
            }

            if (!stateMachine.playerController.isRunning)
            {
                stateMachine.SwitchState(stateMachine.WalkState);
                return;
            }

            // ���׹̳��� ��� �Ҹ�
            stateMachine.playerController.UseStamina();

            // ���׹̳��� 0�� �Ǹ� Ż�� ���·� ��ȯ
            if (stateMachine.playerController.currentStamina <= 0)
            {
                stateMachine.playerController.staminaRegenCooldown = true;
                stateMachine.SwitchState(stateMachine.ExhaustedState);
                return;
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
        }

        public override void OnExit()
        {
            Debug.Log("Run ���� ����");

            // �߼Ҹ� �ڷ�ƾ ����
            stateMachine.playerController.StopAllCoroutines();
        }
    }
}