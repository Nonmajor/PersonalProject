
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSlotUI : MonoBehaviour
{
    public Image[] slotBackgrounds; // 슬롯 배경 이미지 (항상 보임)
    public Image[] slotIcons; // 아이템 아이콘 이미지 (아이템 획득 시 표시)
    public TextMeshProUGUI keyCountText;

    // 아이템 장착 시 반투명 이미지(착용 아이템 강조 효과)를 표시할 배열
    [Header("Equip UI")]
    public Image[] equipOverlays;

    void Start()
    {
        // 씬 시작 시 아이템 아이콘은 투명하게, 슬롯 배경은 보이게 설정
        foreach (var icon in slotIcons)
        {
            icon.sprite = null;
            icon.color = new Color(1, 1, 1, 0);
        }

        

        // 추가: 장착 오버레이도 시작 시 모두 비활성화
        foreach (var overlay in equipOverlays)
        {
            overlay.gameObject.SetActive(false);
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
        // 모든 오버레이를 비활성화 (장착 해제 효과)
        foreach (var overlay in equipOverlays)
        {
            overlay.gameObject.SetActive(false);
        }

        // 선택된 슬롯의 오버레이만 활성화
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