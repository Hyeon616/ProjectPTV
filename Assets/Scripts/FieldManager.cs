using UnityEngine;

public class FieldManager
{

    private GameObject _playerFieldPrefab;
    private GameObject _enemyFieldPrefab;
    public Transform _parent;

    private Tile[,] _field;

    private float _cellSizeX;
    private float _cellSizeY;


    public FieldManager(GameObject playerField, GameObject enemeyField, float cellSizeX, float cellSizeY, Transform parent)
    {
        _playerFieldPrefab = playerField;
        _enemyFieldPrefab = enemeyField;
        _cellSizeX = cellSizeX;
        _cellSizeY = cellSizeY;
        _parent = parent;
    }

    public void GenerateField(int rows, int cols)
    {
        _field = new Tile[rows, cols];

        for (int x = 0; x < rows; x++)
        {
            for (int y = 0; y < cols; y++)
            {
                GameObject fieldTiles = (x <= 4) ? _playerFieldPrefab : _enemyFieldPrefab;

                Vector3 pos = new Vector3((x - y) * _cellSizeX / 2f, (x + y) * _cellSizeY / 2f, 0);

                GameObject tile = Object.Instantiate(fieldTiles, _parent);
                tile.transform.localPosition = pos;
                tile.name = $"{x},{y}";

                Tile t = tile.AddComponent<Tile>();
                t._isPlayerField = (x <= 4);
                _field[x, y] = t;

                //tile.SetActive(false);
                tile.GetComponent<SpriteRenderer>().enabled = false;

            }
        }
    }



    public void ShowField()
    {
        foreach (var field in _field)
        {
            field.gameObject.SetActive(true);
        }
    }

    public void HideField()
    {
        foreach (var field in _field)
        {
            field.gameObject.SetActive(false);
        }
    }

    public Tile GetTile(int x, int y)
    {

        return _field[x, y];
    }


}
