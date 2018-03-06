using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum BrushType
{
    Floor,
    Wall,
    Player,
    Enemy,
    Erase,
    Unassigned
}


public class TileEditor : MonoBehaviour
{
    public static bool Pause { get; set; }

    [SerializeField]
    private GameManager m_gameManager;
    [SerializeField]
    private PlayerController m_playerController;
    [SerializeField]
    private CameraController m_cameraController;
    [SerializeField]
    private bool m_playMode = false;


    private BrushType m_currBrushType = BrushType.Unassigned;


    private void Awake()
    {
        m_gameManager = FindObjectOfType<GameManager>();
        m_cameraController = FindObjectOfType<CameraController>();
        // TODO: Error check
    }


    private void Start()
    {
        Pause = false;
        
        EditMode();

        // Force m_gameManager to generate a random map.
        m_gameManager.GenerateRandomLevel();

        // Start Coroutine which returns true when a player is found.
    }


    private void OnEnable()
    {
        UIManager.changeBrushType += ChangeBrushType;
    }

    private void OnDisable()
    {
        UIManager.changeBrushType -= ChangeBrushType;
    }


    private void Update()
    {
        if(Pause) return;

        SwitchCameras();
        Paint();

        if(Input.GetMouseButtonUp(1) &&
            (m_currBrushType != BrushType.Erase || m_currBrushType != BrushType.Unassigned))
        {
            RecalculateBitmaskTiles();
        }
    }


    private void SwitchCameras()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            PlayMode();
        }

        if(Input.GetKeyDown(KeyCode.E))
        {
            EditMode();
        }
    }


    private void Paint()
    {
        if(m_playMode)
            return;


        if(Input.GetMouseButton(1))
        {
            switch(m_currBrushType)
            {
                case BrushType.Floor:
                    foreach(var hit in FireRay2D())
                    {
                        var item = hit.transform.gameObject;
                        if(item.tag == BrushType.Floor.ToString())
                            break;

                        var x = (int)item.transform.position.x;
                        var y = (int)item.transform.position.y;

                        m_gameManager.m_boardManager.m_tiles[x][y] = TileType.Floor;
                        m_gameManager.m_boardManager.InstantiateFromArray(
                            m_gameManager.m_boardManager.m_floorTiles,
                            m_gameManager.m_boardManager.m_board,
                            x, y);

                        Destroy(hit.transform.gameObject);
                    }
                    break;
                case BrushType.Wall:
                    foreach(var hit in FireRay2D())
                    {
                        var item = hit.transform.gameObject;
                        if(item.tag == BrushType.Wall.ToString())
                            break;

                        var x = (int)item.transform.position.x;
                        var y = (int)item.transform.position.y;

                        m_gameManager.m_boardManager.m_tiles[x][y] = TileType.Wall;
                        m_gameManager.m_boardManager.InstantiateFromArray(
                            m_gameManager.m_boardManager.m_wallTiles,
                            m_gameManager.m_boardManager.m_board,
                            x, y);

                        Destroy(hit.transform.gameObject);
                    }
                    break;

                case BrushType.Erase:
                    Erase();
                    break;
                case BrushType.Unassigned:
                    m_currBrushType = BrushType.Unassigned;
                    break;
            }
        }

        if(Input.GetMouseButtonDown(1))
        {
            switch(m_currBrushType)
            {
                case BrushType.Enemy:
                    foreach(var hit in FireRay2D())
                    {
                        var obj = hit.transform.gameObject;
                        if(obj.tag == BrushType.Enemy.ToString())
                            break;

                        var x = (int)obj.transform.position.x;
                        var y = (int)obj.transform.position.y;

                        m_gameManager.m_boardManager.m_tiles[x][y] |= TileType.Enemy;
                        m_gameManager.m_boardManager.InstantiateFromArray(
                            m_gameManager.m_boardManager.m_enemyTiles,
                            m_gameManager.m_boardManager.m_board,
                            x, y);

                        if(hit.transform.name == "Enemy" || hit.transform.name == "Player")
                            Destroy(hit.transform.gameObject);
                    }
                    break;
                case BrushType.Player:
                    foreach(var hit in FireRay2D())
                    {
                        var obj = hit.transform.gameObject;
                        if(obj.tag == BrushType.Player.ToString())
                            break;

                        var x = (int)obj.transform.position.x;
                        var y = (int)obj.transform.position.y;

                        m_gameManager.m_boardManager.m_tiles[x][y] |= TileType.Player;
                        m_gameManager.m_boardManager.InstantiateFromArray(
                            m_gameManager.m_boardManager.m_enemyTiles,
                            m_gameManager.m_boardManager.m_board,
                            x, y);

                        if(hit.transform.name == "Enemy" || hit.transform.name == "Player")
                            Destroy(hit.transform.gameObject);
                    }
                    break;

            }
        }
    }


    private void Erase()
    {
        foreach(var hit in FireRay2D())
        {
            var item = hit.transform.gameObject;

            var x = (int)item.transform.position.x;
            var y = (int)item.transform.position.y;

            m_gameManager.m_boardManager.m_tiles[x][y] = TileType.Empty;

            Destroy(hit.transform.gameObject);
        }
    }

    private void RecalculateBitmaskTiles()
    {
        m_gameManager.m_boardManager.RemoveEdgeAndFloorTiles();
        m_gameManager.m_boardManager.BitmaskFloorEdges();

    }


    private void ChangeBrushType(BrushType type)
    {
        m_currBrushType = type;
    }


    /// <summary>
    /// Change camera to player camera if in play mode. 
    /// </summary>
    private void PlayMode()
    {
        m_playerController = FindObjectOfType<PlayerController>();
        m_playMode = true;

        if(m_playerController)
        {
            m_playerController.PlayMode = m_playMode;
        }

        if(m_cameraController)
        {
            m_cameraController.PlayMode = m_playMode;
        }
    }

    /// <summary>
    /// Change camera to edit camera if not in playmode.
    /// </summary>
    private void EditMode()
    {
        m_playerController = FindObjectOfType<PlayerController>();
        m_playMode = false;

        if(m_playerController)
        {
            m_playerController.PlayMode = m_playMode;
        }

        if(m_cameraController)
        {
            m_cameraController.PlayMode = m_playMode;
        }
    }


    /// <summary>
    /// Fire ray from mouse poisition into screen space.
    /// </summary>
    /// <returns></returns>
    public static RaycastHit2D[] FireRay2D()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        var hits = Physics2D.RaycastAll(ray.origin, ray.direction);
        return hits;
    }

}
