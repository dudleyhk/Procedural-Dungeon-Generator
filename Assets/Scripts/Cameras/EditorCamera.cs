using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorCamera : MonoBehaviour 
{
    [SerializeField]
    private Camera m_cam;
    [SerializeField]
    private GameObject m_player;
    [SerializeField]
    private float m_moveSpeed = 5f;

    private float m_booster = 5f;


    private void Awake()
    {
        m_cam = GetComponent<Camera>();
        m_player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        // TODO: Centre at middle of map.
        if(Input.GetKeyDown(KeyCode.C))
        {
            if(m_player)
            {
                transform.position = new Vector3(m_player.transform.position.x, m_player.transform.position.y, -10f);
            }
            else
            {
                transform.position = new Vector3(0f, 0f, -10f);
                m_player = GameObject.FindGameObjectWithTag("Player");
            }
        }
        Move();
        ZoomIt();
    }

    /// <summary>
    /// TODO: Move with middle mouse button. 
    /// Move using Arrows and WASD.
    /// </summary>
    private void Move()
    {
        var booster = 1f;
        if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            booster = m_booster;
        }

        var speed = m_moveSpeed * booster;
        if(Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            transform.position += Vector3.right * speed * Time.deltaTime;
        }

        if(Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            transform.position += Vector3.left * speed * Time.deltaTime;
        }

        if(Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            transform.position += Vector3.up * speed * Time.deltaTime;
        }

        if(Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            transform.position += Vector3.down * speed * Time.deltaTime;
        }
    }

    /// <summary>
    /// // TODO: Scroll into mouse position
    /// Zoom in with mouse wheel.
    /// </summary>
    private void ZoomIt()
    {
        var mouseWheel = Input.mouseScrollDelta;
        var newSize = m_cam.orthographicSize + -mouseWheel.y * 0.5f;

        if(newSize <= 0)
            return;

        m_cam.orthographicSize = newSize;
    }
}
