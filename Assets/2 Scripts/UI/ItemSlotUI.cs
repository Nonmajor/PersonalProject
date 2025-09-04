
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSlotUI : MonoBehaviour
{
    public Image[] slotBackgrounds; // ���� ��� �̹��� (�׻� ����)
    public Image[] slotIcons; // ������ ������ �̹��� (������ ȹ�� �� ǥ��)
    public TextMeshProUGUI keyCountText;

    // ������ ���� �� ������ �̹���(���� ������ ���� ȿ��)�� ǥ���� �迭
    [Header("Equip UI")]
    public Image[] equipOverlays;

    void Start()
    {
        // �� ���� �� ������ �������� �����ϰ�, ���� ����� ���̰� ����
        foreach (var icon in slotIcons)
        {
            icon.sprite = null;
            icon.color = new Color(1, 1, 1, 0);
        }

        

        // �߰�: ���� �������̵� ���� �� ��� ��Ȱ��ȭ
        foreach (var overlay in equipOverlays)
        {
            overlay.gameObject.SetActive(false);
        }
    }

    public void UpdateSlotUI(int index, Sprite icon)
    {
        if (index >= 0 && index < slotIcons.Length)
        {
            // �������� ���̰� �ϰ� ��������Ʈ �Ҵ�
            slotIcons[index].sprite = icon;
            slotIcons[index].color = new Color(1, 1, 1, 1);

            // �������� ���� ũ�⸦ �ʰ����� �ʵ��� Aspect Ratio ����
            slotIcons[index].preserveAspect = true;
        }
    }

    public void ClearSlotUI(int index)
    {
        if (index >= 0 && index < slotIcons.Length)
        {
            slotIcons[index].sprite = null;
            slotIcons[index].color = new Color(1, 1, 1, 0);
        }
    }

    public void SelectSlot(int index)
    {
        // ��� �������̸� ��Ȱ��ȭ (���� ���� ȿ��)
        foreach (var overlay in equipOverlays)
        {
            overlay.gameObject.SetActive(false);
        }

        // ���õ� ������ �������̸� Ȱ��ȭ
        if (index >= 0 && index < equipOverlays.Length)
        {
            equipOverlays[index].gameObject.SetActive(true);
        }
    }

    public void UpdateKeyCount(int currentCount, int maxCount)
    {
        if (keyCountText != null)
        {
            keyCountText.text = $"{currentCount}/{maxCount}";
        }
    }

}