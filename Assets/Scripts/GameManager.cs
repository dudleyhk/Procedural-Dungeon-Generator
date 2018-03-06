using UnityEngine;



public class GameManager : MonoBehaviour 
{
    [SerializeField]
    public BoardManager m_boardManager;
    



    private void Awake()
    {
        m_boardManager = FindObjectOfType<BoardManager>();
        // TODO: error check
    }



    private void OnEnable()
    {
        // TODO: Hook up to the menu load functions.
        UIManager.saveLevel += SaveFile;
        UIManager.loadLevel += GenerateFromFile;
        UIManager.generate += GenerateRandomLevel;
    }

    private void OnDisable()
    {
        // TODO: Hook up to the menu load functions.
        UIManager.saveLevel -= SaveFile;
        UIManager.loadLevel -= GenerateFromFile;
        UIManager.generate -= GenerateRandomLevel;
    }
    

    public void GenerateRandomLevel()
    {
        m_boardManager.GenerateRandomLevel();
       
    }


    public void GenerateFromFile(string file)
    {
        if(FileManager.ReadDungeonFile(file))
        {
            m_boardManager.LoadLevelFromData(FileManager.CurrentLoadData);
        }
    }


    private void SaveFile(string fileName)
    {
        FileManager.WriteDungeonFile(m_boardManager.m_tiles, fileName);
    }
}
