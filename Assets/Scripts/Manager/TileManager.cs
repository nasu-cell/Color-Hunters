using UnityEngine;
using System.Collections.Generic;
public class TileManager : MonoBehaviour
{
    [SerializeField] private List<Tile> tiles = new List<Tile>();

    public int GetOwnedTileCount(int playerId)
    {
        int count = 0;
        foreach(Tile tile in tiles)
        {
            if(tile.OwnerId == playerId)
            {
                count++;
            }
        }
        return count;
    }
}
