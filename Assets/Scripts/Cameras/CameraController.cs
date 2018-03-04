using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour 
{
    public bool PlayMode { private get; set; }
    

    [SerializeField]
    private GameObject m_playerCamera, m_editorCamera;
    


    private void Start()
    {
        PlayMode = false;
    }

    private void Update()
    {
        if(PlayMode)
        {
            m_playerCamera.SetActive(true);
            m_editorCamera.SetActive(false);
        }
        else
        {
            m_playerCamera.SetActive(false);
            m_editorCamera.SetActive(true);
        }
    }
}
