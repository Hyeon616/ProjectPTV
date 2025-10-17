using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    [SerializeField] private FieldSceneManager _fieldSceneManager;
    [SerializeField] private List<StageWaveData> _stageWaveData = new List<StageWaveData>();

    private readonly float _waveInterval = 10f;

    private int _selectedStage = 0;

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

        for (int i = 0; i < _playerUnits.Count; i++)
            _playerUnits[i]?.StateUpdate();
        for (int i = 0; i < _enemyUnits.Count; i++)
            _enemyUnits[i]?.StateUpdate();

    }

    private IEnumerator WaveLoop()
    {
        StageWaveData stageData = _stageWaveData[_selectedStage];

        for (int i = 0; i < stageData._waves.Length; i++)
        {
            yield return new WaitForSeconds(_waveInterval);
            yield return StartCoroutine(StartWave(stageData._waves[i]));
        }

        EndStage();
    }


    private IEnumerator StartWave(WaveData waveData)
    {
        SpawnEnemyWave(waveData);
        IsBattle = true;

        yield return null;

        AddFieldUnits();


        foreach (var u in _playerUnits)
        {
            if (u != null)
            {
                if (u == null) 
                    continue;

                u.Target = null;
                u.RequestState(UnitActionState.Chase);
                u.StateUpdate();
            }
        }
        foreach (var u in _enemyUnits)
        {
            if (u != null)
            {
                if (u == null) 
                    continue;

                u.Target = null;
                u.RequestState(UnitActionState.Chase);
                u.StateUpdate();
            }

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
            if (tile.Unit == null) continue;

            if (tile.Unit.UnitState.Owner == Owner.Player)
            {
                _playerUnits.Add(tile.Unit);
                tile.Unit.SpawnTile(tile);
            }
            else if (tile.Unit.UnitState.Owner == Owner.Enemy)
            {
                _enemyUnits.Add(tile.Unit);
            }
        }
    }

    private void SpawnEnemyWave(WaveData waveData)
    {
        foreach (var enemy in waveData.enemies)
        {
            Tile tile = _fieldSceneManager.FieldManager.GetTile(enemy._gridPos.x, enemy._gridPos.y);
            _fieldSceneManager.UnitManager
                .SpawnUnitCoordinate(enemy._enemyUnit, Owner.Enemy, tile, (int)LayerNum.Unit, enemy.grade);
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
        for (int i = 0; i < _playerUnits.Count; i++)
        {
            var u = _playerUnits[i];
            if (u != null)
                u.ResetWave();
        }

        foreach (var tile in _fieldSceneManager.FieldManager.GetAllUnits())
        {
            if (tile.Unit != null && tile.Unit.UnitState.Owner == Owner.Enemy)
            {
                
                tile.Unit.gameObject.SetActive(false);
                tile.ClearUnit();
            }
        }

        _enemyUnits.Clear();
    }

    private void EndStage()
    {
        Debug.Log("stage끝");
        //TODO 스테이지 클리어 UI/보상 등 처리
    }
}
