using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections.Generic;

// 플레이어의 아이템 소지, 장착, 사용 및 상호작용을 관리
public class PlayerItemController : MonoBehaviour
{


    // 인벤토리 설정
    [Header("Inventory Settings")]
    public int maxItemSlots = 3; // 인벤토리 슬롯의 최대 개수
    private ItemData[] itemSlots; // 아이템 데이터를 저장하는 배열
    private int currentEquippedIndex = -1; // 현재 장착 중인 아이템의 인덱스 (-1은 장착된 아이템 없음)


    // 아이템 장착
    [Header("Item Equip")]
    public Transform itemEquipPoint; // 아이템 프리팹이 장착될 위치
    private GameObject currentEquippedPrefab; // 현재 장착된 아이템의 프리팹 인스턴스


    // UI
    [Header("UI")]
    public ItemSlotUI itemSlotUI; // 아이템 슬롯 UI를 제어하는 스크립트
    public TextMeshProUGUI interactionText; // 상호작용 텍스트를 표시할 UI
    public Transform playerCamera; // 플레이어 카메라의 Transform (Raycast에 사용)
    public float interactionDistance = 5f; // 아이템을 주울 수 있는 최대 거리


    // 시스템 컴포넌트
    private PlayerInput playerInput; // 유니티 Input System 컴포넌트
    private PlayerController playerController; // 플레이어의 상태를 제어하는 스크립트
    private GameManager gameManager; // 게임 상태를 관리하는 싱글톤 인스턴스



    void Awake()
    {
        // 인벤토리 슬롯 배열을 초기화
        itemSlots = new ItemData[maxItemSlots];
        playerInput = GetComponent<PlayerInput>();
        playerController = GetComponent<PlayerController>();


        // GameManager 인스턴스를 가져옴
        gameManager = GameManager.Instance;
        if (gameManager == null)
        {
            Debug.LogError("GameManager 인스턴스를 찾을 수 없습니다. 씬에 GameManager 오브젝트가 있고 스크립트가 할당되었는지 확인하세요.");
        }

        if (playerController == null)
        {
            Debug.LogError("PlayerController 컴포넌트를 찾을 수 없습니다.");
        }
    }

    void Start()
    {
        // GameManager에서 현재 열쇠 개수와 총 열쇠 개수를 가져와 UI를 업데이트함
        itemSlotUI.UpdateKeyCount(gameManager.keysCollected, gameManager.KEYS_TO_SPAWN);
    }


    // 입력 이벤트 등록 및 해제
    private void OnEnable()
    {
        if (playerInput != null && playerInput.actions != null)
        {
            // Input Action에 대한 콜백 함수를 등록
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
            // 스크립트 비활성화 시 콜백 함수를 해제
            playerInput.actions["Use Item"].performed -= OnUseItemPerformed;
            playerInput.actions["Use Item"].canceled -= OnUseItemCanceled;

            playerInput.actions["Equip_1"].performed -= context => EquipItem(0);
            playerInput.actions["Equip_2"].performed -= context => EquipItem(1);
            playerInput.actions["Equip_3"].performed -= context => EquipItem(2);
        }
    }


    void Update()
    {
        // 매 프레임 상호작용 텍스트를 초기화
        if (interactionText != null)
        {
            interactionText.text = "";
        }


        // 플레이어 카메라에서 전방으로 Raycast를 쏨
        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            PickupItem item = hit.collider.GetComponent<PickupItem>();
            if (item != null)
            {
                // Raycast에 아이템이 감지되면 상호작용 텍스트를 표시함
                if (interactionText != null && item.itemData != null)
                {
                    interactionText.text = $"{item.itemData.itemName} (E)";
                }

                // E 키를 눌러 아이템을 획득
                if (Input.GetKeyDown(KeyCode.E))
                {
                    PickupItem(item);
                }
            }
        }
    }


    //  핵심 기능 함수
    // 아이템을 주워서 인벤토리에 추가
    private void PickupItem(PickupItem itemToPickup)
    {
        if (itemToPickup == null) return;

        ItemData itemData = itemToPickup.itemData;

        if (itemData.itemType == ItemType.QuestItem)
        {
            if (gameManager != null && !GameManager.isGameOver)
            {
                GameManager.Instance.IncrementKeysCollected();
                // 수정: UI 업데이트 시 열쇠의 현재 개수와 총 개수를 함께 전달
                itemSlotUI.UpdateKeyCount(gameManager.keysCollected, gameManager.KEYS_TO_SPAWN);
                Destroy(itemToPickup.gameObject);
            }
        }

        else // 기타 아이템 획득 로직
        {
            // 비어있는 슬롯을 찾음
            int targetSlotIndex = FindEmptySlot();

            // 비어있는 슬롯이 없을 경우 현재 장착된 아이템과 교체
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


            // 아이템을 인벤토리에 추가하고 UI를 업데이트
            itemSlots[targetSlotIndex] = itemData;
            itemSlotUI.UpdateSlotUI(targetSlotIndex, itemData.itemIcon);


            // 획득한 아이템을 바로 장착
            EquipItem(targetSlotIndex);


            Debug.Log($"'{itemData.itemName}'을(를) 획득했습니다.");
            Destroy(itemToPickup.gameObject);

        }
    }


    // 비어있는 인벤토리 슬롯의 인덱스를 찾아 반환
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


    // 인벤토리의 특정 인덱스에 있는 아이템을 장착
    public void EquipItem(int index)
    {
        if (itemSlots == null) return;
        if (index < 0 || index >= itemSlots.Length) return;
        if (itemSlots[index] == null) return;


        // 기존에 장착된 아이템이 있으면 파괴
        if (currentEquippedPrefab != null)
        {
            Destroy(currentEquippedPrefab);
        }


        // 새 아이템을 장착하고 프리팹을 생성
        currentEquippedIndex = index;
        currentEquippedPrefab = Instantiate(itemSlots[index].itemPrefab, itemEquipPoint.position, itemEquipPoint.rotation, itemEquipPoint);
        itemSlotUI.SelectSlot(index);


        Debug.Log($"'{itemSlots[index].itemName}'을(를) 착용했습니다.");

    }


    // 아이템 사용 버튼(좌클릭)을 눌렀을 때 호출
    private void OnUseItemPerformed(InputAction.CallbackContext context)
    {
        if (currentEquippedIndex == -1) return;

        ItemData equippedItem = itemSlots[currentEquippedIndex];

        // 도구 아이템(손전등) 사용
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


        // 소모성 아이템(부스터) 사용
        else if (equippedItem?.itemType == ItemType.Consume)
        {
            if (playerController != null)
            {
                BoosterData boosterData = (BoosterData)equippedItem;
                playerController.ActivateBoosterEffect(boosterData.boosterDuration, boosterData.speedMultiplier);

                // 사용된 아이템을 인벤토리에서 제거하고 UI를 업데이트
                itemSlots[currentEquippedIndex] = null;
                itemSlotUI.ClearSlotUI(currentEquippedIndex);
                Destroy(currentEquippedPrefab);
                currentEquippedIndex = -1;
            }
        }
    }



    // 아이템 사용 버튼(좌클릭)을 뗐을 때 호출
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