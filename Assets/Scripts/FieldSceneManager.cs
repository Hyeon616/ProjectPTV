using UnityEngine;

public enum LayerNum
{
    Default = 0,
    TransparentFX = 1,
    IgnoreRaycast = 2,
    Water = 4,
    UI = 5,
    Tile = 6,
    Unit = 7,
}


public class FieldSceneManager : MonoBehaviour
{
    [SerializeField] private GameObject _tilePrefab;

    [Header("Field Settings")]
    [SerializeField] private Field _field;
    [SerializeField] private Grid _grid;

    private FieldManager _fieldManager;
    private UnitManager _unitManager;
    private UnitFactory _unitFactory;

    public UnitManager UnitManager => _unitManager;
    public FieldManager FieldManager => _fieldManager;

    private readonly int _rows = 10;
    private readonly int _cols = 8;

    private void Awake()
    {
        _fieldManager = new FieldManager(_tilePrefab, _grid.cellSize.x, _grid.cellSize.y, _field.transform);
         _unitManager = new UnitManager();
    }

    void Start()
    {
        _fieldManager.GenerateField(_rows, _cols, (int)LayerNum.Tile);
        

        // test РЏДж
        _unitManager.SpawnUnit(StageManager._instance.UnitDB.GetUnitData(UnitType.Knight), Owner.Player, _fieldManager.GetTile(0, 3), (int)LayerNum.Unit);
        


    }



}
