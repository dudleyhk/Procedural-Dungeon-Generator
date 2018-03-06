using System.Collections.Generic;
using UnityEngine;


[System.Flags]
public enum TileType
{
    Wall = 1,
    Floor = 2,
    Player = 4,
    OutterWall = 8,
    Item = 16,
    Exit = 32,
    Empty = 64,
    Enemy = 128
}

public class BoardManager : MonoBehaviour
{
    [Header("Sprite Prefabs")]
    // TODO: Write func to write file these automatically in Resource folder.
    public List<GameObject> m_floorTiles;
    public List<GameObject> m_barrierTiles;
    public List<GameObject> m_wallTiles;
    public List<GameObject> m_outerWallTiles;
    public List<GameObject> m_enemyTiles;
    public GameObject m_emptyTile;
    public GameObject m_playerTile;


    public TileType[][] m_tiles;
    public GameObject m_board;



    // TODO: Handle incorrect sizes.
    [Header("Board/ Grid Info")]
    [SerializeField]
    [Range(5, 500)]
    private int m_columns = 100;
    [Range(5, 500)]
    [SerializeField]
    private int m_rows = 100;

    [Header("Room Info")]
    [SerializeField]
    [Range(1, 1000)]
    private int m_minRooms = 5;
    [SerializeField]
    [Range(1, 1000)]
    private int m_maxRooms = 15;

    [Header("")]
    [SerializeField]
    [Range(1, 1000)]
    private int m_minRoomWidth = 5;
    [SerializeField]
    [Range(1, 1000)]
    private int m_maxRoomWidth = 15;

    [Header("")]
    [SerializeField]
    [Range(1, 1000)]
    private int m_minRoomHeight = 5;
    [SerializeField]
    [Range(1, 1000)]
    private int m_maxRoomHeight = 15;

    [Header("Items")]
    [SerializeField]
    private uint m_maxEnemiesPerRoom = 3;

    [Header("Corridor Info")]
    [SerializeField]
    [Range(1, 1000)]
    private int m_minCorridorLength = 3;
    [SerializeField]
    [Range(1, 1000)]
    private int m_maxCorridorLength = 15;
    

    [Header("Generated Info")]
    [SerializeField]
    private List<Room> m_rooms;
    [SerializeField]
    private List<Corridor> m_corridors;
   

    

    [Header("Game Info")]
    [SerializeField]
    private List<GameObject> m_gridObjects = new List<GameObject>();
    [SerializeField]
    private int playerRoomSpawnID = 0;


    private IntRange m_numRooms;
    private IntRange m_roomWidth;
    private IntRange m_roomHeight;
    private IntRange m_corridorLength;


    private void Awake()
    {
        m_board = new GameObject("TileBoard");
    }


    /// <summary>
    /// Generate a completely new level.
    /// </summary>
    public void GenerateRandomLevel()
    {
        ClearGridObjects();

        m_numRooms       = new IntRange(m_minRooms, m_maxRooms);
        m_roomWidth      = new IntRange(m_minRoomWidth, m_minRoomHeight);
        m_roomHeight     = new IntRange(m_minRoomHeight, m_maxRoomHeight);
        m_corridorLength = new IntRange(m_minCorridorLength, m_maxCorridorLength);

        InitTileList();
        GenerateDungeon();
        InstantiateTiles();
    }


    public void LoadLevelFromData(LoadData levelData)
    {
        ClearGridObjects();

        m_tiles   = levelData.TileData;
        m_rows    = levelData.m_rows;
        m_columns = levelData.m_columns;

        InstantiateTiles();
    }



    /// <summary>
    /// Set the tile to the correct width.
    /// </summary>
    private void InitTileList()
    {
        m_tiles = new TileType[m_columns][];
        for(int i = 0; i < m_tiles.Length; i++)
        {
            m_tiles[i] = new TileType[m_rows];
            for(int j = 0; j < m_tiles[i].Length; j++)
            {
                m_tiles[i][j] = TileType.Wall;
            }
        }
    }


    /// <summary>
    /// This is where the generation happens.
    /// </summary>
    private void GenerateDungeon()
    {
        m_rooms = new List<Room>(new Room[m_numRooms.Random]);
        m_corridors = new List<Corridor>(new Corridor[m_rooms.Count - 1]);

        m_rooms[0] = new Room();
        m_corridors[0] = new Corridor();

        // Setup the first room without a corridor.
        m_rooms[0].InitRoom(m_roomWidth, m_roomHeight, m_columns, m_rows);

        // Setup the first corridor using the first room.
        m_corridors[0].InitCorridor(m_rooms[0], m_corridorLength, m_roomWidth, m_roomHeight, m_columns, m_rows, true);

        for(int i = 1; i < m_rooms.Count; i++)
        {
            m_rooms[i] = new Room();
            m_rooms[i].InitRoom(m_roomWidth, m_roomHeight, m_columns, m_rows, m_corridors[i - 1]);

            if(i < m_corridors.Count)
            {
                m_corridors[i] = new Corridor();
                m_corridors[i].InitCorridor(m_rooms[i], m_corridorLength, m_roomWidth, m_roomHeight, m_columns, m_rows, false);
            }
        }

        SetRoomTiles();
        SetCorridorTiles();
        SetPlayerTile();
        SetOutterWallTiles();
        SetEnemies(); // Set last.
    }

