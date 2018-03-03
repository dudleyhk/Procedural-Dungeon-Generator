using UnityEngine;

// Clockwise directions.
public enum Direction { North, East, South, West }


public class Corridor 
{
    public int m_startXPosition;
    public int m_startYPosition;
    public int m_corridorLength;
    public Direction m_direction;

    /// <summary>
    /// Calculate the end position based on the start and end position. 
    /// </summary>
    public int EndPositionX
    {
        get
        {
            if(m_direction == Direction.North || m_direction == Direction.South)
                return m_startXPosition;
            if(m_direction == Direction.East)
                return m_startXPosition + m_corridorLength - 1;
            return m_startXPosition - m_corridorLength + 1;
        }
    }


    public int EndPositionY
    {
        get
        {
            if(m_direction == Direction.East || m_direction == Direction.West)
                return m_startYPosition;
            if(m_direction == Direction.North)
                return m_startYPosition + m_corridorLength - 1;
            return m_startYPosition - m_corridorLength + 1;
        }
    }


    /// <summary>
    /// Setup a corridor and set length so it doesn't exceed the board size.
    /// </summary>
    /// <param name="room"></param>
    /// <param name="length"></param>
    /// <param name="roomWidth"></param>
    /// <param name="roomHeight"></param>
    /// <param name="columns"></param>
    /// <param name="rows"></param>
    /// <param name="firstCorridor"></param>
    public void InitCorridor(Room room, IntRange length, IntRange roomWidth, IntRange roomHeight, int columns, int rows, bool firstCorridor)
    {
        // Select a random direction. 
        m_direction = (Direction)Random.Range(0, 4);

        // Calculate the oppose direction from the direction we came from.
        Direction oppositeDirection = (Direction)(((int)room.m_enteringCorridor + 2) % 4);

        // If this is noth the first corridor and the randomly selected direction is opposite to the previous corridor's direction...
        if(!firstCorridor && m_direction == oppositeDirection)
        {
            // Get the direction add 1 and divide by 4.
            int directionInt = (int)m_direction;
            directionInt++;
            directionInt = directionInt % 4;
            m_direction = (Direction)directionInt;
        }

        m_corridorLength = length.Random;
        int maxLength = length.MaxRange;


        // Make sure the corridor doesn't go off the board.
        switch(m_direction)
        {
            case Direction.North:
                m_startXPosition = Random.Range(room.m_xPos, room.m_xPos + room.m_roomWidth - 1);
                m_startYPosition = room.m_yPos + room.m_roomHeight;
                maxLength = rows - m_startYPosition - roomHeight.MinRange;
                break;

            case Direction.East:
                m_startXPosition = room.m_xPos + room.m_roomWidth;
                m_startYPosition = Random.Range(room.m_yPos, room.m_yPos + room.m_roomHeight - 1);
                maxLength = columns - m_startXPosition - roomWidth.MinRange;
                break;

            case Direction.South:
                m_startXPosition = Random.Range(room.m_xPos, room.m_xPos + room.m_roomWidth);
                m_startYPosition = room.m_yPos;
                maxLength = m_startYPosition - roomHeight.MinRange;
                break;

            case Direction.West:
                m_startXPosition = room.m_xPos;
                m_startYPosition = Random.Range(room.m_yPos, room.m_yPos + room.m_roomHeight);
                maxLength = m_startXPosition - roomWidth.MinRange;
                break;
        }

        // We clamp the length of the corridor to make sure it doesn't go off the board.
        m_corridorLength = Mathf.Clamp(m_corridorLength, 1, maxLength);
    }
}
