using UnityEngine;

public class FieldSceneManager : MonoBehaviour
{
    [SerializeField] private GameObject _playerFieldPrefab;
    [SerializeField] private GameObject _enemyFieldPrefab;

    [Header("Field Settings")]
    [SerializeField] private Field _field;
    [SerializeField] private Grid _grid;
    private FieldManager _fieldManager;
    private UnitManager _unitManager;

    public UnitManager UnitManager => _unitManager;
    public FieldManager FieldManager => _fieldManager;

    private int _rows = 10;
    private int _cols = 8;


    void Start()
    {
        _fieldManager = new FieldManager(_playerFieldPrefab, _enemyFieldPrefab, _grid.cellSize.x, _grid.cellSize.y, _field.transform);
        _unitManager = new UnitManager();

        _fieldManager.GenerateField(_rows, _cols);

        _unitManager.SpawnUnit(StageManager._instance.UnitDB.GetUnitData(UnitType.Knight), Owner.Player, _fieldManager.GetTile(0, 3));

    }



}
