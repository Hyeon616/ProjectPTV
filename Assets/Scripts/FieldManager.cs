using UnityEngine;

public class FieldManager
{

    private GameObject _tilePrefab;

    public Transform _parent;

    private Tile[,] _field;

    private float _cellSizeX;
    private float _cellSizeY;

    private readonly Color _playerFieldColor = new Color(1f, 1f, 1f); // white
    private readonly Color _enemyFieldColor = new Color(0f, 0.78f, 0.75f); // blue


    public FieldManager(GameObject tilePrefab, float cellSizeX, float cellSizeY, Transform parent)
    {

        _tilePrefab = tilePrefab;
        _cellSizeX = cellSizeX;
        _cellSizeY = cellSizeY;
        _parent = parent;
    }

    public void GenerateField(int rows, int cols, int layer)
    {
        _field = new Tile[rows, cols];

        for (int x = 0; x < rows; x++)
        {
            for (int y = 0; y < cols; y++)
            {
                Vector3 pos = new Vector3((x - y) * _cellSizeX / 2f, (x + y) * _cellSizeY / 2f, 0);

                GameObject tile = Object.Instantiate(_tilePrefab, _parent);
                tile.transform.localPosition = pos;
                tile.name = $"{x},{y}";
                tile.layer = layer;
                tile.AddComponent<PolygonCollider2D>();

                // Collider
                PolygonCollider2D poly = tile.GetComponent<PolygonCollider2D>();
                Vector2[] points = new Vector2[4];
                points[0] = new Vector2(0, 0.25f);
                points[1] = new Vector2(0.5f, 0);
                points[2] = new Vector2(0, -0.25f);
                points[3] = new Vector2(-0.5f, 0);
                poly.points = points;

                // Tile 스크립트 설정
                Tile t = tile.AddComponent<Tile>();
                bool isPlayerField = (x <= 4);
                t.Init(x, y, isPlayerField);

                _field[x, y] = t;

                // 타일 색상 적용
                SpriteRenderer renderer = tile.GetComponent<SpriteRenderer>();
                if (renderer != null)
                    renderer.color = t.IsPlayerField ? _playerFieldColor : _enemyFieldColor;

                // 타일 숨김
                renderer.enabled = false;


            }
        }
    }



    public void ShowField()
    {
        foreach (var field in _field)
        {
            field.GetComponent<SpriteRenderer>().enabled = true;
        }
    }

    public void HideField()
    {
        foreach (var field in _field)
        {
            field.GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    public Tile GetTile(int x, int y)
    {

        return _field[x, y];
    }


}
