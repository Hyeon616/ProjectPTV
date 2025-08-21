using UnityEngine;

public class FieldManager : MonoBehaviour
{
    [Header("Field Setting")]
    public GameObject _playerFieldPrefab;
    public GameObject _enemyFieldPrefab;

    public int _rows = 8;
    public int _cols = 10;
    public float _cellWidth = 1f;
    public float _cellHeight = 0.5f;

    private GameObject[,] _FieldTiles;

    void Start()
    {
        GenerateField();
    }


    private void GenerateField()
    {
        _FieldTiles = new GameObject[_rows, _cols];

        for (int y = 0; y < _rows; y++)
        {
            for (int x = 0; x < _cols; x++)
            {
                GameObject fieldTiles = (x <=4) ? _playerFieldPrefab : _enemyFieldPrefab;

                Vector3 pos = new Vector3((x - y) * _cellWidth / 2f, (x + y) * _cellHeight / 2f, 0);

                GameObject tile = Instantiate(fieldTiles, transform);
                tile.transform.localPosition = pos;
                tile.name = $"{x},{y}";
                _FieldTiles[y, x] = tile;

            }
        }
    }

}
