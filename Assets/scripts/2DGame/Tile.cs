using UnityEngine;

public enum Sides
{
    None = -1,
    Top,  
    Left,   
    Right,   
    Bottom,    
}
public class Tile 
{
    public int id;
    public Tile[] adjacents = new Tile[4];             
    public int autoTileId;
    public bool isVisited = false;
    public int distance;
    public bool CanMove => autoTileId != (int)TileType.Empty;

    public void UpdateAutoTileId()
    {
        autoTileId = 0;
        for (int i = 0; i < adjacents.Length; i++)
        {
            if (adjacents[i] != null)
            {
                // i = 3일때 : 1000
                // i = 2: 0100
                // i = 1: 0010
                // i = 0: 0001
                autoTileId |= 1 << i;
            }
        }
    }
    public void RemoveAdjacents(Tile tile)
    {
        for (int i = 0; i < adjacents.Length; i++)
        {
            if (adjacents[i] == null)
                continue;
            if (adjacents[i].id == tile.id)
            {
                adjacents[i] = null;
                UpdateAutoTileId();
                break;
            }

        }
    }
    public void ClearAdjacents()
    {
        for (int i = 0; i < adjacents.Length; i++)
        {
            if (adjacents[i] == null)
            {
                continue;
            }
            adjacents[i].RemoveAdjacents(this);
            adjacents[i] = null;
        }
        UpdateAutoTileId();
    }
}
