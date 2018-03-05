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
        // PlayGame += GenerateFromFile(file)
    }

    private void OnDisable()
    {
        // TODO: Hook up to the menu load functions.
        // PlayGame -= GenerateFromFile(file)
    }


    private void Start()
    {

    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
            GenerateRandomLevel();

        if(Input.GetKeyDown(KeyCode.F))
            GenerateFromFile("tempDungeon.txt");
    }

    private void GenerateRandomLevel()
    {
        m_boardManager.GenerateRandomLevel();
        FileManager.WriteDungeonFile(m_boardManager.m_tiles, "tempDungeon.txt");
    }


    private void GenerateFromFile(string file)
    {
        if(FileManager.ReadDungeonFile(file))
        {
            m_boardManager.LoadLevelFromData(FileManager.CurrentLoadData);
        }
    }
}
