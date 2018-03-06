using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour 
{
    public bool PlayMode;

    [SerializeField]
    private Rigidbody2D m_rb;
    [SerializeField]
    private Animator m_animator;
    [SerializeField]
    private float m_movementSpeed = 0.5f;
    [SerializeField]
    private float m_damp = -0.01f;

    [SerializeField]
    private bool m_moveUp, m_moveRight, m_moveDown, m_moveLeft, m_pickSwing = false;
    private bool m_idle = false;

    private void Awake()
    {
        m_rb = GetComponent<Rigidbody2D>();
        m_animator = GetComponent<Animator>();

        PlayMode = false;

        // TODO: ERROR CHECK
    }

    private void Start()
    {
        // TODO: Set this up in the game manager instead.
        if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == 
            "Main")
            PlayMode = true;
    }


    private void Update()
    {
        if(!PlayMode) return;

        if(Input.GetMouseButton(0))
        {
            m_pickSwing = true;
        }
        if(Input.GetMouseButtonUp(0))
        {
            m_pickSwing = false;
        }

        if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            m_moveUp = true;
        }
        if(Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            m_moveDown = true;
        }
        if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            m_moveLeft = true;
        }
        if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
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

        UpdateAnimations();
    }

    private void FixedUpdate()
    {
        if(m_moveUp)
        {
            m_rb.velocity = new Vector2(0f, m_movementSpeed * 1 * Time.fixedDeltaTime);
        }
        if(m_moveRight)
        {
            m_rb.velocity = new Vector2(m_movementSpeed * 1 * Time.fixedDeltaTime, 0f);
        }
        if(m_moveLeft)
        {
            m_rb.velocity = new Vector2(m_movementSpeed * -1 * Time.fixedDeltaTime,0f);
        }
        if(m_moveDown)
        {
            m_rb.velocity = new Vector2(0f, m_movementSpeed * -1 * Time.fixedDeltaTime);
        }
        if(m_idle)
        {
            m_rb.velocity = Vector2.zero;
        }

        m_moveUp = m_moveRight = m_moveDown = m_moveLeft = false;
    }


    private void UpdateAnimations()
    {
        // Flip direction
        if(m_rb.velocity.x > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        if(m_rb.velocity.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }

        if(m_pickSwing)
        {
            ChangeAnimationState(1);
        }
        else
        {
            ChangeAnimationState(0);
        }
    }


    private void ChangeAnimationState(int value)
    {
        m_animator.SetInteger("AnimState", value);
    }
}
