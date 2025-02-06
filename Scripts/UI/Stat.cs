using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace SpaceWar
{
    public class Stat : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_ValueText;
        [SerializeField] private CanvasGroup m_CanvasGroup;
        public StatTag Tag;
        public float Value;

        public void SetValue(string _value)
        {
            m_ValueText.text = _value;
        }

        public void SetAlpha(float _value)
        {
            m_CanvasGroup.alpha = _value;
        }
    }
}
