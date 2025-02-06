using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SpaceWar
{
    public class PauseMenuManager : Singlton<PauseMenuManager>
    {
        [SerializeField] private Canvas m_PauseMenuCanvas;
        public bool IsPaused = false;
        private int m_CurrentButtonsIndex = 0;
        [SerializeField] private Button[] m_Buttons;
        [SerializeField] private Animator m_Anim;
        //private InputState m_LastInputState;

        public enum Trigger
        {
            FromSettingsToMenu, FromMenuToSettings, FromSettingsToKeyBinding, FromKeyBindingToSettings
        }

        protected override void Awake()
        {
            base.Awake();

            if (m_PauseMenuCanvas != null) m_PauseMenuCanvas.enabled = false;
        }

        private void Update()
        {
            //if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Joystick1Button7))
            //{
            //    Toggle();
            //}
        }

        public void Toggle()
        {
            // cache what was the player doing before pausing so that it's return after unpausing
            if (!IsPaused)
            {
                //m_LastInputState = GameManager.Instance.PlayersManager.Players[0].Components.Input.GetInputState();
                m_Buttons[0].Select();
            }

            // toggle pausing bool
            IsPaused = !IsPaused;
            // show and hide the pause menu
            m_PauseMenuCanvas.enabled = IsPaused;
            // freeze the game and unfreeze it
            Time.timeScale = IsPaused ? 0f : 1f;
            // change the player input state depending on the pause state
            //GameManager.Instance.PlayersManager.Players.ForEach(p => p.Components.Input.ChangeInputState(IsPaused? InputState.PauseMenu : m_LastInputState));
        }

        public void MainMoveButton(KeyTag _key)
        {
            if (_key == KeyTag.Up)
            {
                m_CurrentButtonsIndex--;

                if (m_CurrentButtonsIndex < 0)
                {
                    m_CurrentButtonsIndex = m_Buttons.Length - 1;
                }

                m_Buttons[m_CurrentButtonsIndex].Select();
                return;
            }

            if (_key == KeyTag.Down)
            {
                m_CurrentButtonsIndex++;

                if (m_CurrentButtonsIndex > m_Buttons.Length - 1)
                {
                    m_CurrentButtonsIndex = 0;
                }

                m_Buttons[m_CurrentButtonsIndex].Select();
                return;
            }
        }

        public void SelectMainButton()
        {
            m_Buttons[m_CurrentButtonsIndex].onClick.Invoke();
        }

        public void SelectFirstButton()
        {
            m_Buttons[m_CurrentButtonsIndex].Select();
        }

        #region Windows Transition Functions
        public void PlayTransitionAnimation(Trigger _trigger)
        {
            m_Anim.SetTrigger(_trigger.ToString());
            //GameManager.Instance.PlayersManager.Players.ForEach(p => p.Components.Input.ChangeInputState(_inputState));
        }
        #endregion

        #region Main Button Functionality
        public void RestartButton()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(1);
        }

        public void SettingsButton()
        {
            //PlayTransitionAnimation(Trigger.FromMenuToSettings, InputState.SettingsMenu);
            // select the first button in the settings menu
            PauseMenuSettings.i.SelectFirstButton();
        }

        public void QuitButton()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(0);
        }
        #endregion
    }
}