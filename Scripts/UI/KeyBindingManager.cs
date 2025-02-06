using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SpaceWar
{
    public class KeyBindingManager : Singlton<KeyBindingManager>
    {
        [System.Serializable]
        public struct ButtonSelection
        {
            public Button TheButton;
            [HideInInspector] public KeyBend TheKeyBind;
            [HideInInspector] public bool HasKeyBind;
        }

        [SerializeField] private ButtonSelection[] m_Buttons;
        private int m_CurrentButtonIndex = 0;
        public bool IsKeyBindSelecting = false;
        [HideInInspector] public int PlayerIndex = 0;
        private KeyTag m_SelectedKeyTag;
        // for switching keyboard and controller
        public bool IsKeyboard = false;
        [SerializeField] private Image m_InputIconImage;
        [SerializeField] private Sprite m_KeyboardIcon;
        [SerializeField] private Sprite m_ControllerIcon;
        private PlayerShipInput[] m_PlayersKeyboardInput;
        private PlayerControllerInput[] m_PlayersControllerInput;

        protected override void Awake()
        {
            base.Awake();
            return;

            // caching the key bends
            for (int i = 0; i < m_Buttons.Length; i++)
            {
                if (m_Buttons[i].TheButton.GetComponent<KeyBend>() != null)
                {
                    m_Buttons[i].TheKeyBind = m_Buttons[i].TheButton.GetComponent<KeyBend>();
                    m_Buttons[i].HasKeyBind = true;
                }
                else
                {
                    m_Buttons[i].HasKeyBind = false;
                }
            }
        }

        public void StartKeyBinding()
        {
            cachePlayersInputData();

            for (int i = 0; i < GameManager.i.PlayersManager.NumberOfPlayers; i++)
            {
                PlayerIndex = i;
                switchInputType(IsKeyboard);
            }
        }

        private void cachePlayersInputData()
        {
            int numberOfPlayers = GameManager.i.PlayersManager.NumberOfPlayers;
            var players = GameManager.i.PlayersManager.Players;
            m_PlayersKeyboardInput = new PlayerShipInput[numberOfPlayers];
            m_PlayersControllerInput = new PlayerControllerInput[numberOfPlayers];

            for (int i = 0; i < numberOfPlayers; i++)
            {
                m_PlayersKeyboardInput[i] = players[i].Components.Input;
                m_PlayersControllerInput[i] = players[i].Components.GetComponent<PlayerControllerInput>();
            }
        }

        public void FillKeyBindElements(int _playerIndex)
        {
            var keys = GameManager.i.PlayersManager.Keys[_playerIndex].Keys;
            PlayerIndex = _playerIndex;

            if (keys.Count != m_Buttons.Length - 4)
            {
                Debug.LogError($"Keys and binding keys number is not matching. Curtom keys count = {keys.Count} while buttons count = {m_Buttons.Length} - 3 = {m_Buttons.Length - 3}");
            }

            int keyIndex = 0;

            for (int i = 0; i < m_Buttons.Length; i++)
            {
                if (m_Buttons[i].HasKeyBind)
                {
                    m_Buttons[i].TheKeyBind.FillInfo(keys[keyIndex++]);
                }
            }

        }

        public void SelectFirstButton()
        {
            m_CurrentButtonIndex = 0;
            m_Buttons[0].TheButton.Select();
        }

        #region Navigation Methods
        public void MoveButtonSelection(KeyTag _key)
        {
            // if a key bind is selected then we can't navigate unless we select a key
            if (IsKeyBindSelecting)
            {
                return;
            }

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

                // if the next button to select is disabled, then we move to the next button
                if (!m_Buttons[m_CurrentButtonIndex].TheButton.interactable)
                {
                    MoveButtonSelection(_key);
                    return;
                }

                m_Buttons[m_CurrentButtonIndex].TheButton.Select();
                return;
            }
        }

        public void SelectButton(bool _IsController = false)
        {
            // if a key is highlighted and is waiting to be set
            if (IsKeyBindSelecting)
            {
                return;
            }

            // we can bind keys only if we're using a keyboard
            if (m_Buttons[m_CurrentButtonIndex].HasKeyBind && !_IsController)
            {
                m_Buttons[m_CurrentButtonIndex].TheKeyBind.OnSelected();
                IsKeyBindSelecting = true;
                m_SelectedKeyTag = m_Buttons[m_CurrentButtonIndex].TheKeyBind.Tag;
                //GameManager.Instance.PlayersManager.Players.ForEach(p => p.Components.Input.ChangeInputState(InputState.SelectKeyBind));
            }
            // if we selected anything else in the menu just do whatever the button does
            else
            {
                m_Buttons[m_CurrentButtonIndex].TheButton.onClick.Invoke();
            }
        }
        #endregion

        #region Key Binding 
        public void SetKeyBind(KeyCode _key)
        {
            // change the button of the action in code
            var keys = GameManager.i.PlayersManager.Keys[PlayerIndex];
            int index = keys.Keys.FindIndex(k => k.Tag == m_SelectedKeyTag);
            keys.Keys[index] = new KeySet { Tag = m_SelectedKeyTag, TheButton = _key };
            // change the text in the binding keys menu
            m_Buttons[m_CurrentButtonIndex].TheKeyBind.KeyText.text = _key.ToString();
            StartCoroutine(enableSelection());
            // select the last selected button
            m_Buttons[m_CurrentButtonIndex].TheButton.Select();
            // set the buttons to the player
            GameManager.i.PlayersManager.Players[PlayerIndex].Components.Input.SetUpButtons(keys);
            // return control the buttons 
            //GameManager.Instance.PlayersManager.Players.ForEach(p => p.Components.Input.ChangeInputState(InputState.KeyBindingMenu));
        }

        private IEnumerator enableSelection()
        {
            yield return new WaitForSecondsRealtime(0.3f);
            IsKeyBindSelecting = false;
        }
        #endregion

        #region Buttons Methods
        public void ResetToDefaultButton()
        {
            // clears the keys that are set currently for the selected player
            var playerKeys = GameManager.i.PlayersManager.Keys[PlayerIndex].Keys;
            playerKeys.Clear();
            playerKeys = new List<KeySet>();
            // cache the default keys
            var defaultKeys = GameManager.i.PlayersManager.DefaultKeys[PlayerIndex].Keys;
            // add the default keys to the player's keys
            defaultKeys.ForEach(k => GameManager.i.PlayersManager.Keys[PlayerIndex].Keys.Add(new KeySet { Tag = k.Tag, TheButton = k.TheButton }));
            // set up the buttons for the player that is already spawned (the previous steps was to save it in the data base)
            GameManager.i.PlayersManager.Players[PlayerIndex].Components.Input.SetUpButtons(GameManager.i.PlayersManager.Keys[PlayerIndex]);
            // this will apply the change to the player movement script which requires some tweaking
            // update the UI
            FillKeyBindElements(PlayerIndex);
        }

        public void BackButton()
        {
            //PauseMenuManager.Instance.PlayTransitionAnimation(PauseMenuManager.Trigger.FromKeyBindingToSettings, InputState.SettingsMenu);
            PauseMenuSettings.i.SelectFirstButton();
        }

        public void SwitchInputType()
        {
            switchInputType(!IsKeyboard);
        }

        private void switchInputType(bool _IsKeyboard)
        {
            // toggle input type
            IsKeyboard = _IsKeyboard;

            DisableKeyBinding(IsKeyboard);

            if (!IsKeyboard)
            {
                m_InputIconImage.sprite = m_ControllerIcon;
            }
            else
            {
                m_InputIconImage.sprite = m_KeyboardIcon;
            }

            m_PlayersKeyboardInput[PlayerIndex].Activate(IsKeyboard);
            m_PlayersControllerInput[PlayerIndex].Activate(!IsKeyboard);
        }

        public void DisableKeyBinding(bool _enable)
        {
            for (int i = 0; i < m_Buttons.Length; i++)
            {
                if (m_Buttons[i].HasKeyBind)
                {
                    m_Buttons[i].TheButton.interactable = _enable;
                }
            }
        }
        #endregion
    }
}