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

    [SerializeField] private ShopUI _shopUI;
    [SerializeField] private BenchUI _benchUI;
    [SerializeField] private UnitDB _unitDB;

    private FieldManager _fieldManager;
    private UnitManager _unitManager;
    private ShopManager _shopManager;

    public UnitManager UnitManager => _unitManager;
    public FieldManager FieldManager => _fieldManager;
    public ShopManager ShopManager => _shopManager;
    public BenchUI BenchUI => _benchUI;

    private readonly int _rows = 10;
    private readonly int _cols = 8;

    private void Awake()
    {

        _fieldManager = new FieldManager(_tilePrefab, _grid.cellSize.x, _grid.cellSize.y, _field.transform);
        _unitManager = new UnitManager(_fieldManager, _benchUI);
        _shopManager = new ShopManager(_unitManager, _unitDB);

    }

    void Start()
    {
        _fieldManager.GenerateField(_rows, _cols, (int)LayerNum.Tile);
        _fieldManager.GenerateTilePriority(_rows, _cols);

        // test РЏДж
        var unitdata1 = _unitDB.GetUnitData(UnitType.Knight);
        var unitdata2 = _unitDB.GetUnitData(UnitType.Archer);

        _unitManager.SpawnUnitCoordinate(unitdata1, Owner.Player, _fieldManager.GetTile(0, 3), (int)LayerNum.Unit, 3);
        _unitManager.SpawnUnitCoordinate(unitdata1, Owner.Player, _fieldManager.GetTile(0, 2), (int)LayerNum.Unit, 2);

        _unitManager.SpawnUnit(unitdata2, Owner.Player, (int)LayerNum.Unit, 2);
        _unitManager.SpawnUnit(unitdata2, Owner.Player, (int)LayerNum.Unit, 1);
        _unitManager.SpawnUnit(unitdata2, Owner.Player, (int)LayerNum.Unit, 3);

        _shopUI.Init(_shopManager);
        _shopUI.ShowShop();

    }



}
