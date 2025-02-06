using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar.MainMenu
{
    public class MainMenuPlayersDisplayer : MonoBehaviour
    {
        [SerializeField] private List<PlayerSelectionPanelReferences> m_PlayerSelections;
        private List<PlayerGraphicsHolderMainMenu> _graphics = new List<PlayerGraphicsHolderMainMenu>();

        public void CreatePlayerCharactersGraphics(List<PlayerShipInfo> characters)
        {
            for (int i = 0; i < 2; i++)
            {
                PlayerGraphicsHolderMainMenu playerGraphics = new PlayerGraphicsHolderMainMenu();

                for (int j = 0; j < characters.Count; j++)
                {
                    Transform startPoint = m_PlayerSelections[i].StartPoint;
                    PlayerCharacterMenuController character = Instantiate(characters[j].ShipGraphics,
                        startPoint.position, startPoint.rotation, startPoint).GetComponent<PlayerCharacterMenuController>();
                    playerGraphics.AddGraphics(character);
                }
             
                _graphics.Add(playerGraphics);
            }

        }

        public void DisplayShip(int _value, int _playerIndex)
        {
            var player = m_PlayerSelections[_playerIndex];
            // disable the previously displayed ship
            MainMenuManager.i.PlayerShipsData.Graphics[_playerIndex].Graphics[player.Index].SetActive(false);
            // change the index
            player.Index = (int)Helper.GetNextValueWithinRange(player.Index, _value, MainMenuManager.i.PlayerShipsData.ShipsInfo.Count - 1, 0);
            // show the new ship based on the new index
            MainMenuManager.i.PlayerShipsData.Graphics[_playerIndex].Graphics[player.Index].SetActive(true);
            // get the ship info to display it
            FillInformation(MainMenuManager.i.PlayerShipsData.ShipsInfo[player.Index], player.References);
            // store data of the player
            MainMenuManager.i.PlayerShipsData.StorePlayerData(_playerIndex, m_PlayerSelections[_playerIndex].Index);
        }

        private void FillInformation(PlayerShipInfo _info, ShipUIReferencesMainMenu _ui)
        {
            _ui.ShipName.text = _info.Name;
            _ui.ShipDescription.text = _info.Description;
            _ui.HealthBar.fillAmount = _info.MaxHealth / GameManager.i.GeneralValues.MaxHealth;
            _ui.SpeedBar.fillAmount = _info.MovementSpeed / GameManager.i.GeneralValues.MaxSpeed;
            _ui.DamageBar.fillAmount = _info.Damage / GameManager.i.GeneralValues.MaxDamagePerSecond;
        }
    }
}
