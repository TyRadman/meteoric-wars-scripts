using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SpaceWar
{
    public class PauseMenuSettings : Singlton<PauseMenuSettings>
    {
        [System.Serializable]
        public struct SelectionButton
        {
            public Button TheButton;
            public Slider TheSlider;
            public Toggle TheToggle;
        }

        [SerializeField] private SelectionButton[] m_Buttons;
        private int m_CurrentButtonIndex = 0;
        private const float SLIDER_VALUE_CHANGE = 0.1f;

        #region Input Methods
        public void MoveButtonSelection(KeyTag _key)
        {
            if (_key == KeyTag.Up || _key == KeyTag.Down)
            {
                m_CurrentButtonIndex += _key == KeyTag.Up ? -1 : 1;

                if (m_CurrentButtonIndex < 0)
                {
                    m_CurrentButtonIndex = m_Buttons.Length - 1;
                }

                if (m_CurrentButtonIndex > m_Buttons.Length - 1)
                {
                    m_CurrentButtonIndex = 0;
                }

                m_Buttons[m_CurrentButtonIndex].TheButton.Select();
                return;
            }

            if (_key == KeyTag.Left || _key == KeyTag.Right)
            {
                // if the selection index is for one of the sliders' buttons then we can use the left and right button to change the volume
                if (m_Buttons[m_CurrentButtonIndex].TheSlider != null)
                {
                    m_Buttons[m_CurrentButtonIndex].TheSlider.value += _key == KeyTag.Left ? -SLIDER_VALUE_CHANGE : SLIDER_VALUE_CHANGE;
                }
            }
        }

        public void SelectButton()
        {
            if (m_Buttons[m_CurrentButtonIndex].TheToggle != null)
            {
                m_Buttons[m_CurrentButtonIndex].TheToggle.isOn = !m_Buttons[m_CurrentButtonIndex].TheToggle.isOn;
            }
            else
            {
                m_Buttons[m_CurrentButtonIndex].TheButton.onClick.Invoke();
            }
        }
        #endregion

        public void SelectFirstButton()
        {
            m_CurrentButtonIndex = 0;
            m_Buttons[0].TheButton.Select();
        }

        #region Buttons Functions
        public void BackButton()
        {
            PauseMenuManager.i.PlayTransitionAnimation(PauseMenuManager.Trigger.FromSettingsToMenu);
            PauseMenuManager.i.SelectFirstButton();
        }

        public void KeyBindingButton()
        {
            PauseMenuManager.i.PlayTransitionAnimation(PauseMenuManager.Trigger.FromSettingsToKeyBinding);
            KeyBindingManager.i.SelectFirstButton();
            KeyBindingManager.i.FillKeyBindElements(0);
        }
        #endregion
    }
}