using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private float m_MovementSpeed = 1f;
    [SerializeField] private float m_StepSize = 2f;
    [SerializeField] private float m_WaitingTime = 1f;

    private void Start()
    {
        StartCoroutine(MovementProcess());
    }

    IEnumerator MovementProcess()
    {
        float levelXBorders = LevelDimensions.Instance.LevelWidth / 2 - 1;
        float time;
        float multiplier = 1;
        bool threePoints = false;
        Vector3 target;
        Vector3 intermediatePoint = Vector3.zero;
        Vector3 previousPosition;

        while (true)
        {
            target = transform.position + Vector3.right * m_StepSize * multiplier;

            if (target.x > levelXBorders || target.x < -levelXBorders)
            {
                // the position that the enemy will rest at
                target -= (Mathf.Abs(target.x) - levelXBorders) * Vector3.right * 2 * multiplier;
                // the edge that will act as an intermediate point between the enemy and the target point
                intermediatePoint = new Vector3(levelXBorders * multiplier, 0f, target.z);
                
                // the enemy will go through three points in this step since it reached the edge
                threePoints = true;
                multiplier *= -1;
            }

            time = 0f;
            previousPosition = transform.position;

            if (!threePoints)
            // moving the transform if it's between two points
            {
                while (time < m_MovementSpeed)
                {
                    time += Time.deltaTime;
                    transform.position = Vector3.Lerp(previousPosition, target, time / m_MovementSpeed);

                    yield return null;
                }
            }
            // if it's going to the edge and back it means that we have three points therefore, two lerps
            else
            {
                float newTime = Vector3.Distance(transform.position, intermediatePoint) / m_StepSize * m_MovementSpeed;
                
                while (time < newTime)
                {
                    time += Time.deltaTime;
                    transform.position = Vector3.Lerp(previousPosition, intermediatePoint, time / newTime);

                    yield return null;
                }

                newTime = Vector3.Distance(intermediatePoint, target) / m_StepSize * m_MovementSpeed;
                time = 0f;

                while (time < newTime)
                {
                    time += Time.deltaTime;
                    transform.position = Vector3.Lerp(intermediatePoint, target, time / newTime);

                    yield return null;
                }

                threePoints = false;
            }

            yield return new WaitForSeconds(m_WaitingTime);
        }
    }
}
