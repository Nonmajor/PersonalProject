using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerItemController : MonoBehaviour
{
    // Inventory Settings
    [Header("Inventory Settings")]
    public int maxItemSlots = 3;
    private ItemData[] itemSlots;
    private int currentEquippedIndex = -1;

    // Item Equip
    [Header("Item Equip")]
    public Transform itemEquipPoint;
    private GameObject currentEquippedPrefab;

    // Key Settings
    [Header("Key Settings")]
    public int maxKeyCount = 5;
    private int keyCount = 0;

    // UI
    [Header("UI")]
    public ItemSlotUI itemSlotUI;
    public TextMeshProUGUI interactionText;
    public Transform playerCamera;
    public float interactionDistance = 5f;

    // Input System
    private PlayerInput playerInput;

    // PlayerController 컴포넌트
    private PlayerController playerController;

    void Awake()
    {
        itemSlots = new ItemData[maxItemSlots];
        playerInput = GetComponent<PlayerInput>();
        playerController = GetComponent<PlayerController>();
        SetupInputActions();

        if (playerController == null)
        {
            Debug.LogError("PlayerController 컴포넌트를 찾을 수 없습니다.");
        }
    }

    void Start()
    {
        itemSlotUI.UpdateKeyCount(keyCount, maxKeyCount);
    }

    private void SetupInputActions()
    {
        if (playerInput != null && playerInput.actions != null)
        {
            playerInput.actions["Use Item"].performed += ctx => UseItem();
            playerInput.actions["Equip_1"].performed += ctx => EquipItem(0);
            playerInput.actions["Equip_2"].performed += ctx => EquipItem(1);
            playerInput.actions["Equip_3"].performed += ctx => EquipItem(2);
        }
    }

    void Update()
    {
        if (interactionText != null)
        {
            interactionText.text = "";
        }

        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            PickupItem item = hit.collider.GetComponent<PickupItem>();
            if (item != null)
            {
                if (interactionText != null && item.itemData != null)
                {
                    interactionText.text = $"{item.itemData.itemName} (E)";
                }

                if (Input.GetKeyDown(KeyCode.E))
                {
                    PickupItem(item);
                }
            }
        }
    }

    private void PickupItem(PickupItem itemToPickup)
    {
        if (itemToPickup == null) return;

        ItemData itemData = itemToPickup.itemData;

        if (itemData.itemType == ItemType.QuestItem)
        {
            if (keyCount < maxKeyCount)
            {
                keyCount++;
                itemSlotUI.UpdateKeyCount(keyCount, maxKeyCount);
                Debug.Log($"열쇠 획득! 현재 갯수: {keyCount}");
                Destroy(itemToPickup.gameObject);
            }
        }
        else
        {
            int targetSlotIndex = FindEmptySlot();

            if (targetSlotIndex == -1)
            {
                if (currentEquippedIndex != -1)
                {
                    targetSlotIndex = currentEquippedIndex;
                }
                else
                {
                    Debug.Log("아이템 슬롯이 전부 찼습니다.");
                    return;
                }
            }

            itemSlots[targetSlotIndex] = itemData;
            itemSlotUI.UpdateSlotUI(targetSlotIndex, itemData.itemIcon);

            EquipItem(targetSlotIndex);

            Debug.Log($"'{itemData.itemName}'을(를) 획득했습니다.");
            Destroy(itemToPickup.gameObject);
        }
    }

    private int FindEmptySlot()
    {
        if (itemSlots == null) return -1;
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (itemSlots[i] == null)
            {
                return i;
            }
        }
        return -1;
    }

    public void EquipItem(int index)
    {
        if (itemSlots == null) return;
        if (index < 0 || index >= itemSlots.Length) return;
        if (itemSlots[index] == null) return;

        if (currentEquippedPrefab != null)
        {
            Destroy(currentEquippedPrefab);
        }

        currentEquippedIndex = index;
        currentEquippedPrefab = Instantiate(itemSlots[index].itemPrefab, itemEquipPoint.position, itemEquipPoint.rotation, itemEquipPoint);
        itemSlotUI.SelectSlot(index);

        Debug.Log($"'{itemSlots[index].itemName}'을(를) 착용했습니다.");
    }

    private void UseItem()
    {
        if (currentEquippedIndex == -1) return;

        ItemData equippedItem = itemSlots[currentEquippedIndex];
        if (equippedItem == null) return;

        if (equippedItem.itemType == ItemType.Tool)
        {
            if (currentEquippedPrefab != null)
            {
                FlashlightHandler handler = currentEquippedPrefab.GetComponent<FlashlightHandler>();
                if (handler != null)
                {
                    handler.Use();
                }
            }
        }
        else if (equippedItem.itemType == ItemType.Consume)
        {
            if (playerController != null)
            {
                BoosterData boosterData = (BoosterData)equippedItem;
                // PlayerController의 ActivateBoosterEffect 함수를 직접 호출
                playerController.ActivateBoosterEffect(boosterData.boosterDuration, boosterData.speedMultiplier);

                itemSlots[currentEquippedIndex] = null;
                itemSlotUI.ClearSlotUI(currentEquippedIndex);
                Destroy(currentEquippedPrefab);
                currentEquippedIndex = -1;
            }
        }
    }
}