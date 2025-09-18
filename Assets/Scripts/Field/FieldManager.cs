using System.Collections.Generic;
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

    private readonly List<Tile> _tilePriority = new List<Tile>();

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


                // Collider
                PolygonCollider2D poly = tile.AddComponent<PolygonCollider2D>();
                poly.points = new Vector2[]
                {
                    new Vector2(0, 0.25f),
                    new Vector2(0.5f, 0),
                    new Vector2(0, -0.25f),
                    new Vector2(-0.5f, 0)
                };

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

    public void GenerateTilePriority(int rows, int cols)
    {
        int startX = rows / 4;
        int startY = cols / 2;

        bool[,] visited = new bool[rows, cols];
        Queue<(int x, int y)> queue = new Queue<(int, int)>();
        queue.Enqueue((startX, startY));
        visited[startX, startY] = true;

        int[] dx = { 1, -1, 0, 0 };
        int[] dy = { 0, 0, 1, -1 };

        while (queue.Count > 0)
        {
            var (x, y) = queue.Dequeue();
            if (_field[x, y].IsPlayerField)
                _tilePriority.Add(_field[x, y]);

            for (int i = 0; i < 4; i++)
            {
                int nx = x + dx[i];
                int ny = y + dy[i];

                if (nx >= 0 && nx < rows && ny >= 0 && ny < cols && !visited[nx, ny])
                {
                    visited[nx, ny] = true;
                    queue.Enqueue((nx, ny));
                }
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

    public Tile GetTile(int x, int y) => _field[x, y];

    public Tile FindTilePriority()
    {
        foreach (var tile in _tilePriority)
        {
            if (tile.Unit == null)
                return tile;
        }
        return null;
    }

    public IEnumerable<Tile> GetAllUnits()
    {
        foreach (var tile in _field)
        {
            yield return tile;
        }
    }

}
