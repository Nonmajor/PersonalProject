using UnityEngine;

public class FlashlightHandler : MonoBehaviour
{
    private Light lightSource;
    private bool isOn = false;

    private void Awake()
    {
        lightSource = GetComponentInChildren<Light>();

        if (lightSource == null)
        {
            Debug.LogError("Light 컴포넌트를 찾을 수 없습니다. 손전등 프리팹에 Light 컴포넌트가 있는지 확인해주세요.");
        }
        else
        {
            // 손전등의 초기 상태를 '꺼진 상태'로 설정
            lightSource.enabled = false;
        }
    }

    public void Use()
    {
        isOn = !isOn;
        if (lightSource != null)
        {
            lightSource.enabled = isOn;
        }

        // === 추가: 손전등 사용 시 사운드 재생 ===
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayFlashlightSound();
        }
    }
}