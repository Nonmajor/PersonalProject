// BoosterData.cs

using UnityEngine;

[CreateAssetMenu(fileName = "New Booster Data", menuName = "Item/Booster Data", order = 2)]
public class BoosterData : ItemData
{
    // 부스터 효과가 지속될 시간
    public float boosterDuration = 10f;
    // 플레이어 속도 배수
    public float speedMultiplier = 2f;
}