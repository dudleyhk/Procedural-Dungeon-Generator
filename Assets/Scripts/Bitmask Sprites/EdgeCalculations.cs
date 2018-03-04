using UnityEngine;



public class EdgeCalculations : ScriptableObject
{
    [System.Flags]
    private enum MaskDirections { North = 1, West = 2, East = 4, South = 8 }


    public static int TileID(TileType[][] tiles, int x, int y)
    {
        var maskDir = new MaskDirections();
        BitmaskDirection(tiles, ref maskDir, MaskDirections.West , x - 1, y); // West
        BitmaskDirection(tiles, ref maskDir, MaskDirections.East , x + 1, y); // East
        BitmaskDirection(tiles, ref maskDir, MaskDirections.North, x, y + 1); // North
        BitmaskDirection(tiles, ref maskDir, MaskDirections.South, x, y - 1); // South

        var tileID = CalculateSpriteID(maskDir);
        return tileID;
    }


    /// <summary>
    /// If the direction is also a floor, add the dir to the maskDir bits.
    /// </summary>
    /// <param name="tiles"></param>
    /// <param name="maskDir"></param>
    /// <param name="dir"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    private static void BitmaskDirection(TileType[][] tiles, ref MaskDirections maskDir, MaskDirections dir, int x, int y)
    {
        if(x >= tiles.Length || x < 0)
            return;
        if(y >= tiles[0].Length || y < 0)
            return;

        if((tiles[x][y] & TileType.Floor) == TileType.Floor)
            maskDir |= dir;
    }


    private static int CalculateSpriteID(MaskDirections maskDir)
    {
        int total = 0;

        if((maskDir & MaskDirections.North) == MaskDirections.North)
        {
            total += (int)MaskDirections.North;
        }

        if((maskDir & MaskDirections.West) == MaskDirections.West)
        {
            total += (int)MaskDirections.West;
        }

        if((maskDir & MaskDirections.East) == MaskDirections.East)
        {
            total += (int)MaskDirections.East;
        }

        if((maskDir & MaskDirections.South) == MaskDirections.South)
        {
            total += (int)MaskDirections.South;
        }

        return total;
    }


}
