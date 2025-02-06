using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    public class RandomMultiPlayerShipManager : Singlton<RandomMultiPlayerShipManager>
    {
        [SerializeField] private ShipRank m_CurrentRank;
        [SerializeField] private float m_Difficulty = 1f;

        public void CreatePlayers(int _playersCount)
        {
            for (int i = 0; i < _playersCount; i++)
            {
                CreatePlayer(i);
            }
        }

        private void CreatePlayer(int _index)
        {
            PlayerProfile playerProfile = new PlayerProfile();
            Vector2 difficulty = GameManager.i.SetUpDifficulty(m_Difficulty);
            GameObject player = ShipBuilder.i.BuildPlayerShip(m_CurrentRank, difficulty.x, difficulty.y);
            ShipRankValues values = GameManager.i.GeneralValues.RankValues[(int)m_CurrentRank];

            player.name = "Player " + _index;
            var stats = player.GetComponent<PlayerStats>();
            PlayerComponents components = player.GetComponent<PlayerComponents>();
            components.PlayerIndex = _index;
            stats.SetStartingLives(0);

            // set speed
            float speed = values.RandomMovementInfo.SpeedRange.Lerp(difficulty.y);
            player.GetComponent<PlayerMovement>().SetOriginalShipMovementSpeed(speed);
            playerProfile.PlayerTransform = player.transform;
            // set up the input
            player.GetComponent<PlayerShipInput>().SetUpButtons(GameManager.i.PlayersManager.Keys[_index]);
            playerProfile.WeaponInfo = player.GetComponent<PlayerShipWeaponInfo>();
            playerProfile.Components = player.GetComponent<PlayerComponents>();
            playerProfile.Components.IsAlive = true;

            GameManager.i.PlayersManager.AbilitySlots[_index].Elements.ForEach(e => e.SetUp());
        }
    }
}