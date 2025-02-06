using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace SpaceWar
{
    public class AbilitySlotDisplay : MonoBehaviour
    {
        public enum AbilityAddType
        {
            Set = 0, Move = 1, Upgrade = 2, Replace = 3
        }

        private AbilityAddType m_AddType;
        [SerializeField] private int m_SlotIndex;
        [SerializeField] private Image m_Icon;
        [SerializeField] private Text m_KeyText;
        [SerializeField] private Text m_ActionText;
        [SerializeField] private Text m_LevelText;
        [SerializeField] private Button m_ActionButton;
        [SerializeField] private Color m_KeyActiveColor;
        [SerializeField] private Color m_KeyInactiveColor;
        private PlayerAbilities.AbilityInfo m_CurrentAbility;
        private PlayerAbilities.AbilityInfo m_PreviousAbility;
        private PlayerAbilities m_PlayerAbilities;
        private AbilityPrefab m_AbilityToAdd;
        private UnityAction m_OnPressAction;
        private bool m_Selected = false;

        // called when the slots selection screen is displayed
        public void SetUpSlot(PlayerAbilities _player, AbilityPrefab _abilityPrefab)
        {
            bool abilityExists = _player.Abilities.Exists(a => a.TheAbility != null && a.TheAbility.Tag == _abilityPrefab.Tag);

            // clear the action
            m_OnPressAction = null;
            m_AbilityToAdd = _abilityPrefab;
            m_PreviousAbility = m_CurrentAbility;
            m_PlayerAbilities = _player;
            m_CurrentAbility = m_PlayerAbilities.Abilities[m_SlotIndex];

            m_ActionButton.onClick.RemoveAllListeners();

            // if there is no ability in this slot
            if (m_CurrentAbility.TheAbility == null)
            {
                // empty the icon and text of the level of the ability
                m_Icon.sprite = null;
                m_LevelText.text = string.Empty;
                // select a color for the text and set it
                SetButtonColor(abilityExists ? GameManager.i.GeneralValues.MoveColor : GameManager.i.GeneralValues.SetColor);
                // select a text 
                m_ActionText.text = abilityExists ? "Move & Upgrade Ability" : "Set Ability";
                m_AddType = abilityExists ? AbilityAddType.Move : AbilityAddType.Set;
                m_KeyText.color = m_KeyInactiveColor;
            }
            else
            {
                m_Icon.sprite = m_CurrentAbility.TheAbility.Image;
                m_KeyText.color = m_KeyActiveColor;

                // if the the ability displayed in this slot is upgradable then display the level, if it's an addative ability then we display the amount this ability has left
                if (m_CurrentAbility.TheAbility.UpgradeType == Ability.AbilityLevelUpType.Upgradable)
                {
                    m_LevelText.text = $"Lvl {m_CurrentAbility.TheAbility.AbilityLevel + 1:0}";
                }
                else if (m_CurrentAbility.TheAbility.UpgradeType == Ability.AbilityLevelUpType.Addaditve)
                {
                    m_LevelText.text = $"Amount: {m_CurrentAbility.TheAbility.CurrentAmount:0}";
                }

                // if the new ability is the same as the ability that already exists in the slot
                if (m_CurrentAbility.TheAbility.Tag == _abilityPrefab.Tag)
                {
                    m_ActionText.text = "Upgrade Ability";
                    SetButtonColor(GameManager.i.GeneralValues.UpgradeColor);
                    m_AddType = AbilityAddType.Upgrade;
                }
                else
                {
                    m_ActionText.text = "Replace Ability";
                    SetButtonColor(GameManager.i.GeneralValues.ReplaceColor);
                    m_AddType = AbilityAddType.Replace;
                }
            }
        }

        public void ApplyAbility()
        {
            switch (m_AddType)
            {
                case AbilityAddType.Move:
                    {
                        MoveAbility();
                        break;
                    }
                case AbilityAddType.Replace:
                    {
                        ReplaceAbility();
                        break;
                    }
                case AbilityAddType.Set:
                    {
                        SetAbility();
                        break;
                    }
                case AbilityAddType.Upgrade:
                    {
                        UpgradeAbility();
                        break;
                    }
            }
        }

        private void SetAbility()
        {
            AbilitiesManager.i.OnAbilityObtained(m_AbilityToAdd);
            m_PlayerAbilities.AddAbility(m_AbilityToAdd.Prefab, m_SlotIndex);
        }

        private void MoveAbility()
        {
            m_PlayerAbilities.MoveAbility(m_AbilityToAdd.Prefab.Tag, m_SlotIndex);
        }

        private void UpgradeAbility()
        {
            AbilitiesManager.i.OnAbilityObtained(m_AbilityToAdd);
            m_PlayerAbilities.UpdateAbility(m_SlotIndex);
        }

        private void ReplaceAbility()
        {
            m_PreviousAbility.TheAbility.AbilityLevel = -1;
            m_PlayerAbilities.ReplaceAbility(m_AbilityToAdd, m_SlotIndex);
        }

        private void SetButtonColor(Color _color)
        {
            m_ActionText.color = _color;
        }

        public void SetKeyText(string key)
        {
            m_KeyText.text = key;
        }
    }
}