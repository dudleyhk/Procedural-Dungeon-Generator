using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField]
    private GameObject m_target;
    [SerializeField]
    private Vector3 m_targetPosition;
    [SerializeField]
    private float m_moveSpeed;
    [SerializeField]
    private bool init = false;


    private void Awake()
    {
        Init();
    }


    private void Update()
    {
        if(!init)
        {
            Init();
        }
        else
        {
            SetTargetPosition();
           transform.position = Vector3.MoveTowards(transform.position, m_targetPosition, m_moveSpeed * Time.deltaTime);
        }
    }

    private void Init()
    {
        m_target = GameObject.FindGameObjectWithTag("Player");
        if(m_target != null)
        {
            init = true;
            transform.position = new Vector3(
                m_target.transform.position.x,
                m_target.transform.position.y,
                transform.position.z);
        }
        else
        {
            Debug.LogWarning("Warning: Player object with tag 'Player' not found");
        }
    }


    private void SetTargetPosition()
    {
        //TODO: Error check target.
        m_targetPosition = new Vector3(
            m_target.transform.position.x,
            m_target.transform.position.y,
            transform.position.z);
    }
}
