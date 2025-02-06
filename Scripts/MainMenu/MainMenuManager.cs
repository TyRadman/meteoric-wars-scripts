using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar.MainMenu
{
    public class MainMenuManager : Singlton<MainMenuManager>
    {
        [field: SerializeField] public MainMenuPlayersDisplayer PlayersDisplayer { private set; get; }
        [field: SerializeField] public PlayerShipsData PlayerShipsData { private set; get; }

        private void Start()
        {
            SetUp();
        }

        private void SetUp()
        {
            PlayersDisplayer.CreatePlayerCharactersGraphics(PlayerShipsData.ShipsInfo);
        } 
    }
}
