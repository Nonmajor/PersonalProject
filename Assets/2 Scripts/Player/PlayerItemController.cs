using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections.Generic;

// �÷��̾��� ������ ����, ����, ��� �� ��ȣ�ۿ��� ����
public class PlayerItemController : MonoBehaviour
{


    // �κ��丮 ����
    [Header("Inventory Settings")]
    public int maxItemSlots = 3; // �κ��丮 ������ �ִ� ����
    private ItemData[] itemSlots; // ������ �����͸� �����ϴ� �迭
    private int currentEquippedIndex = -1; // ���� ���� ���� �������� �ε��� (-1�� ������ ������ ����)


    // ������ ����
    [Header("Item Equip")]
    public Transform itemEquipPoint; // ������ �������� ������ ��ġ
    private GameObject currentEquippedPrefab; // ���� ������ �������� ������ �ν��Ͻ�


    // UI
    [Header("UI")]
    public ItemSlotUI itemSlotUI; // ������ ���� UI�� �����ϴ� ��ũ��Ʈ
    public TextMeshProUGUI interactionText; // ��ȣ�ۿ� �ؽ�Ʈ�� ǥ���� UI
    public Transform playerCamera; // �÷��̾� ī�޶��� Transform (Raycast�� ���)
    public float interactionDistance = 5f; // �������� �ֿ� �� �ִ� �ִ� �Ÿ�


    // �ý��� ������Ʈ
    private PlayerInput playerInput; // ����Ƽ Input System ������Ʈ
    private PlayerController playerController; // �÷��̾��� ���¸� �����ϴ� ��ũ��Ʈ
    private GameManager gameManager; // ���� ���¸� �����ϴ� �̱��� �ν��Ͻ�



    void Awake()
    {
        // �κ��丮 ���� �迭�� �ʱ�ȭ
        itemSlots = new ItemData[maxItemSlots];
        playerInput = GetComponent<PlayerInput>();
        playerController = GetComponent<PlayerController>();


        // GameManager �ν��Ͻ��� ������
        gameManager = GameManager.Instance;
        if (gameManager == null)
        {
            Debug.LogError("GameManager �ν��Ͻ��� ã�� �� �����ϴ�. ���� GameManager ������Ʈ�� �ְ� ��ũ��Ʈ�� �Ҵ�Ǿ����� Ȯ���ϼ���.");
        }

        if (playerController == null)
        {
            Debug.LogError("PlayerController ������Ʈ�� ã�� �� �����ϴ�.");
        }
    }

    void Start()
    {
        // GameManager���� ���� ���� ������ �� ���� ������ ������ UI�� ������Ʈ��
        itemSlotUI.UpdateKeyCount(gameManager.keysCollected, gameManager.KEYS_TO_SPAWN);
    }


    // �Է� �̺�Ʈ ��� �� ����
    private void OnEnable()
    {
        if (playerInput != null && playerInput.actions != null)
        {
            // Input Action�� ���� �ݹ� �Լ��� ���
            playerInput.actions["Use Item"].performed += OnUseItemPerformed;
            playerInput.actions["Use Item"].canceled += OnUseItemCanceled;

            playerInput.actions["Equip_1"].performed += context => EquipItem(0);
            playerInput.actions["Equip_2"].performed += context => EquipItem(1);
            playerInput.actions["Equip_3"].performed += context => EquipItem(2);
        }
    }


    private void OnDisable()
    {
        if (playerInput != null && playerInput.actions != null)
        {
            // ��ũ��Ʈ ��Ȱ��ȭ �� �ݹ� �Լ��� ����
            playerInput.actions["Use Item"].performed -= OnUseItemPerformed;
            playerInput.actions["Use Item"].canceled -= OnUseItemCanceled;

            playerInput.actions["Equip_1"].performed -= context => EquipItem(0);
            playerInput.actions["Equip_2"].performed -= context => EquipItem(1);
            playerInput.actions["Equip_3"].performed -= context => EquipItem(2);
        }
    }


