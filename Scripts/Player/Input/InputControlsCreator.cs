using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    public class InputControlsCreator : MonoBehaviour
    {
        [SerializeField] private PlayerShipsData _playersInfoSaver;
        [SerializeField] private PlayerInputHandler _playerPrefab;
        [SerializeField] [Range(1, 2)] public int PlayersCount = 1;

        public void SetUp()
        {
            for (int i = 0; i < PlayersCount; i++)
            {
                //_inputManager.JoinPlayer();
                PlayerInputHandler player = Instantiate(_playerPrefab, transform);
                _playersInfoSaver.AddPlayerInputHandler(player);
            }
        }
    }
}
