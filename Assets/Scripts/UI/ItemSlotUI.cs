// ItemSlotUI.cs

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSlotUI : MonoBehaviour
{
    public Image[] slotBackgrounds; // ���� ��� �̹��� (�׻� ����)
    public Image[] slotIcons; // ������ ������ �̹��� (������ ȹ�� �� ǥ��)
    public GameObject selectedSlotEffect;
    public TextMeshProUGUI keyCountText;

    void Start()
    {
        // �� ���� �� ������ �������� �����ϰ�, ���� ����� ���̰� ����
        foreach (var icon in slotIcons)
        {
            icon.sprite = null;
            icon.color = new Color(1, 1, 1, 0);
        }

        // ���� ���� ����Ʈ �����
        if (selectedSlotEffect != null)
        {
            selectedSlotEffect.SetActive(false);
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
        if (selectedSlotEffect != null)
        {
            selectedSlotEffect.SetActive(true);
            selectedSlotEffect.transform.position = slotIcons[index].transform.position;
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