    void Update()
    {
        // �� ������ ��ȣ�ۿ� �ؽ�Ʈ�� �ʱ�ȭ
        if (interactionText != null)
        {
            interactionText.text = "";
        }


        // �÷��̾� ī�޶󿡼� �������� Raycast�� ��
        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            PickupItem item = hit.collider.GetComponent<PickupItem>();
            if (item != null)
            {
                // Raycast�� �������� �����Ǹ� ��ȣ�ۿ� �ؽ�Ʈ�� ǥ����
                if (interactionText != null && item.itemData != null)
                {
                    interactionText.text = $"{item.itemData.itemName} (E)";
                }

                // E Ű�� ���� �������� ȹ��
                if (Input.GetKeyDown(KeyCode.E))
                {
                    PickupItem(item);
                }
            }
        }
    }


    //  �ٽ� ��� �Լ�
    // �������� �ֿ��� �κ��丮�� �߰�
    private void PickupItem(PickupItem itemToPickup)
    {
        if (itemToPickup == null) return;

        ItemData itemData = itemToPickup.itemData;

        if (itemData.itemType == ItemType.QuestItem)
        {
            if (gameManager != null && !GameManager.isGameOver)
            {
                GameManager.Instance.IncrementKeysCollected();
                // ����: UI ������Ʈ �� ������ ���� ������ �� ������ �Բ� ����
                itemSlotUI.UpdateKeyCount(gameManager.keysCollected, gameManager.KEYS_TO_SPAWN);
                Destroy(itemToPickup.gameObject);
            }
        }

        else // ��Ÿ ������ ȹ�� ����
        {
            // ����ִ� ������ ã��
            int targetSlotIndex = FindEmptySlot();

            // ����ִ� ������ ���� ��� ���� ������ �����۰� ��ü
            if (targetSlotIndex == -1)
            {
                if (currentEquippedIndex != -1)
                {
                    targetSlotIndex = currentEquippedIndex;
                }
                else
                {
                    Debug.Log("������ ������ ���� á���ϴ�.");
                    return;
                }
            }


            // �������� �κ��丮�� �߰��ϰ� UI�� ������Ʈ
            itemSlots[targetSlotIndex] = itemData;
            itemSlotUI.UpdateSlotUI(targetSlotIndex, itemData.itemIcon);


            // ȹ���� �������� �ٷ� ����
            EquipItem(targetSlotIndex);


            Debug.Log($"'{itemData.itemName}'��(��) ȹ���߽��ϴ�.");
            Destroy(itemToPickup.gameObject);

        }
    }


    // ����ִ� �κ��丮 ������ �ε����� ã�� ��ȯ
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


    // �κ��丮�� Ư�� �ε����� �ִ� �������� ����
    public void EquipItem(int index)
    {
        if (itemSlots == null) return;
        if (index < 0 || index >= itemSlots.Length) return;
        if (itemSlots[index] == null) return;


        // ������ ������ �������� ������ �ı�
        if (currentEquippedPrefab != null)
        {
            Destroy(currentEquippedPrefab);
        }


        // �� �������� �����ϰ� �������� ����
        currentEquippedIndex = index;
        currentEquippedPrefab = Instantiate(itemSlots[index].itemPrefab, itemEquipPoint.position, itemEquipPoint.rotation, itemEquipPoint);
        itemSlotUI.SelectSlot(index);


        Debug.Log($"'{itemSlots[index].itemName}'��(��) �����߽��ϴ�.");

    }


    // ������ ��� ��ư(��Ŭ��)�� ������ �� ȣ��
    private void OnUseItemPerformed(InputAction.CallbackContext context)
    {
        if (currentEquippedIndex == -1) return;

        ItemData equippedItem = itemSlots[currentEquippedIndex];

        // ���� ������(������) ���
        if (equippedItem?.itemType == ItemType.Tool)
        {
            if (playerController != null)
            {
                playerController.isUsingItem = true;
            }

            if (currentEquippedPrefab != null)
            {
                FlashlightHandler handler = currentEquippedPrefab.GetComponent<FlashlightHandler>();
                if (handler != null)
                {
                    handler.Use();
                }
            }
        }


        // �Ҹ� ������(�ν���) ���
        else if (equippedItem?.itemType == ItemType.Consume)
        {
            if (playerController != null)
            {
                BoosterData boosterData = (BoosterData)equippedItem;
                playerController.ActivateBoosterEffect(boosterData.boosterDuration, boosterData.speedMultiplier);

                // ���� �������� �κ��丮���� �����ϰ� UI�� ������Ʈ
                itemSlots[currentEquippedIndex] = null;
                itemSlotUI.ClearSlotUI(currentEquippedIndex);
                Destroy(currentEquippedPrefab);
                currentEquippedIndex = -1;
            }
        }
    }



    // ������ ��� ��ư(��Ŭ��)�� ���� �� ȣ��
    private void OnUseItemCanceled(InputAction.CallbackContext context)
    {
        if (currentEquippedIndex != -1 && itemSlots[currentEquippedIndex]?.itemType == ItemType.Tool)
        {
            if (playerController != null)
            {
                playerController.isUsingItem = false;
            }
        }
    }
}