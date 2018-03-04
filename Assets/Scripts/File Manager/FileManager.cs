using UnityEngine;



public class FileManager : MonoBehaviour 
{
    public static LoadData CurrentLoadData
    {
        get
        {
            return TextDungeon.FileData;
        }
    }

    /// <summary>
    /// TODO: Saving in Unity via UI
    /// </summary>  
    /// <param name="fileName"></param>
    public static void WriteDungeonFile(TileType[][] tiles, string fileName)
    {
        if(!TextDungeon.BuildTextDungeonFile(tiles))
        {
            Debug.LogError("Error: Building the dungeon file");
        }

        if(!TextDungeon.OutputToFile(Application.dataPath + "/Resources/Saves/", fileName, TextDungeon.OutputData))
        {
            Debug.LogError("Error: Outputting to file.");
        }
    }


    public static bool ReadDungeonFile(string fileName)
    {
        if(!TextDungeon.ReadFile(Application.dataPath + "/Resources/Saves/", fileName))
        {
            Debug.LogError("Error: Function returned false");
            return false;
        }
        return true;
    }
}
