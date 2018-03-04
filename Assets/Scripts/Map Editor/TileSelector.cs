using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSelector : MonoBehaviour 
{
    [SerializeField]
    private PlayerController m_playerController;
    [SerializeField]
    private CameraController m_cameraController;


    [SerializeField]
    private bool m_playMode = false;

    
    private void Init()
    {
        m_playerController = FindObjectOfType<PlayerController>();
        m_cameraController = FindObjectOfType<CameraController>();
    }


    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            FireRay2D();
        }

        if(Input.GetKeyDown(KeyCode.P))
        {
            PlayMode();
        }

        if(Input.GetKeyDown(KeyCode.E))
        {
            EditMode();
        }
    }

    public static RaycastHit2D[] FireRay2D()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        var hits = Physics2D.RaycastAll(ray.origin, ray.direction);
        return hits;
    }


    private void PlayMode()
    {
        Init();

        if(m_playerController)
        {
            m_playerController.PlayMode = true;
        }

        if(m_cameraController)
        {
            m_cameraController.PlayMode = true;
        }
    }


    private void EditMode()
    {
        Init();

        if(m_playerController)
        {
            m_playerController.PlayMode = false;
        }

        if(m_cameraController)
        {
            m_cameraController.PlayMode = false;
        }
    }
}
