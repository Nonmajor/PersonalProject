using UnityEngine;

// Asset �޴��� ���� �ɼ��� �߰���
[CreateAssetMenu(fileName = "New Item Data", menuName = "Item/Base Item Data", order = 0)]
public class ItemData : ScriptableObject
{
    // �������� �̸�
    public string itemName;
    // �������� Ÿ�� (����, �Ҹ�ǰ ��)
    public ItemType itemType;
    // ������ ȹ�� �� �÷��̾ ���� �� �𵨸� ������
    public GameObject itemPrefab;
    // UI�� ǥ�õ� ������ ������
    public Sprite itemIcon;
}