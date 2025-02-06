using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueBox : MonoBehaviour
{
    [SerializeField] private TextMeshPro m_Text;
    [SerializeField] private Transform m_LeftPoint;
    [SerializeField] private Transform m_RightsPoint;
    [SerializeField] private Transform m_Body;

    public void PrintMessage(string _message, Vector2 _position)
    {
        // position the box
        // if it's on the left then show the box on the right
        Transform anchor = _position.x < 0 ? m_LeftPoint : m_RightsPoint;
        m_Body.parent = null;
        transform.parent = anchor;
        transform.localPosition = Vector2.zero;
        transform.parent = null;
        m_Body.parent = transform;
        transform.position = _position;
        m_Body.parent = null;
        transform.position = m_Body.position;
        m_Body.parent = transform;
        // typing the message
        m_Text.text = _message;
    }
}
