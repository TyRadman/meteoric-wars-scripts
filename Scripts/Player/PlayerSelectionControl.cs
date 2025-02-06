using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

namespace SpaceWar.MainMenu
{
    public class PlayerSelectionControl : Singlton<PlayerSelectionControl>
    {
        // the references of different elements on the player selection menu (no functionality)
        [SerializeField] private List<PlayerSelectionPanelReferences> m_PlayerSelections;
        [SerializeField] private List<TextMeshProUGUI> m_SelectTexts;
        [SerializeField] private List<TextMeshProUGUI> m_ReturnTexts;

        #region Level 1
        public void StartShipSelection(int _playerIndex)
        {
            // set the current user as active to be accounted for  when the count down time comes
            m_PlayerSelections[_playerIndex].Active = true;

            m_PlayerSelections[_playerIndex].ShowCover(false);
            MenuController.i.SwitchInput(MainMenuAnimations.AnimationType.PlayerSelectionProcess, _playerIndex);
            MainMenuManager.i.PlayersDisplayer.DisplayShip(0, _playerIndex);
            MainMenuAnimations.i.StopCountDown();
        }

        public void ReturnToModes(int _playerIndex)
        {
            // hide the selected ships, so that they don't appear when the player selection menu disappears
            HideActiveShips();
            // play the animation
            MainMenuAnimations.i.ReturnToModesMenu();
            // switch the input
            MenuController.i.SwitchInput(MainMenuAnimations.AnimationType.Modes, _playerIndex);
            // close the covers if any of them was opened and reset the characters indices
            m_PlayerSelections.ForEach(p => p.ShowCover(true));
        }
        #endregion

        #region Level 2
        public void SelectShip(int _playerIndex)
        {
            m_PlayerSelections[_playerIndex].Ready = true;
            // play some effects
            m_PlayerSelections[_playerIndex].ShowReadiness(true);
            // move the player to a final state
            MenuController.i.SwitchInput(MainMenuAnimations.AnimationType.PlayerConfirmationSelection, _playerIndex);

            // check if the count down should start
            // if it's one player
            if (m_PlayerSelections.FindAll(p => p.Active).Count == 1)
            {
                // play animation
                MainMenuAnimations.i.StartCountDown();
            }
            // if it's two players
            else
            {
                if (m_PlayerSelections.TrueForAll(p => p.Ready))
                {
                    // play animation
                    MainMenuAnimations.i.StartCountDown();
                    MainMenuManager.i.PlayerShipsData.PlayersCount = 2;
                }
            }
        }

        public void MoveSelection(Vector2 _values, int _playerIndex)
        {
            if (_values.x > 0)
            {
                MainMenuManager.i.PlayersDisplayer.DisplayShip(1, _playerIndex);
            }
            else if (_values.x < 0)
            {
                MainMenuManager.i.PlayersDisplayer.DisplayShip(-1, _playerIndex);
            }
        }

        public void ReturnFromSelectionProcess(int _playerIndex)
        {
            m_PlayerSelections[_playerIndex].ShowCover(true);
            MenuController.i.SwitchInput(MainMenuAnimations.AnimationType.PlayerSelectionWindow, _playerIndex);
            // set the current user as inactive to be accounted for  when the count down time comes
            m_PlayerSelections[_playerIndex].Active = false;
        }
        #endregion

        #region Level 3
        public void ReturnToSelectionProcess(int _playerIndex)
        {
            MenuController.i.SwitchInput(MainMenuAnimations.AnimationType.PlayerSelectionProcess, _playerIndex);
            m_PlayerSelections[_playerIndex].ShowReadiness(false);
            // stop the countdown if we return
            MainMenuAnimations.i.StopCountDown();
            // remove readiness
            m_PlayerSelections[_playerIndex].Ready = false;
        }
        #endregion

        public void HideActiveShips()
        {
            for (int i = 0; i < m_PlayerSelections.Count; i++)
            {
                MainMenuManager.i.PlayerShipsData.Graphics[i].Graphics[m_PlayerSelections[i].Index].SetActive(false);
            }
        }


        public void SetActionTexts(string _selectKey, string _returnKey, int _playerIndex)
        {
            m_SelectTexts[_playerIndex].text = $"Select - {_selectKey}";
            m_ReturnTexts[_playerIndex].text = $"Return - {_returnKey}";
        }
    }
}