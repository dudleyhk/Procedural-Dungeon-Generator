using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour 
{
    [SerializeField]
    private Rigidbody2D m_rb;
    [SerializeField]
    private Animator m_animator;
    [SerializeField]
    private float m_movementSpeed = 0.5f;
    [SerializeField]
    private float m_damp = -0.01f;


    private bool m_moveUp, m_moveRight, m_moveDown, m_moveLeft = false;
    private bool m_idle = false;

    private void Awake()
    {
        m_rb = GetComponent<Rigidbody2D>();
        m_animator = GetComponent<Animator>();

        // TODO: ERROR CHECK
    }

    private void Update()
    {
        if(Input.GetKey(KeyCode.UpArrow))
        {
            m_moveUp = true;
        }
        if(Input.GetKey(KeyCode.DownArrow))
        {
            m_moveDown = true;
        }
        if(Input.GetKey(KeyCode.LeftArrow))
        {
            m_moveLeft = true;
        }
        if(Input.GetKey(KeyCode.RightArrow))
        {
            m_moveRight = true;
        }
        if(!m_moveUp && !m_moveLeft && !m_moveDown && !m_moveRight)
        {
            m_idle = true;
        }
        else
        {
            m_idle = false;
        }
    }



    private void FixedUpdate()
    {
        if(m_moveUp)
        {
            m_rb.velocity = new Vector2(m_rb.velocity.x, m_movementSpeed * 1 * Time.fixedDeltaTime);
        }
        if(m_moveRight)
        {
            m_rb.velocity = new Vector2(m_movementSpeed * 1 * Time.fixedDeltaTime, m_rb.velocity.y);
        }
        if(m_moveLeft)
        {
            m_rb.velocity = new Vector2(m_movementSpeed * -1 * Time.fixedDeltaTime, m_rb.velocity.y);
        }
        if(m_moveDown)
        {
            m_rb.velocity = new Vector2(m_rb.velocity.x, m_movementSpeed * -1 * Time.fixedDeltaTime);
        }
        if(m_idle)
        {
            m_rb.velocity = Vector2.zero;
        }

        m_moveUp  = m_moveRight = m_moveDown = m_moveLeft = false;
    }
}
