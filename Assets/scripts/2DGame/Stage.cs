using UnityEngine;
using System.Collections.Generic;

public class Stage : MonoBehaviour
{
    public GameObject tilePrefabs;
    private GameObject[] tileObjs;

    public int mapWidth = 20;
    public int mapHeight = 20;

    [Range(0f, 0.9f)]
    public float erodePercent = 0.5f;
    public int erodeIterations = 2;
    public float lakePercent;
    public float treePercent;
    public float hillPercent;
    public float mountainPercent;
    public float townPercent;
    public float monsterPercent;


    public Vector2 tileSize = new Vector2(16, 16);
    public Sprite[] islandSprites;
    public Sprite[] fowSprites;
    private Map map;
    public Map Map => map;
    private Camera mainCamera;
    private int prevTileId = -1;
    public PlayerMovement playerPrefab;
    private PlayerMovement player;
    public Vector3 FirstTilePos
    {
        get
        {
            var pos = transform.position;
            pos.x -= (mapWidth * tileSize.x * 0.5f);
            pos.y += (mapHeight * tileSize.y * 0.5f);
            pos.x -= tileSize.x * 0.5f;
            pos.y += tileSize.y * 0.5f;
            return pos;
        }
    }
    private void Start()
    {
        mainCamera = Camera.main;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ResetStage();
        }
        if (tileObjs != null)
        {
            int currentTileId = ScreenPosToTileId(Input.mousePosition);
            if (prevTileId != currentTileId)
            {
                tileObjs[currentTileId].GetComponent<SpriteRenderer>().color = Color.green;
                if (prevTileId >= 0 && prevTileId < tileObjs.Length)
                {
                    tileObjs[prevTileId].GetComponent<SpriteRenderer>().color = Color.white;
                }
                prevTileId = currentTileId;
            }
        }
    }
    private void ResetStage()
    {
        map = new Map();
        map.Init(mapHeight, mapWidth);
        map.CreateIsland(erodePercent, erodeIterations, lakePercent, treePercent,
            hillPercent, mountainPercent, townPercent, monsterPercent);
        CreateGrid();
        CreatePlayer();
    }
    private void CreatePlayer()
    {
        if (player != null)
        {
            Destroy(player.gameObject);
        }

        player = Instantiate(playerPrefab);
        player.MoveTo(map.startTile.id);
    }
    private void CreateGrid()
    {
        //초기화 먼저
        if (tileObjs != null)
        {
            foreach (var tile in tileObjs)
            {
                Destroy(tile.gameObject);
            }
        }
        tileObjs = new GameObject[mapWidth * mapHeight];

        var position = FirstTilePos;
        for (int i = 0; i < mapHeight; i++)
        {
            for (int j = 0; j < mapWidth; j++)
            {
                var tileId = i * mapWidth + j;

                var newGo = Instantiate(tilePrefabs, transform);
                newGo.transform.position = position;
                position.x += tileSize.x;
                tileObjs[tileId] = newGo;
                DecorateTile(tileId);
            }
            position.x = FirstTilePos.x;
            position.y -= tileSize.y;
        }
    }
    public void UpdateVision(int startTileId)
    {
        foreach (var tile in map.tiles)
        {
            tile.distance = int.MaxValue;
            tile.isVisited = false;
        }
        var queue = new Queue<Tile>();
        Tile startTile = map.tiles[startTileId];
        startTile.distance = 0;
        startTile.isVisited = true;
        queue.Enqueue(startTile);

        //캐릭터 부터 3칸 범위 타일만 BFS탐색
        while (queue.Count > 0)
        {
            Tile currentTile = queue.Dequeue();
            if (currentTile.distance >= 3)
                continue;
            foreach (var adjacent in currentTile.adjacents)
            {
                if (adjacent != null && adjacent.isVisited == false)
                {
                    queue.Enqueue(adjacent);
                    adjacent.isVisited = true;
                    adjacent.distance = currentTile.distance + 1;
                }
            }
        }
    }
    //타일 스프라이트 세팅
    public void DecorateTile(int tileId)
    {
        var tile = map.tiles[tileId]; //실제 데이터
        var tileGo = tileObjs[tileId]; //껍데기
        var ren = tileGo.GetComponent<SpriteRenderer>();

        //캐릭터 부터 3칸 범위 타일
        if (tile.distance <= 3)
        {
            if (tile.autoTileId != (int)TileType.Empty)
                ren.sprite = islandSprites[tile.autoTileId];

            else
                ren.sprite = null;
        } //3칸 범위 초과 타일 (안개)
        else if (tile.distance >= 4)
        {
            ren.sprite = fowSprites[15];
        }

    }





public int ScreenPosToTileId(Vector3 screenPos)
{
    screenPos.z = Mathf.Abs(transform.position.z - mainCamera.transform.position.z);
    return WorldPosToTileId(mainCamera.ScreenToWorldPoint(screenPos));
}
public int WorldPosToTileId(Vector3 worldPos)
{
    var first = FirstTilePos;
    int x = Mathf.FloorToInt((worldPos.x - first.x) / tileSize.x + 0.5f);
    int y = Mathf.FloorToInt((first.y - worldPos.y) / tileSize.y + 0.5f);
    x = Mathf.Clamp(x, 0, mapWidth - 1);
    y = Mathf.Clamp(y, 0, mapHeight - 1);
    return y * mapWidth + x;
}
public Vector3 GetTilePos(int tileId)
{
    var pos = Vector3.zero;

    var y = tileId / mapWidth;
    var x = tileId % mapWidth;

    return GetTilePos(y, x);
}
public Vector3 GetTilePos(int y, int x)
    => FirstTilePos + new Vector3(x * tileSize.x, -y * tileSize.y);


}
