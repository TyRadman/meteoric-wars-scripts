using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopUpText : MonoBehaviour
{
    [SerializeField] private Animation m_Anim;
    [SerializeField] private Text m_Text;
    [SerializeField] private bool m_IsActive = false;


    public void Activate(Vector2 _position, string _text, Color _color, int _textSize, string _specialSign)
    {
        m_Text.text = _specialSign;
        transform.position = _position;
        m_Text.text += _text;
        m_Text.color = _color;
        m_Text.fontSize = _textSize;
        m_Anim.Play();
    }

    public bool IsActive()
    {
        return m_IsActive;
    }
}
