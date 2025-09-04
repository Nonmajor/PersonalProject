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
            Debug.LogError("Light ������Ʈ�� ã�� �� �����ϴ�. ������ �����տ� Light ������Ʈ�� �ִ��� Ȯ�����ּ���.");
        }
        else
        {
            // �������� �ʱ� ���¸� '���� ����'�� ����
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

        // === �߰�: ������ ��� �� ���� ��� ===
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayFlashlightSound();
        }
    }
}