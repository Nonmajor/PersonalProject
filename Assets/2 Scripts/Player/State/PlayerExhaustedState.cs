using UnityEngine;


// Ż�� ����
namespace Player.State
{
    public class PlayerExhaustedState : PlayerState
    {
        // Ż�� ������ ���� �ð��� ����� Ÿ�̸�
        private float timer;


        public PlayerExhaustedState(PlayerStateMachine stateMachine) : base(stateMachine) { }


        public override void OnEnter()
        {

            Debug.Log("Exhausted ���� ����");
            // �÷��̾��� �̵� �ӵ��� Ż�� �ӵ��� ����
            stateMachine.playerController.moveSpeed = stateMachine.playerController.walkSpeed * stateMachine.playerController.exhaustedSpeedMultiplier;
            // Ÿ�̸Ӹ� Ż�� ���� �ð����� �ʱ�ȭ
            timer = stateMachine.playerController.exhaustedDuration;
            // ���׹̳� ��� ��ٿ��� Ȱ��ȭ (��� ����)
            stateMachine.playerController.staminaRegenCooldown = true;
        }


        public override void OnExit()
        {
            Debug.Log("Exhausted ���� ����");
        }




        public override void OnUpdate()
        {

            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                stateMachine.SwitchState(stateMachine.IdleState);
            }
        }







        public override void OnFixedUpdate()
        {
            // �÷��̾��� ������ ó�� (���ӵ� �ӵ��� �̵�)
            stateMachine.playerController.HandleMovement();
            // ���׹̳� ��� ó�� (��ٿ� �����̹Ƿ� ������� ����)
            stateMachine.playerController.RegenerateStamina();
        }
    }
}
