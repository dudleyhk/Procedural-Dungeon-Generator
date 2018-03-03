using System.Collections.Generic;
using UnityEngine;

// Keep in this order.
public enum TileType { Wall, Floor, Player, OutterWall, Item, Exit, }

public class BoardManager : MonoBehaviour
{
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

    [Header("Corridor Info")]
    [SerializeField]
    [Range(1, 1000)]
    private int m_minCorridorLength = 3;
    [SerializeField]
    [Range(1, 1000)]
    private int m_maxCorridorLength = 15;

    [Header("Sprite Prefabs")]
    // TODO: Write func to write file these automatically in Resource folder.
    [SerializeField]
    private List<GameObject> m_floorTiles;
    [SerializeField]
    private List<GameObject> m_wallTiles;
    [SerializeField]
    private List<GameObject> m_outerWallTiles;
    [SerializeField]
    private GameObject m_player;


    [Header("Generated Info")]
    [SerializeField]
    private List<Room> m_rooms;
    [SerializeField]
    private List<Corridor> m_corridors;
    [SerializeField]
    private GameObject m_board;

    [Header("Game Info")]
    [SerializeField]
    private int playerRoomSpawnID = 0;
    [SerializeField]
    private bool m_loadFromFile = false;



    private TileType[][] m_tiles;
    private IntRange m_numRooms;
    private IntRange m_roomWidth;
    private IntRange m_roomHeight;
    private IntRange m_corridorLength;


    private void Awake()
    {
        m_numRooms = new IntRange(m_minRooms, m_maxRooms);
        m_roomWidth = new IntRange(m_minRoomWidth, m_minRoomHeight);
        m_roomHeight = new IntRange(m_minRoomHeight, m_maxRoomHeight);
        m_corridorLength = new IntRange(m_minCorridorLength, m_maxCorridorLength);

        m_board = new GameObject("TileBoard");
    }

    private void Start()
    {
        Run();
    }


    private void Update()
    {
        // TODO: Send to UIManager
        if(Input.GetKeyDown(KeyCode.G))
        {
            Run();
        }
    }



    private void Run()
    {
        Debug.Log("Log: Generate.");
        if(m_loadFromFile && ReadDungeonFile("tempDungeon.txt"))
        {
            m_tiles = TextDungeon.FileData.TileData;
            m_rows = TextDungeon.FileData.m_rows;
            m_columns = TextDungeon.FileData.m_columns;
        }
        else
        {
            InitTileList();
            GenerateDungeon();



            
            

            WriteDungeonFile("tempDungeon.txt");
        }
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
        AddOutterWall();
    }


    private void SetPlayerTile()
    {
        // foreach room in roomList
            // save lowest x
            // save lowest y

        AddPlayer(m_rooms[playerRoomSpawnID]);
    }


    private void AddOutterWall()
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


    private void AddPlayer(Room room)
    {
        // TODO: Player to start in bottom left and end top right.
        Vector3 playerPos = new Vector3(room.m_xPos, room.m_yPos);
        Instantiate(m_player, playerPos, Quaternion.identity);
        m_tiles[(int)playerPos.x][(int)playerPos.y] = TileType.Player;
    }



    /// <summary>
    /// Set the tile type for the Rooms.
    /// </summary>
    private void SetRoomTiles()
    {
        for(int i = 0; i < m_rooms.Count; i++)
        {
            var room = m_rooms[i];
            for(int j = 0; j < room.m_roomWidth; j++)
            {
                int x = room.m_xPos + j;
                for(int k = 0; k < room.m_roomHeight; k++)
                {
                    int y = room.m_yPos + k;
                    m_tiles[x][y] = TileType.Floor;
                }
            }
        }
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


                //TODO: Different size corridors by changing the Xs and Ys
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

                // Set the tile at these coordinates to Floor.
                m_tiles[x][y] = TileType.Floor;
            }
        }
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
                InstantiateFromArray(m_floorTiles, i, j);

                if(m_tiles[i][j] == TileType.Wall)
                {
                    InstantiateFromArray(m_wallTiles, i, j);
                }
                if(m_tiles[i][j] == TileType.OutterWall)
                {
                    InstantiateFromArray(m_outerWallTiles, i, j);
                }
            }
        }
    }

    /// <summary>
    /// TODO: Saving in Unity via UI
    /// </summary>
    /// <param name="fileName"></param>
    public void WriteDungeonFile(string fileName)
    {
        if(!TextDungeon.BuildTextDungeonFile(m_tiles))
        {
            Debug.LogError("Error: Building the dungeon file");
        }

        if(!TextDungeon.OutputToFile(Application.dataPath + "/Resources/Saves/", fileName, TextDungeon.OutputData))
        {
            Debug.LogError("Error: Outputting to file.");
        }
    }


    public bool ReadDungeonFile(string fileName)
    {
        if(!TextDungeon.ReadFile(Application.dataPath + "/Resources/Saves/", fileName))
        {
            Debug.LogError("Error: Function returned false");
            return false;
        }
        return true;
    }

    /// <summary>
    /// Generate a prefab at random.
    /// </summary>
    /// <param name="prefabs"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    private void InstantiateFromArray(List<GameObject> prefabs, float x, float y)
    {
        var randID = Random.Range(0, prefabs.Count);
        var position = new Vector3(x, y, 0f);

        var tile = Instantiate(prefabs[randID], position, Quaternion.identity) as GameObject;
        tile.name = prefabs[randID].name;

        tile.transform.parent = m_board.transform;
    }
}
