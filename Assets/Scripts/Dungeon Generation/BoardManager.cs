using System.Collections.Generic;
using UnityEngine;


public enum TileType { Wall, Floor }

public class BoardManager : MonoBehaviour
{
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
    private GameObject m_boardHolder;



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
    }

    private void Start()
    {
        m_boardHolder = new GameObject("BoardHolder");

        SetupTileList();
        CreateRoomsAndCorridors();

        // WriteToFile();

        // TODO: Read from text file. 
        SetTilesValuesForRooms();
        SetTilesValuesForCorridors();
        InstantiateTiles();

        //InstantiateOuterWalls();
    }

    /// <summary>
    /// Set the tile to the correct width. 
    /// </summary>
    private void SetupTileList()
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
    private void CreateRoomsAndCorridors()
    {
        m_rooms = new List<Room>(new Room[m_numRooms.Random]);
        m_corridors = new List<Corridor>(new Corridor[m_rooms.Count - 1]);

        m_rooms[0] = new Room();
        m_corridors[0] = new Corridor();

        // Setup the first room without a corridor.
        m_rooms[0].SetupRoom(m_roomWidth, m_roomHeight, m_columns, m_rows);

        // Setup the first corridor using the first room.
        m_corridors[0].SetupCorridor(m_rooms[0], m_corridorLength, m_roomWidth, m_roomHeight, m_columns, m_rows, true);

        for(int i = 1; i < m_rooms.Count; i++)
        {
            m_rooms[i] = new Room();
            m_rooms[i].SetupRoom(m_roomWidth, m_roomHeight, m_columns, m_rows, m_corridors[i - 1]);

            if(i < m_corridors.Count)
            {
                m_corridors[i] = new Corridor();
                m_corridors[i].SetupCorridor(m_rooms[i], m_corridorLength, m_roomWidth, m_roomHeight, m_columns, m_rows, false);
            }

            if(i == m_rooms.Count * 0.5f)
            {
                Vector3 playerPos = new Vector3(m_rooms[i].m_xPos, m_rooms[i].m_yPos, 0);
                Instantiate(m_player, playerPos, Quaternion.identity);
            }
        }
    }

    /// <summary>
    /// Set the tile type for the Rooms.
    /// </summary>
    private void SetTilesValuesForRooms()
    {
        for(int i = 0; i < m_rooms.Count; i++)
        {
            Room currentRoom = m_rooms[i];

            for(int j = 0; j < currentRoom.m_roomWidth; j++)
            {
                int xCoord = currentRoom.m_xPos + j;

                for(int k = 0; k < currentRoom.m_roomHeight; k++)
                {
                    int yCoord = currentRoom.m_yPos + k;
                    m_tiles[xCoord][yCoord] = TileType.Floor;
                }
            }
        }
    }


    private void SetTilesValuesForCorridors()
    {
        for(int i = 0; i < m_corridors.Count; i++)
        {
            Corridor currentCorridor = m_corridors[i];

            for(int j = 0; j < currentCorridor.m_corridorLength; j++)
            {
                int xCoord = currentCorridor.m_startXPosition;
                int yCoord = currentCorridor.m_startYPosition;

                switch(currentCorridor.m_direction)
                {
                    case Direction.North:
                        yCoord += j;
                        break;
                    case Direction.East:
                        xCoord += j;
                        break;
                    case Direction.South:
                        yCoord -= j;
                        break;
                    case Direction.West:
                        xCoord -= j;
                        break;
                }

                // Set the tile at these coordinates to Floor.
                m_tiles[xCoord][yCoord] = TileType.Floor;
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
                InstantiateFromArray(m_floorTiles.ToArray(), i, j);

                if(m_tiles[i][j] == TileType.Wall)
                {
                    InstantiateFromArray(m_wallTiles.ToArray(), i, j);
                }
            }
        }
    }


    private void InstantiateOuterWalls()
    {
        // The outer walls are one unit left, right, up and down from the board.
        float leftEdgeX = -1f;
        float rightEdgeX = m_columns + 0f;
        float bottomEdgeY = -1f;
        float topEdgeY = m_rows + 0f;

        // Instantiate both vertical walls (one on each side).
        InstantiateVerticalOuterWall(leftEdgeX, bottomEdgeY, topEdgeY);
        InstantiateVerticalOuterWall(rightEdgeX, bottomEdgeY, topEdgeY);

        // Instantiate both horizontal walls, these are one in left and right from the outer walls.
        InstantiateHorizontalOuterWall(leftEdgeX + 1f, rightEdgeX - 1f, bottomEdgeY);
        InstantiateHorizontalOuterWall(leftEdgeX + 1f, rightEdgeX - 1f, topEdgeY);
    }


    private void InstantiateVerticalOuterWall(float xCoord, float startingY, float endingY)
    {
        // Start the loop at the starting value for Y.
        float currentY = startingY;

        // While the value for Y is less than the end value...
        while(currentY <= endingY)
        {
            // ... instantiate an outer wall tile at the x coordinate and the current y coordinate.
            InstantiateFromArray(m_outerWallTiles.ToArray(), xCoord, currentY);

            currentY++;
        }
    }


    private void InstantiateHorizontalOuterWall(float startingX, float endingX, float yCoord)
    {
        // Start the loop at the starting value for X.
        float currentX = startingX;

        // While the value for X is less than the end value...
        while(currentX <= endingX)
        {
            // ... instantiate an outer wall tile at the y coordinate and the current x coordinate.
            InstantiateFromArray(m_outerWallTiles.ToArray(), currentX, yCoord);

            currentX++;
        }
    }

    /// <summary>
    /// Generate a prefab at random.
    /// </summary>
    /// <param name="prefabs"></param>
    /// <param name="xCoord"></param>
    /// <param name="yCoord"></param>
    private void InstantiateFromArray(GameObject[] prefabs, float xCoord, float yCoord)
    {
        var randomIndex = Random.Range(0, prefabs.Length);
        var position = new Vector3(xCoord, yCoord, 0f);

        GameObject tileInstance = Instantiate(prefabs[randomIndex], position, Quaternion.identity) as GameObject;

        tileInstance.transform.parent = m_boardHolder.transform;
    }
}
