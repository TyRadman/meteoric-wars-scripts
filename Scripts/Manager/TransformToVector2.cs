using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformToVector2 : MonoBehaviour
{
    [SerializeField] private Transform m_Transform;
    [SerializeField] private Vector2 m_Vector2;

    private void OnValidate()
    {
        if (m_Transform != null)
        {
            m_Vector2 = m_Transform.localPosition;
        }
    }
}
