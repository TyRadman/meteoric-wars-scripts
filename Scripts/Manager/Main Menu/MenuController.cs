using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.InputSystem;

namespace SpaceWar.MainMenu
{
    public class MenuController : Singlton<MenuController>
    {
        [System.Serializable]
        public class MenuButtons
        {
            public MainMenuAnimations.AnimationType Type;
            public List<Button> Buttons;
            public int CurrentButtonIndex = 0;

            public void SelectButton()
            {
                Buttons[CurrentButtonIndex].OnSelect(null);
                Buttons[CurrentButtonIndex].Select();
            }

            public void InvokeButton()
            {
                Buttons[CurrentButtonIndex].onClick.Invoke();
            }
        }

        [System.Serializable]
        public class MenuActions
        {
            public MainMenuAnimations.AnimationType State;
            public UnityAction<Vector2, int> NavigationAction;
            public UnityAction<int> SelectionAction;
            public UnityAction<int> ReturnAction;
        }

        public List<MenuActions> Actions = new List<MenuActions>() { new MenuActions(), new MenuActions() };
        [SerializeField] private List<MenuButtons> m_ButtonGroups;
        private MenuButtons m_CurrentButtonsGroup;

        protected override void Awake()
        {
            base.Awake();
            AssignMenuSwitching(SwitchMenu, SelectButton, ReturnButton);
            m_CurrentButtonsGroup = m_ButtonGroups.Find(g => g.Type == MainMenuAnimations.AnimationType.MainMenu);
        }

        private void AssignMenuSwitching(UnityAction<Vector2, int> _movement, UnityAction<int> _select, UnityAction<int> _return, int _playerIndex = -1)
        {
            if (_playerIndex == -1)
            {
                for (int i = 0; i < Actions.Count; i++)
                {
                    Actions[i].NavigationAction += _movement;
                    Actions[i].SelectionAction += _select;
                    Actions[i].ReturnAction += _return;
                }
            }
            else
            {
                Actions[_playerIndex].NavigationAction += _movement;
                Actions[_playerIndex].SelectionAction += _select;
                Actions[_playerIndex].ReturnAction += _return;
            }
        }

        private void ClearActions(int _playerIndex = -1)
        {
            if (_playerIndex == -1)
            {
                for (int i = 0; i < Actions.Count; i++)
                {
                    Actions[i].NavigationAction = null;
                    Actions[i].SelectionAction = null;
                    Actions[i].ReturnAction = null;
                }
            }
            else
            {
                Actions[_playerIndex].NavigationAction = null;
                Actions[_playerIndex].SelectionAction = null;
                Actions[_playerIndex].ReturnAction = null;
            }
        }

        public void SwitchMenu(Vector2 _value, int _playerIndex)
        {
            if (_value.y > 0)
            {
                m_CurrentButtonsGroup.CurrentButtonIndex = (int)Helper.GetNextValueWithinRange(m_CurrentButtonsGroup.CurrentButtonIndex, -1f, m_CurrentButtonsGroup.Buttons.Count - 1, 0f);
            }
            else if (_value.y < 0)
            {
                m_CurrentButtonsGroup.CurrentButtonIndex = (int)Helper.GetNextValueWithinRange(m_CurrentButtonsGroup.CurrentButtonIndex, 1f, m_CurrentButtonsGroup.Buttons.Count - 1, 0f);
            }

            m_CurrentButtonsGroup.SelectButton();
        }

        public void SelectButton(int _playerIndex)
        {
            m_CurrentButtonsGroup.InvokeButton();
        }

        public void ReturnButton(int _playerIndex)
        {

        }

        public void SwitchInput(MainMenuAnimations.AnimationType _type, int _playerIndex = -1, bool _keepOldIndex = true)
        {
            SetCurrentButtons(_type, _keepOldIndex, _playerIndex);
        }

        private void SetCurrentButtons(MainMenuAnimations.AnimationType _layer, bool _keepOldIndex, int _playerIndex)
        {
            // Reset  the index if it's more suitable
            if (!_keepOldIndex) m_CurrentButtonsGroup.CurrentButtonIndex = 0;

            if (_playerIndex == -1)
            {
                Actions.ForEach(a => a.State = _layer);
            }
            else
            {
                Actions[_playerIndex].State = _layer;
            }

            switch (_layer)
            {
                case MainMenuAnimations.AnimationType.MainMenu:
                    {
                        SwitchMenus(_layer);
                        break;
                    }
                case MainMenuAnimations.AnimationType.Modes:
                    {
                        SwitchMenus(_layer);
                        break;
                    }
                case MainMenuAnimations.AnimationType.PlayerSelectionWindow:
                    {
                        PriorPlayerSelection(_playerIndex);
                        break;
                    }
                case MainMenuAnimations.AnimationType.PlayerConfirmationSelection:
                    {
                        OnPlayerConfirmationSelection(_playerIndex);
                        break;
                    }
                case MainMenuAnimations.AnimationType.PlayerSelectionProcess:
                    {
                        OnPlayerSelection(_playerIndex);
                        break;
                    }
            }
        }

        /// <summary>
        /// Navigating through menus (for both players)
        /// </summary>
        /// <param name="_layer"></param>
        private void SwitchMenus(MainMenuAnimations.AnimationType _layer)
        {
            // clear the actions
            ClearActions();
            // assign the actions
            AssignMenuSwitching(SwitchMenu, SelectButton, ReturnButton);
            // assign the button group to be used
            m_CurrentButtonsGroup = m_ButtonGroups.Find(b => b.Type == _layer);
            m_CurrentButtonsGroup.SelectButton();
        }

        private void PriorPlayerSelection(int _playerIndex)
        {
            if (Actions.Exists(a => a.State == MainMenuAnimations.AnimationType.PlayerSelectionProcess))
            {
                ClearActions(_playerIndex);
                AssignMenuSwitching(null, PlayerSelectionControl.i.StartShipSelection, PlayerSelectionControl.i.ReturnToModes, _playerIndex);
            }
            else
            {
                ClearActions();
                AssignMenuSwitching(null, PlayerSelectionControl.i.StartShipSelection, PlayerSelectionControl.i.ReturnToModes);
            }
        }

        private void OnPlayerSelection(int _playerIndex)
        {
            // remove the older methods
            ClearActions(_playerIndex);
            // assign the new ones
            AssignMenuSwitching(PlayerSelectionControl.i.MoveSelection, PlayerSelectionControl.i.SelectShip, PlayerSelectionControl.i.ReturnFromSelectionProcess, _playerIndex);
        }

        private void OnPlayerConfirmationSelection(int _playerIndex)
        {
            ClearActions(_playerIndex);
            AssignMenuSwitching(null, null, PlayerSelectionControl.i.ReturnToSelectionProcess, _playerIndex);
        }
    }
}