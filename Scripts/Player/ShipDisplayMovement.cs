using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SpaceWar
{
    public class ShipDisplayMovement : MonoBehaviour
    {
        [SerializeField] private Vector2 m_MovementRange;
        [SerializeField] private float m_MovementSpeed = 3f;
        [SerializeField] private float m_TimeBetweenSteps = 1f;
        private Vector2 m_CenterPoint;
        private WaitForSeconds m_WaitBeforeMovement;

        private void Awake()
        {
            if (SceneManager.GetActiveScene().buildIndex != 0)
            {
                this.enabled = false;
            }

            m_WaitBeforeMovement = new WaitForSeconds(m_TimeBetweenSteps);
        }

        private void OnEnable()
        {
            m_CenterPoint = transform.parent.position;
            StartCoroutine(moveToPoint());
        }

        private IEnumerator moveToPoint()
        {
            // chosing a random point to move to
            Vector2 point = m_CenterPoint + m_MovementRange.RandomVector2();
            float timeToMove = 10f / m_MovementSpeed;
            float time = 0f;
            Vector2 initialPosition = transform.position;

            while (time < timeToMove)
            {
                time += Time.deltaTime;
                float t = time / timeToMove;
                transform.position = Vector2.Lerp(initialPosition, point, t);
                yield return null;
            }

            yield return m_WaitBeforeMovement;
            StartCoroutine(moveToPoint());
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }
    }
}