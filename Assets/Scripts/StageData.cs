using System;
using UnityEngine;

[CreateAssetMenu(fileName = "StageData", menuName = "Game/StageData")]
public class StageData : ScriptableObject
{
    [Serializable]
    public struct SpawnData
    {
        public UnitType _unitType;
        public Owner _owner;
        public int _x;
        public int _y;

    }

    [Serializable]
    public struct Wave
    {
        public SpawnData[] _spawnDatas;
    }

    public Wave[] _waves;

}
