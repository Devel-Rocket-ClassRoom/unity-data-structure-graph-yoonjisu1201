using System.Linq;
using UnityEngine;

public enum TileType
{
    Empty = -1,
    Grass = 15,
    Tree,
    Hills,
    Mountains,
    Towns,
    Castle,
    Monster,
}
public class Map
{
    public int rows = 0;
    public int cols = 0;
    public Tile[] tiles;
    public Tile[] CoastTiles => tiles.Where(t => t.autoTileId >= 0 && t.autoTileId < (int)TileType.Grass).ToArray();
    public Tile[] LandTiles => tiles.Where(t => t.autoTileId == (int)TileType.Grass).ToArray();
    public Tile startTile;
    public Tile castleTile;
    

    public void Init(int rows, int cols)
    {
        this.rows = rows;
        this.cols = cols;
        tiles = new Tile[rows * cols];
        for (int i = 0; i < tiles.Length; i++)
        {
            tiles[i] = new Tile();
            tiles[i].id = i;
        }
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                var index = r * cols + c;
                var adjacents = tiles[index].adjacents;
                if ((r - 1) >= 0)
                {
                    adjacents[(int)Sides.Top] = tiles[index - cols];
                }
                if ((c + 1) < cols)
                {
                    adjacents[(int)Sides.Right] = tiles[index + 1];
                }
                if ((c - 1) >= 0)
                {
                    adjacents[(int)Sides.Left] = tiles[index - 1];
                }
                if ((r + 1) < rows)
                {
                    adjacents[(int)Sides.Bottom] = tiles[index + cols];
                }
            }
        }
        for (int i = 0; i < tiles.Length; i++)
        {
            tiles[i].UpdateAutoTileId();
        }
    }
    public void ShuffleTiles(Tile[] tiles)
    {
        for (int i = tiles.Length - 1; i > 0; --i)
        {
            int rand = Random.Range(0, i + 1);
            (tiles[rand], tiles[i]) = (tiles[i], tiles[rand]);
        }
    }
    public void DecorateTiles(Tile[] tiles, float percent, TileType tileType)
    {
        ShuffleTiles(tiles);

        int total = Mathf.FloorToInt(tiles.Length * percent);
        for (int i = 0; i < total; i++)
        {
            //if (tileType == TileType.Empty)
            //{
            //    tiles[i].ClearAdjacents();
            //}
            tiles[i].autoTileId = (int)tileType;
        }
    }
    public bool CreateIsland(
        float erodePercent, //해안선 타일에서 퍼센트만큼 지우려고 만든 매개변수?
        int erodeIterations,
        float lakePercent,
        float treePercent,
        float hillPercent,
        float mountainPercent,
        float townPercent,
        float monsterPercent) //Castle은 1개만
    {
        for (int i = 0; i < erodeIterations; i++)
        {
            DecorateTiles(CoastTiles, erodePercent, TileType.Empty);
        }

        DecorateTiles(LandTiles, lakePercent, TileType.Empty);
        DecorateTiles(LandTiles, treePercent, TileType.Tree);
        DecorateTiles(LandTiles, hillPercent, TileType.Hills);
        DecorateTiles(LandTiles, mountainPercent, TileType.Mountains);
        DecorateTiles(LandTiles, townPercent, TileType.Towns);
        DecorateTiles(LandTiles, monsterPercent, TileType.Monster);
        var towns = tiles.Where(x => x.autoTileId == (int)TileType.Towns).ToArray();
        ShuffleTiles(towns);
        startTile = towns[0];
        castleTile = towns[1];
        towns[0].autoTileId = (int)TileType.Castle;
        return true;
    }
}
