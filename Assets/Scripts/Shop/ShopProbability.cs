
using System.Collections.Generic;

public static class ShopProbability
{
    private static readonly Dictionary<int, float[]> _probTable = new Dictionary<int, float[]>
    {
        {1, new float[]{100f, 0f, 0f}},
        {2, new float[]{100f, 0f, 0f}},
        {3, new float[]{87.5f, 12.5f, 0f}},
        {4, new float[]{70f, 30f, 0f}},
        {5, new float[]{60.5f, 39.5f, 0f}},
        {6, new float[]{47.5f, 52.5f, 0f}},
        {7, new float[]{28f, 72f, 0f}},
        {8, new float[]{14f, 86f, 0f}},
        {9, new float[]{0f, 87f, 13f}},
        {10,new float[]{0f, 50f, 50f}}
    };

    public static float[] GetProbabilities(int level)
    {
        return _probTable.ContainsKey(level) ? _probTable[level] : new float[] { 100f, 0f, 0f };
    }


}
