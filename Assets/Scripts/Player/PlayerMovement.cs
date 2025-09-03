using System.Collections;
using UnityEngine;

// 이 스크립트가 플레이어의 실제 움직임을 제어하는 스크립트라고 가정합니다.
// 스크립트 이름을 PlayerMovementScript라고 임의로 지정하겠습니다.
// 실제 스크립트의 이름으로 바꿔서 사용해주세요.

public class PlayerMovement : MonoBehaviour
{
    // 이동 속도를 제어하는 변수
    public float moveSpeed = 5.0f;

    // 부스터가 활성화되었는지 확인하는 변수
    public bool isBoosterActive = false;

    private CharacterController characterController;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        if (characterController == null)
        {
            Debug.LogError("CharacterController 컴포넌트를 찾을 수 없습니다.");
        }
    }

    void Update()
    {
        // 부스터가 활성화된 상태라면, 외부에서 속도를 제어하도록 합니다.
        if (isBoosterActive)
        {
            // 부스터 효과가 적용 중이므로, Update 루프에서 속도를 변경하지 않습니다.
        }
        else
        {
            // 부스터가 비활성화된 경우에만 정상적인 이동 로직을 실행
            Vector3 moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            if (characterController != null)
            {
                characterController.Move(moveDirection * moveSpeed * Time.deltaTime);
            }
        }
    }
}