    /// <summary>
    /// TODO: Start player in bottom left of dungeon
    /// </summary>
    private void SetPlayerTile()
    {
        var room = m_rooms[playerRoomSpawnID];
        m_tiles[room.m_xPos][room.m_yPos] = TileType.Floor | TileType.Player;
    }


    private void SetOutterWallTiles()
    {
        for(int i = 0; i < m_columns; i++)
        {
            for(int j = 0; j < m_rows; j++)
            {
                var idx = i * m_columns + j;
                if(i == 0)
                {
                    m_tiles[i][j] = TileType.OutterWall;
                }
                else if(i == m_columns - 1)
                {
                    m_tiles[i][j] = TileType.OutterWall;
                }
                else if(j == 0)
                {
                    m_tiles[i][j] = TileType.OutterWall;
                }
                else if(j == m_rows - 1)
                {
                    m_tiles[i][j] = TileType.OutterWall;
                }
                else
                {
                    // Do nothing.
                }
            }
        }
    }



    /// <summary>
    /// Set the tile type for the Rooms.
    /// </summary>
    private void SetRoomTiles()
    {
        foreach(var room in m_rooms)
            foreach(var cell in room.m_cells)
                m_tiles[(int)cell.x][(int)cell.y] = TileType.Floor;
    }


    private void SetCorridorTiles()
    {
        for(int i = 0; i < m_corridors.Count; i++)
        {
            var currentCorridor = m_corridors[i];
            for(int j = 0; j < currentCorridor.m_corridorLength; j++)
            {
                int x = currentCorridor.m_startXPosition;
                int y = currentCorridor.m_startYPosition;

                switch(currentCorridor.m_direction)
                {
                    case Direction.North:
                        y += j;
                        break;
                    case Direction.East:
                        x += j;
                        break;
                    case Direction.South:
                        y -= j;
                        break;
                    case Direction.West:
                        x -= j;
                        break;
                }
                m_tiles[x][y] = TileType.Floor;
            }
        }
    }

    /// <summary>
    /// // TODO: Calculate this using the text file instead.
    /// Set the enemies in avaiable rooms. 
    /// 
    /// </summary>
    private void SetEnemies()
    {
        bool result = TextDungeon.BuildTextDungeonFile(m_tiles);
        if(!result) return;

        var levelStr = TextDungeon.OutputData;

        // TODO: Optimise algorithm by cross referencing each room cell ID with the 
        //          levelStr


        var rows = 0;
        var columns = 0;
        var flag = false;
        for(int i = 0; i < levelStr.Length; i++)
        {
            if(levelStr[i] == StringLiterals.NewLine)
            {
                flag = true;
                columns++;
            }

            if(!flag)
                rows++;
        }


        for(int i = 0; i < columns; i++)
        {
            for(int j = 0; j < rows; j++)
            {
                if(i < 0 || i >= columns)
                    continue;

                if(j < 0 || j >= rows)
                    continue;

                var idx = i * rows + j;

                if(levelStr[idx] != StringLiterals.Floor)
                    continue;
                


                int kk = 0;
                for(int _y = i - 1; _y <= i + 1; _y++)
                {
                    for(int _x = j - 1; _x <= j + 1; _x++)
                    {
                        if(_y < 0 || _y >= columns)
                            continue;

                        if(_x < 0 || _x >= rows)
                            continue;

                        var _idx = _y * rows + _x;

                        if(levelStr[_idx] != StringLiterals.Floor)
                            continue;

                        Debug.Log("ID - " + _x + ", " + _y + " is a floor");

                        kk++;
                    }
                }


                if(kk >= 8)
                {
                    m_tiles[i][j] |= TileType.Enemy;

                    var strBuilder = new System.Text.StringBuilder(levelStr);
                    strBuilder[idx] = StringLiterals.Enemy;
                    levelStr = strBuilder.ToString();
                }
            }
        }


      /// for(int i = 0; i < columns; i++)
      /// {
      ///     for(int j = 0; j < rows; j++)
      ///     {
      ///         var idx = i * rows + j;
      ///         if(levelStr[idx] == StringLiterals.Enemy)
      ///         {
      ///             
      ///         }
      ///     }
      /// }


                // foreach(var room in m_rooms)
                // {
                //     var enemyTileIDs = room.PossibleEnemyPositions(m_tiles);
                //     if(enemyTileIDs.Length < m_maxEnemiesPerRoom)
                //         m_maxEnemiesPerRoom = (uint)enemyTileIDs.Length;    
                //
                //
                //     int fails = 0;
                //     for(int i = 0; i < m_maxEnemiesPerRoom; i++)
                //     {
                //         if(fails >= 100) break;
                //
                //         var rand = Random.Range(0, enemyTileIDs.Length);
                //         var id = enemyTileIDs[rand];
                //         var x = id % room.m_roomWidth;
                //         var y = id / room.m_roomHeight;
                //
                //         // if it already equals enemy
                //         if((m_tiles[x][y] & TileType.Enemy) == TileType.Enemy)
                //         {
                //             i--;
                //             fails++;
                //         }
                //         else
                //         {
                //             m_tiles[x][y] |= TileType.Enemy;
                //         }
                //     }
                //
                //
                //     // TODO: Add evil enemies to corridors.
                //     //          if a corridor is > 5 add to firth tile.
                //     
                // }
            }


