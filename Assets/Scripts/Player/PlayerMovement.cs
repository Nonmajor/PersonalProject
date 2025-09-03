using System.Collections;
using UnityEngine;

// �� ��ũ��Ʈ�� �÷��̾��� ���� �������� �����ϴ� ��ũ��Ʈ��� �����մϴ�.
// ��ũ��Ʈ �̸��� PlayerMovementScript��� ���Ƿ� �����ϰڽ��ϴ�.
// ���� ��ũ��Ʈ�� �̸����� �ٲ㼭 ������ּ���.

public class PlayerMovement : MonoBehaviour
{
    // �̵� �ӵ��� �����ϴ� ����
    public float moveSpeed = 5.0f;

    // �ν��Ͱ� Ȱ��ȭ�Ǿ����� Ȯ���ϴ� ����
    public bool isBoosterActive = false;

    private CharacterController characterController;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        if (characterController == null)
        {
            Debug.LogError("CharacterController ������Ʈ�� ã�� �� �����ϴ�.");
        }
    }

    void Update()
    {
        // �ν��Ͱ� Ȱ��ȭ�� ���¶��, �ܺο��� �ӵ��� �����ϵ��� �մϴ�.
        if (isBoosterActive)
        {
            // �ν��� ȿ���� ���� ���̹Ƿ�, Update �������� �ӵ��� �������� �ʽ��ϴ�.
        }
        else
        {
            // �ν��Ͱ� ��Ȱ��ȭ�� ��쿡�� �������� �̵� ������ ����
            Vector3 moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            if (characterController != null)
            {
                characterController.Move(moveDirection * moveSpeed * Time.deltaTime);
            }
        }
    }
}