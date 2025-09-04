using UnityEngine;

[CreateAssetMenu(fileName = "New Flashlight Data", menuName = "Item/Flashlight Data", order = 1)]
public class FlashlightData : ItemData
{
    // 손전등의 빛에 대한 프리팹 (Light 컴포넌트 포함)
    public GameObject lightPrefab;
    // 빛의 범위와 세기는 프리팹의 Light 컴포넌트에서 직접 조절합니다.
}