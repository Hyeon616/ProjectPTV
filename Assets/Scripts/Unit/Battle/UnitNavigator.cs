using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitNavigator 
{
    private FieldManager _field;

    public UnitNavigator(FieldManager field)
    {
        _field = field;
    }

    // 후보 타일이 타깃의 어느 면에 가까운지에 따라 우선순위(낮을수록 우선).
    // Player: S(0)→E(1)→W(2)→N(3)
    // Enemy : N(3)→E(1)→W(2)→S(0)
    public int DirectionPriority(Tile candidate, Tile target, Owner owner)
    {
        int dx = candidate.X - target.X; // x+:N, x-:S
        int dy = candidate.Y - target.Y; // y+:W, y-:E

        int side; // 0:S, 1:E, 2:W, 3:N
        if (Mathf.Abs(dx) >= Mathf.Abs(dy))
            side = (dx < 0) ? 0 : 3;
        else
            side = (dy < 0) ? 1 : 2;

        return owner switch
        {
            Owner.Player => side switch { 0 => 0, 1 => 1, 2 => 2, _ => 3 },
            Owner.Enemy => side switch { 3 => 0, 1 => 1, 2 => 2, _ => 3 },
            _ => 10
        };
    }

    /// <summary>
    /// self가 타깃을 향해 한 칸 이동할 다음 타일(step)을 반환.
    /// - BFS로 최단거리 계산
    /// - 사거리 이내 & 점유 가능 타일들 중 최단거리 우선, 동점이면 DirectionPriority로 tie-break
    /// - 최종 목적지까지 parent 추적으로 '첫 한 칸' 반환
    /// </summary>
    public Tile FindNextTileTowardTarget(Unit self)
    {
        if (self == null) return null;
        var current = self.CurrentTileRef;
        var targetU = self.TargetRef;
        if (current == null || targetU == null || targetU.CurrentTileRef == null) return null;

        Tile targetTile = targetU.CurrentTileRef;

        int rows = _field.Rows;
        int cols = _field.Cols;
        int range = self.UnitState.UnitStats._attackRange;

        // BFS
        bool[,] visited = new bool[rows, cols];
        int[,] dist = new int[rows, cols];
        for (int x = 0; x < rows; x++)
            for (int y = 0; y < cols; y++)
                dist[x, y] = int.MaxValue;

        Tile[,] parent = new Tile[rows, cols];
        Queue<Tile> q = new Queue<Tile>();

        visited[current.X, current.Y] = true;
        dist[current.X, current.Y] = 0;
        q.Enqueue(current);

        int[] dx4 = { 1, -1, 0, 0 };
        int[] dy4 = { 0, 0, 1, -1 };

        while (q.Count > 0)
        {
            Tile cur = q.Dequeue();
            for (int i = 0; i < 4; i++)
            {
                int nx = cur.X + dx4[i];
                int ny = cur.Y + dy4[i];
                if (nx < 0 || nx >= rows || ny < 0 || ny >= cols) continue;
                if (visited[nx, ny]) continue;

                Tile nxt = _field.GetTile(nx, ny);
                if (!nxt.IsFreeFor(self)) continue;

                visited[nx, ny] = true;
                dist[nx, ny] = dist[cur.X, cur.Y] + 1;
                parent[nx, ny] = cur;
                q.Enqueue(nxt);
            }
        }

        // 사거리 이내 & 점유 가능 후보 중 최단거리 → 면 우선순위
        int bestDist = int.MaxValue;
        int bestDirScore = int.MaxValue;
        Tile bestGoal = null;

        for (int x = 0; x < rows; x++)
            for (int y = 0; y < cols; y++)
            {
                if (dist[x, y] == int.MaxValue) continue;

                int mdToTarget = Mathf.Abs(x - targetTile.X) + Mathf.Abs(y - targetTile.Y);
                if (mdToTarget > range) continue;

                Tile cand = _field.GetTile(x, y);
                if (!cand.IsFreeFor(self)) continue;

                int d = dist[x, y];
                if (d < bestDist)
                {
                    bestDist = d; bestGoal = cand;
                    bestDirScore = DirectionPriority(cand, targetTile, self.UnitState.Owner);
                }
                else if (d == bestDist)
                {
                    int dirScore = DirectionPriority(cand, targetTile, self.UnitState.Owner);
                    if (dirScore < bestDirScore)
                    {
                        bestDirScore = dirScore;
                        bestGoal = cand;
                    }
                }
            }

        if (bestGoal == null || bestGoal == current) return null;

        // 첫 한 칸 되짚기
        Tile step = bestGoal;
        Tile prev = parent[step.X, step.Y];
        while (prev != null && prev != current)
        {
            step = prev;
            prev = parent[step.X, step.Y];
        }
        return step;
    }
}
