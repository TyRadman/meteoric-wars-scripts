using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SpaceWar.UI.Shop
{
    public class AbilitiesInfoDisplayer : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_Abilityname;
        [SerializeField] private TextMeshProUGUI m_AbilityDescription;
        [SerializeField] private List<TextMeshProUGUI> m_AbilityProperties;

        public void FillInformation(string _name, string _description)
        {
            m_Abilityname.text = _name;
            m_AbilityDescription.text = _description;
        }

        public void FillAbilityProperties(List<string> values)
        {
            m_AbilityProperties.ForEach(p => p.text = string.Empty);

            for (int i = 0; i < values.Count; i++)
            {
                m_AbilityProperties[i].text = values[i];
            }
        }

        public void ClearProperties()
        {
            m_AbilityProperties.ForEach(p => p.text = string.Empty);
        }
    }
}