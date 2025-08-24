using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject _playerFieldPrefab;
    [SerializeField] private GameObject _enemyFieldPrefab;


    [Header("Field Settings")]
    private Field _field;
    private Grid _grid;
    private FieldManager _fieldManager;

    private int _rows = 10;
    private int _cols = 8;

    private void Awake()
    {
        _grid = FindObjectOfType<Grid>();
        _field = FindObjectOfType<Field>();
    }

    void Start()
    {
        _fieldManager = new FieldManager(_playerFieldPrefab, _enemyFieldPrefab, _grid.cellSize.x, _grid.cellSize.y, _field.transform);
        _fieldManager.GenerateField(_rows, _cols);

    }

}
