using UnityEngine;

[CreateAssetMenu(fileName = "GameConfigSO", menuName = "Config/GameConfigSO")]
public class GameConfigSO : ScriptableObject
{
    [field: SerializeField] public float SpawnTimeInterval { get; set; } = 2;
    [field: SerializeField] public Vector2 MinMaxSpeed { get; set; } = new Vector2(0.25f, 0.8f);
}
