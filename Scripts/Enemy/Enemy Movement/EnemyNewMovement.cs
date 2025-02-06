using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyNewMovement : MonoBehaviour
{
    private Rigidbody2D m_Rb;
    [SerializeField] private float m_Speed = 5f;
    [SerializeField] private float m_StoppingDistance = 0.1f;

    [SerializeField] private Transform m_Target;

    private void Awake()
    {
        m_Rb = GetComponent<Rigidbody2D>();
        GetRandomTarget();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            GetRandomTarget();
        }

        MoveEnemy(m_Target.position, m_Speed);
    }

    private void FixedUpdate()
    {
        if (Vector2.Distance(transform.position, m_Target.position) < m_StoppingDistance) // add a small threshold to prevent jitter
        {
            m_Rb.velocity = Vector2.zero;
        }
    }

    public void MoveEnemy(Vector2 targetPosition, float moveSpeed)
    {
        Vector2 currentPosition = m_Rb.position;
        Vector2 direction = (targetPosition - currentPosition).normalized;
        m_Rb.velocity = moveSpeed * direction;
    }


    private void GetRandomTarget()
    {
        float width = 16 / 2;
        float height = 10 / 2;
        m_Target.position = new Vector2(Random.Range(-width, width), Random.Range(-height, height)); ;
    }
}
