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

    private BrushType m_currBrushType = BrushType.Unassigned;


    private void Awake()
    {
        m_boardManager = FindObjectOfType<BoardManager>();
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
        if(Input.GetMouseButton(1))
        {
            Paint();
        }

        if(Input.GetMouseButtonUp(1) && 
            (m_currBrushType != BrushType.Erase || m_currBrushType != BrushType.Unassigned))
        {
            RecalculateBitmaskTiles();
        }
    }


    private void Paint()
    {
        switch(m_currBrushType)
        {
            case BrushType.Floor:
                foreach(var hit in TileSelector.FireRay2D())
                {
                    var item = hit.transform.gameObject;
                    if(item.tag == BrushType.Floor.ToString()) break;
                    
                    var x = (int)item.transform.position.x;
                    var y = (int)item.transform.position.y;

                    m_boardManager.m_tiles[x][y] |= TileType.Floor;

                    BoardManager.InstantiateFromArray(m_boardManager.m_floorTiles, m_boardManager.m_board, x, y);
                    Destroy(hit.transform.gameObject);
                }
                break;
            case BrushType.Wall:
                foreach(var hit in TileSelector.FireRay2D())
                {
                    var item = hit.transform.gameObject;
                    if(item.tag == BrushType.Wall.ToString())
                        break;

                    var x = (int)item.transform.position.x;
                    var y = (int)item.transform.position.y;

                    m_boardManager.m_tiles[x][y] = TileType.Wall;

                    BoardManager.InstantiateFromArray(m_boardManager.m_wallTiles, m_boardManager.m_board, x, y);
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


    private void Erase()
    {
        foreach(var hit in TileSelector.FireRay2D())
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
        m_boardManager.BitmaskFloorEdges(false);

    }


    private void ChangeBrushType(BrushType type)
    {
        m_currBrushType = type;
    }
}
