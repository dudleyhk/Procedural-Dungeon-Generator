using UnityEngine;



public class GameManager : MonoBehaviour 
{
    [SerializeField]
    public BoardManager m_boardManager;
    [SerializeField]
    public FileManager m_fileManager;
    



    private void Awake()
    {
        m_boardManager = FindObjectOfType<BoardManager>();
        // TODO: error check
    }



    private void OnEnable()
    {
        // TODO: Hook up to the menu load functions.
        // PlayGame += GenerateFromFile(file)
    }

    private void OnDisable()
    {
        // TODO: Hook up to the menu load functions.
        // PlayGame -= GenerateFromFile(file)
    }


    private void Start()
    {
       // GenerateRandomLevel();
       GenerateFromFile("tempDungeon.txt");
    }


    private void GenerateRandomLevel()
    {
        m_boardManager.GenerateRandomLevel();
    }


    private void GenerateFromFile(string file)
    {
        if(FileManager.ReadDungeonFile(file))
        {
            m_boardManager.LoadLevelFromData(FileManager.CurrentLoadData);
        }
    }
}
