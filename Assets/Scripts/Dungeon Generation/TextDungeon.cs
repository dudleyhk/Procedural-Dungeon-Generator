using System.Collections.Generic;



public struct LoadData
{
    public TileType[][] TileData { get; set; }
    public int m_columns, m_rows;
}

public struct StringLiterals
{
    public const string Blank = "";

    public const char Wall       = '#';
    public const char Floor      = '.';
    public const char Item       = '?';
    public const char Player     = 'p';
    public const char OutterWall = 'x';
    public const char Enemy      = 'e';
    public const char Exit       = 'E';
    public const char Debug      = '~';
    public const char NewLine    = '\n';
}


public class TextDungeon
{
    public static LoadData FileData;
    public static string   OutputData   { get; private set; }


    public static bool BuildTextDungeonFile(TileType[][] tiles)
    {
        OutputData = StringLiterals.Blank;
        for(int i = 0; i < tiles.Length; i++)
        {
            for(int j = 0; j < tiles[i].Length; j++)
            {
                var tileChar = TileTypeToChar(tiles[i][j]);
                OutputData += tileChar;
            }
            OutputData += StringLiterals.NewLine;
        }
        return true;
    }


    /// <summary>
    /// TODO: error check params.
    /// Write to existing file or create a new file. 
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="fileName"></param>
    /// <param name="outputData"></param>
    /// <returns></returns>
    public static bool OutputToFile(string filePath, string fileName, string outputData)
    {
        if(outputData == StringLiterals.Blank)
            return false;

        var result = System.IO.File.Exists(filePath + fileName);
        System.IO.StreamWriter streamWriter;

        if(!result)
        {
            streamWriter = System.IO.File.CreateText(filePath + fileName);
        }
        else
        {
            streamWriter = new System.IO.StreamWriter(filePath + fileName);
        }
        streamWriter.Write(outputData);
        streamWriter.Close();

        return true;
    }



    /// <summary>
    /// Read from Text file, Extrapolate all data from it and store in LoadData struct.
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static bool ReadFile(string filePath, string fileName)
    {
        bool result = false;
        if(System.IO.File.Exists(filePath + fileName))
        {
            var streamReader = new System.IO.StreamReader(filePath + fileName);
            var fileData = streamReader.ReadToEnd();
            if(fileData == "") return false;

            if(StringToTileTypeArray(fileData))
            {
                result = true;
            }
            else
            {
                UnityEngine.Debug.Log("Log: FileDataOut size - " + FileData.TileData.Length);
            }
            streamReader.Close();
        }
        else
        {
            //TODO: Create a new file and save it to saved maps. 
            UnityEngine.Debug.LogWarning("Warning: File " + fileName + " at path " + filePath + " doesn't exist.");
            return result;
        }
        return result;
    }

    /// <summary>
    /// TODO: Encapsulate.
    /// Covnert the file into a tileType array format.
    /// </summary>
    /// <param name="tileStr"></param>
    /// <returns></returns>
    private static bool StringToTileTypeArray(string tileStr)
    {
        var rows = 0;
        foreach(var tileChar in tileStr)
        {
            if(tileChar == StringLiterals.NewLine)
                break;
            rows++;
        }

        var columns = 0;
        foreach(var tileChar in tileStr)
        {
            if(tileChar == StringLiterals.NewLine)
            {
                columns++;
            }
        }



        FileData = new LoadData();
        FileData.TileData  = new TileType[columns][];
        FileData.m_columns = columns;
        FileData.m_rows = rows;

        for(int i = 0; i < columns; i++)
        {
            FileData.TileData[i] = new TileType[rows];
            for(int j = 0; j < rows; j++)
            {
                var ID = i * rows + j;
                var tileChar = tileStr[ID];

                if(tileChar == StringLiterals.NewLine)
                {
                    tileStr = tileStr.Remove(ID, 1);
                    tileChar = tileStr[ID];
                }

                switch(tileChar)
                {
                    case StringLiterals.Wall:
                        FileData.TileData[i][j] = TileType.Wall;
                        break;
                    case StringLiterals.Player:
                        FileData.TileData[i][j] = TileType.Player | TileType.Floor;
                        break;
                    case StringLiterals.Floor:
                        FileData.TileData[i][j] = TileType.Floor;
                        break;
                    case StringLiterals.OutterWall:
                        FileData.TileData[i][j] = TileType.OutterWall;
                        break;
                    default:
                        UnityEngine.Debug.LogWarning("Warning: Invalid TileType - " + tileChar);
                        break;
                }
            }
        }
        return true;
    }


    private static char TileTypeToChar(TileType type)
    {
        char output = StringLiterals.Debug;

        if((type & TileType.Wall) == TileType.Wall)
        {
            output = StringLiterals.Wall;
        }
        else if((type & TileType.OutterWall) == TileType.OutterWall)
        {
            output = StringLiterals.OutterWall;
        }
        else if((type & TileType.Player) == TileType.Player)
        {
            output = StringLiterals.Player;
        }
        else if((type & TileType.Enemy) == TileType.Enemy)
        {
            output = StringLiterals.Enemy;
        }
        else if((type & TileType.Floor) == TileType.Floor)
        {
            output = StringLiterals.Floor;
        }
        else
        {
            UnityEngine.Debug.LogWarning("Warning: Invalid TileType - " + type.ToString());
        }

        return output;
    }
}
