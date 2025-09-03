// ItemSlotUI.cs

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSlotUI : MonoBehaviour
{
    public Image[] slotBackgrounds; // 슬롯 배경 이미지 (항상 보임)
    public Image[] slotIcons; // 아이템 아이콘 이미지 (아이템 획득 시 표시)
    public GameObject selectedSlotEffect;
    public TextMeshProUGUI keyCountText;

    void Start()
    {
        // 씬 시작 시 아이템 아이콘은 투명하게, 슬롯 배경은 보이게 설정
        foreach (var icon in slotIcons)
        {
            icon.sprite = null;
            icon.color = new Color(1, 1, 1, 0);
        }

        // 선택 슬롯 이펙트 숨기기
        if (selectedSlotEffect != null)
        {
            selectedSlotEffect.SetActive(false);
        }
    }

    public void UpdateSlotUI(int index, Sprite icon)
    {
        if (index >= 0 && index < slotIcons.Length)
        {
            // 아이콘을 보이게 하고 스프라이트 할당
            slotIcons[index].sprite = icon;
            slotIcons[index].color = new Color(1, 1, 1, 1);

            // 아이콘이 슬롯 크기를 초과하지 않도록 Aspect Ratio 유지
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