            /// <summary>
            /// Remove any object which are about to be written over by BitmaskFloorEdges();
            /// </summary>
            public void RemoveEdgeAndFloorTiles()
    {
        for(int i = 0; i < m_tiles.Length; i++)
        {
            for(int j = 0; j < m_tiles[i].Length; j++)
            {
                if((m_tiles[i][j] & TileType.Floor) == TileType.Floor &&
                        ((m_tiles[i][j] & TileType.Enemy) != TileType.Enemy) &&
                        ((m_tiles[i][j] & TileType.Player) != TileType.Player))
                {
                    var obj = m_gridObjects.Find(o => 
                    {
                        if(o == null) return false;
                        return o.transform.position.x == i && o.transform.position.y == j;
                    });
                    m_gridObjects.Remove(obj);
                    Destroy(obj);
                }
                else
                {
                    // do nothing
                }
            }
        }
    }



    /// <summary>
    /// init and update edges of sprites.
    /// </summary>
    /// <param name="initFloor"></param>
    public void BitmaskFloorEdges()
    {
        for(int i = 0; i < m_tiles.Length; i++)
        {
            for(int j = 0; j < m_tiles[i].Length; j++)
            {
                if((m_tiles[i][j] & TileType.Floor) == TileType.Floor)
                {
                    var tileID = EdgeCalculations.TileID(m_tiles, i, j);
                    if(tileID == m_barrierTiles.Count)
                    {
                        InstantiateFromArray(m_floorTiles, i, j);
                    }
                    else
                    {
                        Instantiate(m_barrierTiles[tileID], m_board, new Vector3(i, j, 0));
                    }
                }
                else
                {
                    // do nothing
                }
            }
        }
    }


     
    private void ClearGridObjects()
    {
        if(m_gridObjects == null || m_gridObjects.Count <= 0)
            return;

        foreach(var obj in m_gridObjects)
        {
            Destroy(obj);
        }
        m_gridObjects.Clear();
    }



    /// <summary>
    /// Build sprites to scene.
    /// </summary>
    private void InstantiateTiles()
    {
        for(int i = 0; i < m_tiles.Length; i++)
        {
            for(int j = 0; j < m_tiles[i].Length; j++)
            {
                if((m_tiles[i][j] & TileType.Wall) == TileType.Wall)
                {
                    InstantiateFromArray(m_wallTiles, i, j);
                }
                else if((m_tiles[i][j] & TileType.OutterWall) == TileType.OutterWall)
                {
                    InstantiateFromArray(m_outerWallTiles, i, j);
                }
                else if((m_tiles[i][j] & TileType.Enemy) == TileType.Enemy)
                {
                    InstantiateFromArray(m_enemyTiles, i, j);
                }
                else if((m_tiles[i][j] & TileType.Player) == TileType.Player)
                {
                    Instantiate(m_playerTile, m_board, new Vector2(i, j));

                }
                else if((m_tiles[i][j] & TileType.Empty) == TileType.Empty)
                {
                    // TODO: Handle Empty space.
                    Instantiate(m_emptyTile, m_board, i, j);
                }
                else
                {
                    // DO nothing 
                }
            }
        }
        BitmaskFloorEdges();
    }


   

    /// <summary> 
    /// Generate a prefab at random.
    /// </summary>
    /// <param name="prefabs"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    private void InstantiateFromArray(List<GameObject> prefabs, float x, float y) { InstantiateFromArray(prefabs, m_board, x, y); }
    public void InstantiateFromArray(List<GameObject> prefabs, GameObject board, float x, float y)
    {
        var randID = Random.Range(0, prefabs.Count);
        var position = new Vector3(x, y, 0f);

        Instantiate(prefabs[randID], board, position);
    }

    /// <summary>
    /// A instantiation function which will add the object to the gridobjects function.
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="board"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void Instantiate(GameObject prefab, GameObject board, float x, float y) { Instantiate(prefab, board, new Vector3(x,y,0f)); }
    public void Instantiate(GameObject prefab, GameObject board, Vector3 position)
    {
        var tile = Instantiate(prefab, position, Quaternion.identity);
        tile.name = prefab.name;
        tile.transform.parent = board.transform;

        m_gridObjects.Add(tile);
    }
    

}
