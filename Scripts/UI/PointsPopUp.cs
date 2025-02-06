using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointsPopUp : Singlton<PointsPopUp>
{
    public enum PopUpTextType
    {
        Damage, Life, ShooterPoints, Coins
    }
    [System.Serializable]
    public struct PopUpColor
    {
        public PopUpTextType Type;
        public Color TextColor;
        public Vector2 ValueRange;
        public Vector2Int FontSizeRange;
        public string SpecialSign;
    }

    [SerializeField] private int m_MaxNumber;
    [SerializeField] private Transform m_Canvas;
    [SerializeField] private List<PopUpText> m_PopUpText;
    [SerializeField] private GameObject m_PopUpTextPrefab;
    [SerializeField] private List<PopUpColor> m_TextColors;
    private Camera m_Camera;

    protected override void Awake()
    {
        base.Awake();
        m_Camera = Camera.main;
        m_PopUpText = new List<PopUpText>();

        for (int i = 0; i < m_MaxNumber; i++)
        {
            var text = Instantiate(m_PopUpTextPrefab, m_Canvas).GetComponent<PopUpText>();
            m_PopUpText.Add(text);
            text.enabled = false;
        }
    }

    public void CallText(Vector2 _position, PopUpTextType _type, float _amout, bool _needCoversion = false)
    {
        if(_needCoversion)
        {
            _position = m_Camera.WorldToScreenPoint(_position);
        }

        var text = m_PopUpText.Find(t => !t.IsActive());
        var value = m_TextColors.Find(c => c.Type == _type);
        int size = (int)Mathf.Lerp(value.FontSizeRange.x, value.FontSizeRange.y, Mathf.InverseLerp(value.ValueRange.x, value.ValueRange.y, _amout));
        text.Activate(_position, _amout.ToString("0"), value.TextColor, size, value.SpecialSign);
    }
}
