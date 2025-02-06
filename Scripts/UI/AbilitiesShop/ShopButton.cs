using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

namespace SpaceWar.UI.Shop
{
    public class ShopButton : UISelectable
    {
        [SerializeField] private Color m_ActiveColor;
        [SerializeField] private Color m_InactiveColor;
        [SerializeField] private TextMeshProUGUI m_ButtonText;
        public Event OnClicked { get; private set; }

        public override void Highlight()
        {
            base.Highlight();
            m_ButtonText.color = m_ActiveColor;
        }

        public override void Dehighlight()
        {
            base.Dehighlight();
            m_ButtonText.color = m_InactiveColor;
        }

        public override void OnClickPerformed()
        {
            base.OnClickPerformed();

        }
    }
}
