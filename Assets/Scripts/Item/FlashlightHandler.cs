// FlashlightHandler.cs

using UnityEngine;

public class FlashlightHandler : MonoBehaviour
{
    private GameObject lightSource;
    private bool isOn = false;

    void Awake()
    {
        // �ڽ� ������Ʈ���� LightSource ������Ʈ�� ã���ϴ�.
        lightSource = transform.Find("LightSource").gameObject;
        if (lightSource != null)
        {
            lightSource.SetActive(false); // ���� �� ���� ���·�
        }
    }

    // Use() �Լ��� PlayerItemController���� ȣ��˴ϴ�.
    public void Use()
    {
        isOn = !isOn;
        if (lightSource != null)
        {
            lightSource.SetActive(isOn);
        }
    }

    // ������Ʈ�� ��Ȱ��ȭ�� ��(���� ����) ���¸� ����
    void OnDisable()
    {
        // �������� ��Ȱ��ȭ�� �� on/off ���¸� ����
        if (lightSource != null)
        {
            // lightSource�� activeSelf ���¸� �����ϴ� ���� �ʿ�
        }
    }

    // �ٽ� Ȱ��ȭ�� �� ����� ���¸� ����
    void OnEnable()
    {
        // ����� on/off ���¸� �ҷ����� ���� �ʿ�
    }
}