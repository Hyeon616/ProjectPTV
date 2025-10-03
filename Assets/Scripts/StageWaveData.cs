using UnityEngine;

[CreateAssetMenu(menuName = "Game/StageWaveData")]
public class StageWaveData : ScriptableObject
{
    public int _stage;
    public WaveData[] _waves;
}

[System.Serializable]
public class WaveData
{
    public int waveNumber;
    public EnemySpawnInfo[] enemies;
}

[System.Serializable]
public struct EnemySpawnInfo
{
    public UnitData _enemyUnit;
    public Vector2Int _gridPos;
    public int grade;

}