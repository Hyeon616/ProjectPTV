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

        for (int i = 0; i < _playerUnits.Count; i++)
            _playerUnits[i]?.StateUpdate();
        for (int i = 0; i < _enemyUnits.Count; i++)
            _enemyUnits[i]?.StateUpdate();

    }

    private IEnumerator WaveLoop()
    {
        StageWaveData stageData = _stageWaveData[_selectedStage];

        _currentWave = 0;

        for (int i = 0; i < stageData._waves.Length; i++)
        {
            yield return new WaitForSeconds(_waveInterval);
            yield return StartCoroutine(StartWave(stageData._waves[i]));
            _currentWave = i + 1;
        }

        EndStage();
    }


    private IEnumerator StartWave(WaveData waveData)
    {
        // 1) 적 스폰
        SpawnEnemyWave(waveData);

        // 2) 전장에 존재하는 유닛 목록 갱신
        AddFieldUnits();

        // 3) 전투 시작 플래그
        IsBattle = true;

        // 4) 모든 유닛을 Idle로 세팅(유닛 FSM이 스스로 타겟을 찾고 Chase/Attack으로 전이)
        foreach (var u in _playerUnits)
        {
            if (u != null)
            {
                u.RequestState(UnitActionState.Idle);
                u.StateUpdate();
            }

        }
        foreach (var u in _enemyUnits)
        {
            if (u != null)
            {
                u.RequestState(UnitActionState.Idle);
                u.StateUpdate();
            }

        }
        

        // 5) 웨이브가 끝날 때까지 대기
        while (!IsBattleFinished())
            yield return null;

        // 6) 웨이브 종료 처리
        OnWaveFinished();
        IsBattle = false;

        yield break;
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
        foreach (var tile in _fieldSceneManager.FieldManager.GetAllUnits())
        {
            if (tile.Unit != null && tile.Unit.UnitState.Owner == Owner.Player)
                tile.Unit.ResetWave();
        }

        _enemyUnits.Clear();
    }

    private void EndStage()
    {
        Debug.Log("stage끝");
        //TODO 스테이지 클리어 UI/보상 등 처리
    }
}
