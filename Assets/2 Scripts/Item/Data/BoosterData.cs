// BoosterData.cs

using UnityEngine;

[CreateAssetMenu(fileName = "New Booster Data", menuName = "Item/Booster Data", order = 2)]
public class BoosterData : ItemData
{
    // �ν��� ȿ���� ���ӵ� �ð�
    public float boosterDuration = 10f;
    // �÷��̾� �ӵ� ���
    public float speedMultiplier = 2f;
}