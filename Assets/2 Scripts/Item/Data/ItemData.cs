using UnityEngine;

// Asset 메뉴에 생성 옵션을 추가함
[CreateAssetMenu(fileName = "New Item Data", menuName = "Item/Base Item Data", order = 0)]
public class ItemData : ScriptableObject
{
    // 아이템의 이름
    public string itemName;
    // 아이템의 타입 (도구, 소모품 등)
    public ItemType itemType;
    // 아이템 획득 시 플레이어가 보게 될 모델링 프리팹
    public GameObject itemPrefab;
    // UI에 표시될 아이템 아이콘
    public Sprite itemIcon;
}