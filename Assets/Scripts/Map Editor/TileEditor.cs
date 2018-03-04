using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum BrushType
{
    Floor,
    Wall,
    Player,
    Erase,
    Unassigned
}


public class TileEditor : MonoBehaviour
{
    [SerializeField]
    private BoardManager m_boardManager;
    [SerializeField]
    private PlayerController m_playerController;
    [SerializeField]
    private CameraController m_cameraController;
    [SerializeField]
    private bool m_playMode = false;


    private BrushType m_currBrushType = BrushType.Unassigned;


    private void Awake()
    {
        m_boardManager = FindObjectOfType<BoardManager>();
        m_cameraController = FindObjectOfType<CameraController>();
        // TODO: Error check
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
        if(m_playMode) return;

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

                        m_boardManager.m_tiles[x][y] = TileType.Floor;
                        m_boardManager.InstantiateFromArray(m_boardManager.m_floorTiles, m_boardManager.m_board, x, y);
                        
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

                        m_boardManager.m_tiles[x][y] = TileType.Wall;
                        m_boardManager.InstantiateFromArray(m_boardManager.m_wallTiles, m_boardManager.m_board, x, y);

                        Destroy(hit.transform.gameObject);
                    }
                    break;
                case BrushType.Player:
                    break;
                case BrushType.Erase:
                    Erase();
                    break;
                case BrushType.Unassigned:
                    m_currBrushType = BrushType.Unassigned;
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

            m_boardManager.m_tiles[x][y] = TileType.Empty;

            Destroy(hit.transform.gameObject);
        }
    }

    private void RecalculateBitmaskTiles()
    {
        m_boardManager.BitmaskPreRemove();
        m_boardManager.BitmaskFloorEdges(false);

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

        if(m_playerController)
        {
            m_playerController.PlayMode = true;
        }

        if(m_cameraController)
        {
            m_cameraController.PlayMode = true;
        }
    }

    /// <summary>
    /// Change camera to edit camera if not in playmode.
    /// </summary>
    private void EditMode()
    {
        m_playerController = FindObjectOfType<PlayerController>();

        if(m_playerController)
        {
            m_playerController.PlayMode = false;
        }

        if(m_cameraController)
        {
            m_cameraController.PlayMode = false;
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
