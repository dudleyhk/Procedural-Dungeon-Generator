using System.Collections.Generic;
using UnityEngine;

public class Room
{
    public int m_xPos; // Lower left corner
    public int m_yPos; // Lower left corner
    public int m_roomWidth;
    public int m_roomHeight;
    public Direction m_enteringCorridor;

    /// <summary>
    /// The cell positions which make up the room. 
    /// </summary>
    public List<Vector2> m_cells = new List<Vector2>();




    /// <summary>
    /// Setup room with no corridor. This is only used for the first room.
    /// </summary>
    /// <param name="widthRange"></param>
    /// <param name="heightRange"></param>
    /// <param name="columns"></param>
    /// <param name="rows"></param>
    public void InitRoom(IntRange widthRange, IntRange heightRange, int columns, int rows)
    {
        m_roomWidth = widthRange.Random;
        m_roomHeight = heightRange.Random;

        // Init room in bottom Left.
        m_xPos = 1;
        m_yPos = 1;

        AddCellPosition(m_xPos, m_yPos);
    }


    /// <summary>
    /// TODO: Amalgamate the two InitRoom functions. 
    /// Setup a room with a corridor entering it. 
    /// </summary>
    /// <param name="widthRange"></param>
    /// <param name="heightRange"></param>
    /// <param name="columns"></param>
    /// <param name="rows"></param>
    /// <param name="corridor"></param>
    public void InitRoom(IntRange widthRange, IntRange heightRange, int columns, int rows, Corridor corridor)
    {
        m_enteringCorridor = corridor.m_direction;

        m_roomWidth = widthRange.Random;
        m_roomHeight = heightRange.Random;

        switch(m_enteringCorridor)
        {
            case Direction.North:
                m_roomHeight = Mathf.Clamp(m_roomHeight, 1, rows - corridor.EndPositionY);

                m_yPos = corridor.EndPositionY;
                m_xPos = Random.Range(corridor.EndPositionX - m_roomWidth + 1, corridor.EndPositionX);
                m_xPos = Mathf.Clamp(m_xPos, 0, columns - m_roomWidth);
                break;

            case Direction.East:
                m_roomWidth = Mathf.Clamp(m_roomWidth, 1, columns - corridor.EndPositionX);

                m_xPos = corridor.EndPositionX;
                m_yPos = Random.Range(corridor.EndPositionY - m_roomHeight + 1, corridor.EndPositionY);
                m_yPos = Mathf.Clamp(m_yPos, 0, rows - m_roomHeight);
                break;

            case Direction.South:
                m_roomHeight = Mathf.Clamp(m_roomHeight, 1, corridor.EndPositionY);

                m_yPos = corridor.EndPositionY - m_roomHeight + 1;
                m_xPos = Random.Range(corridor.EndPositionX - m_roomWidth + 1, corridor.EndPositionX);
                m_xPos = Mathf.Clamp(m_xPos, 0, columns - m_roomWidth);
                break;

            case Direction.West:
                m_roomWidth = Mathf.Clamp(m_roomWidth, 1, corridor.EndPositionX);

                m_xPos = corridor.EndPositionX - m_roomWidth + 1;
                m_yPos = Random.Range(corridor.EndPositionY - m_roomHeight + 1, corridor.EndPositionY);
                m_yPos = Mathf.Clamp(m_yPos, 0, rows - m_roomHeight);
                break;
        }

        AddCellPosition(m_xPos, m_yPos);
    }

    /// <summary>
    /// Calculate which tileIDs in the room have an ememy.
    /// </summary>
    /// <returns></returns>
    public int[] PossibleEnemyPositions(TileType[][] types)
    {
        List<int> validTileIDs = new List<int>();
        for(int y = m_yPos; y < m_yPos + m_roomHeight; y++)
        {
            for(int x = m_xPos; x < m_xPos + m_roomWidth; x++)
            {
                var idx = y * m_roomWidth + x;

                int startX = x;
                int startY = y;
                int endX = x;
                int endY = y;

                if(x > m_xPos)
                    startX--;
                if(x < m_xPos + m_roomWidth)
                    endX++;
                if(y > m_yPos)
                    startY--;
                if(y < m_yPos + m_roomHeight)
                    endY++;

                int kk = 0;
                //TODO: Encapsule in another function so A* can use it.
                for(int _y = startY; _y <= endY; _y++)
                {
                    for(int _x = startX; _x <= endX; _x++)
                    {
                        if((types[_x][_y] & TileType.Floor) == TileType.Floor)
                        {
                            if(!((types[_x][_y] & TileType.Player) == TileType.Player))
                            {
                                if(!((types[_x][_y] & TileType.OutterWall) == TileType.OutterWall))
                                {
                                    kk++;
                                }
                            }
                        }
                    }
                }

                if(kk >= 9)
                    if(!validTileIDs.Contains(idx))
                        validTileIDs.Add(idx);
            }
        }
        return validTileIDs.ToArray();
    }
    


    private void AddCellPosition(int _x, int _y)
    {
        for(int y = _y; y < m_yPos + m_roomHeight; y++)
            for(int x = _x; x < m_xPos + m_roomWidth; x++)
                m_cells.Add(new Vector2(x, y));
    }
}