using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    [SerializeField] private FieldSceneManager _fieldSceneManager;
    [SerializeField] private List<StageWaveData> _stageWaveData = new List<StageWaveData>();

    private readonly float _waveInterval = 5f;

    private int _selectedStage = 0;
    private int _currentWave = 0;

    public bool IsBattle { get; private set; }

    private List<Unit> _playerUnits = new List<Unit>();
    private List<Unit> _enemyUnits = new List<Unit>();


    public void SetStageIndex(int stage)
    {
        _selectedStage = Mathf.Clamp(stage, 0, _stageWaveData.Count - 1);
    }

    private void Start()
    {
        StartCoroutine(WaveLoop());
    }

    private void Update()
    {
        if (!IsBattle) return;

        foreach (var unit in _playerUnits)
            unit?.StateUpdate();

        foreach (var unit in _enemyUnits)
            unit?.StateUpdate();

    }

    private IEnumerator WaveLoop()
    {
        StageWaveData stageData = _stageWaveData[_selectedStage];

        while (_currentWave <= stageData._waves.Length)
        {
            yield return new WaitForSeconds(_waveInterval);
            yield return StartCoroutine(StartWave(stageData._waves[_currentWave]));

        }

        EndStage();
    }


    public IEnumerator StartWave(WaveData waveData)
    {
        _currentWave++;
        SpawnEnemyWave(waveData);
        IsBattle = true;

        AddFieldUnits();

        foreach (var unit in _playerUnits)
        {
            unit?.FindTarget();
        }

        foreach (var unit in _enemyUnits)
        {
            unit?.FindTarget();
        }

        while (!IsBattleFinished())
            yield return null;

        OnWaveFinished();
        IsBattle = false;
    }

    private void AddFieldUnits()
    {
        _playerUnits.Clear();
        _enemyUnits.Clear();

        foreach (var tile in _fieldSceneManager.FieldManager.GetAllUnits())
        {
            if (tile.Unit == null)
                continue;

            if (tile.Unit.UnitState.Owner == Owner.Player)
            {
                _playerUnits.Add(tile.Unit);
                tile.Unit.SpawnTile(tile);
            }
            else if (tile.Unit.UnitState.Owner == Owner.Enemy)
                _enemyUnits.Add(tile.Unit);
        }
    }


    private void SpawnEnemyWave(WaveData waveData)
    {
        foreach (var enemy in waveData.enemies)
        {
            Tile tile = _fieldSceneManager.FieldManager.GetTile(enemy._gridPos.x, enemy._gridPos.y);
            _fieldSceneManager.UnitManager.SpawnUnitCoordinate(enemy._enemyUnit, Owner.Enemy, tile, (int)LayerNum.Unit, enemy.grade);
        }
    }

    private bool IsBattleFinished()
    {
        foreach (var tile in _fieldSceneManager.FieldManager.GetAllUnits())
        {
            if (tile.Unit != null &&
                tile.Unit.UnitState.Owner == Owner.Enemy &&
                !tile.Unit.UnitState.IsDead)
                return false;
        }

        return true;
    }

    private void OnWaveFinished()
    {
        foreach (var tile in _fieldSceneManager.FieldManager.GetAllUnits())
        {
            if (tile.Unit != null && tile.Unit.UnitState.Owner == Owner.Player)
            {
                tile.Unit.ResetWave();
            }
        }
    }

    private void EndStage()
    {
        // TODO
        Debug.Log("stage³¡");
    }

}
