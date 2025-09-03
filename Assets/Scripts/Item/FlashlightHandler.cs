// FlashlightHandler.cs

using UnityEngine;

public class FlashlightHandler : MonoBehaviour
{
    private GameObject lightSource;
    private bool isOn = false;

    void Awake()
    {
        // 자식 오브젝트에서 LightSource 오브젝트를 찾습니다.
        lightSource = transform.Find("LightSource").gameObject;
        if (lightSource != null)
        {
            lightSource.SetActive(false); // 시작 시 꺼진 상태로
        }
    }

    // Use() 함수는 PlayerItemController에서 호출됩니다.
    public void Use()
    {
        isOn = !isOn;
        if (lightSource != null)
        {
            lightSource.SetActive(isOn);
        }
    }

    // 오브젝트가 비활성화될 때(착용 해제) 상태를 저장
    void OnDisable()
    {
        // 손전등이 비활성화될 때 on/off 상태를 유지
        if (lightSource != null)
        {
            // lightSource의 activeSelf 상태를 저장하는 로직 필요
        }
    }

    // 다시 활성화될 때 저장된 상태를 복구
    void OnEnable()
    {
        // 저장된 on/off 상태를 불러오는 로직 필요
    }
}