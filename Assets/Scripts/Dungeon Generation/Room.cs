using UnityEngine;

public class Room
{
    public int m_xPos; // Lower left corner
    public int m_yPos; // Lower left corner
    public int m_roomWidth;
    public int m_roomHeight;
    public Direction m_enteringCorridor;




    /// <summary>
    /// Setup room with no corridor. This is only used for the first room.
    /// </summary>
    /// <param name="widthRange"></param>
    /// <param name="heightRange"></param>
    /// <param name="columns"></param>
    /// <param name="rows"></param>
    public void SetupRoom(IntRange widthRange, IntRange heightRange, int columns, int rows)
    {
        m_roomWidth  = widthRange.Random;
        m_roomHeight = heightRange.Random;

        // Set the initial room roughly in the middle of the board.
        m_xPos = Mathf.RoundToInt(columns / 2f - m_roomWidth / 2f);
        m_yPos = Mathf.RoundToInt(rows / 2f - m_roomHeight / 2f);
    }


    /// <summary>
    /// Setup a room with a corridor entering it. 
    /// </summary>
    /// <param name="widthRange"></param>
    /// <param name="heightRange"></param>
    /// <param name="columns"></param>
    /// <param name="rows"></param>
    /// <param name="corridor"></param>
    public void SetupRoom(IntRange widthRange, IntRange heightRange, int columns, int rows, Corridor corridor)
    {
        m_enteringCorridor = corridor.m_direction;

        m_roomWidth  = widthRange.Random;
        m_roomHeight = heightRange.Random;

        switch(corridor.m_direction)
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
    }
}