using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SpaceWar.UI.Shop
{
    public class AbilitySelectionUnit : UISelectable
    {
        [SerializeField] private Image m_HighlightImage;
        [SerializeField] private Image m_IconImage;
        [SerializeField] private TextMeshProUGUI m_PriceText;
        [SerializeField] private Image m_PriceIcon;
        [SerializeField] private TextMeshProUGUI m_AmountText;
        [SerializeField] private List<Image> m_LevelImages;
        [SerializeField] private Color m_DisabledSlotColor;
        [SerializeField] private CanvasGroup m_CanvasGroup;
        public AbilityPrefab AbilityPrefab { get; private set; }
        public AbilityTag Tag;

        public void SetLevel(int _level)
        {
            m_LevelImages.ForEach(l => l.enabled = false);

            for (int i = 0; i < _level; i++)
            {
                m_LevelImages[i].enabled = true;
            }
        }

        public void SetPrice(int _price)
        {
            m_PriceText.text = $"{_price:00}";
        }

        public override void Highlight()
        {
            m_HighlightImage.enabled = true;
        }

        public override void Dehighlight()
        {
            m_HighlightImage.enabled = false;

        }

        public void Activate()
        {
            IsAvailable = true;
            m_IconImage.color = Color.white;
            m_PriceIcon.enabled = true;
        }

        public void Deactivate()
        {
            IsAvailable = false;
            m_IconImage.color = m_DisabledSlotColor;
            m_PriceIcon.enabled = false;
            m_PriceText.text = string.Empty;
            m_AmountText.text = string.Empty;
        }

        public void ApplyAbility(AbilityPrefab _ability)
        {
            // if the ability is already purchased, then hide the text and deactivate the slot
            Activate();
            AbilityPrefab = _ability;

            //if (_ability.Purchased)
            //{
            //    return;
            //}

            m_IconImage.sprite = _ability.Prefab.Image;
            m_PriceText.text = $"{_ability.Prefab.Price}";
            m_AmountText.text = _ability.Prefab.Amount > 0 ? $"{_ability.Prefab.Amount}" : string.Empty;
            Tag = _ability.Tag;

            // setting up level
            m_LevelImages.ForEach(l => l.enabled = false);

            for (int i = 0; i < _ability.Level; i++)
            {
                m_LevelImages[i].enabled = true;
            }
        }
    }
}