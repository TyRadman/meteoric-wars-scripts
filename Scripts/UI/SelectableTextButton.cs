using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace SpaceWar.UI
{
    public class SelectableTextButton : UISelectable
    {
        [SerializeField] private TextMeshProUGUI m_ButtonText;
        [SerializeField] private Color m_HighlightColor;
        [SerializeField] private Color m_DehighlightColor;

        public override void Highlight()
        {
            base.Highlight();
            m_ButtonText.color = m_HighlightColor;
        }

        public override void Dehighlight()
        {
            base.Dehighlight();
            m_ButtonText.color = m_DehighlightColor;
        }
    }
}
