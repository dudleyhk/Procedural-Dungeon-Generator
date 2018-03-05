using System.Collections.Generic;
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

    /// <summary>
    /// Return the names of all files in the Resource folder.
    /// </summary>
    /// <returns></returns>
    public static string[] SaveFiles()
    {
        List<string> names = new List<string>();
      
        System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(Application.dataPath + "/Resources/Saves/");
        foreach(System.IO.FileInfo file in dir.GetFiles("*.txt"))
        {
            // TODO: Expand this to be more versile. 
            //var name = file.Name.TrimEnd('.', 't', 'x', 't');
            names.Add(file.Name);
        }
       
        return names.ToArray();
    }
}
