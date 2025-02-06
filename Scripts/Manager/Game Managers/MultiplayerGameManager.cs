using SpaceWar;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    public class MultiplayerGameManager : Singlton<MultiplayerGameManager>
    {
        [SerializeField] private List<ShipRank> m_PlayersRanks = new List<ShipRank>() { ShipRank.Minion, ShipRank.Minion };
        [SerializeField] private ShipRank m_StartingRank;
        [SerializeField] private ShipRank m_EndingRank;
        [SerializeField] [Range(0f, 2f)] private float m_Difficulty = 1f;
        private float m_OffensiveDifficulty;
        private float m_DefensiveDifficulty;

        private void Start()
        {
            Vector2 difficulties = SetUpDifficulty(m_Difficulty);
            m_OffensiveDifficulty = difficulties.x;
            m_DefensiveDifficulty = difficulties.y;

            // Create enemy waves, as variables
            LevelEnemySet waves = GameManager.i.WavesGenerator.GetEnemyWaves();
            // Create enemy ship references and put them in the waves
            int shipColorsNumber = GameManager.i.PlayersManager.DifferentColorPalettes ? 2 : 1;
            EnemySpawner.i.CreateShipsReferences(waves, m_OffensiveDifficulty, m_DefensiveDifficulty, shipColorsNumber);
            GameManager.i.PlayersManager.SetCurrentRank(m_StartingRank);

            for (int i = 0; i < 2; i++)
            {
                GameManager.i.PlayersManager.CreateRandomPlayerCharacter(i, m_StartingRank);
            }

            GameManager.i.PlayersManager.SetUpInputProfiles();
            // assign the onDeath method to the player ships so that they respawn with higher ranks afterwards
            GameManager.i.PlayersManager.Players.ForEach(p => p.Components.Health.OnPlayerDeathAction += OnPlayerDeath);
        }

        public Vector2 SetUpDifficulty(float _difficulty)
        {
            float offensive = Mathf.Clamp01(Mathf.Lerp(0, _difficulty, Random.value));
            float defensive = Mathf.Clamp01(Mathf.Lerp(0, _difficulty - offensive, Random.value));
            float remainingDifficulty = _difficulty - offensive - defensive;

            for (int i = 0; i < 20; i++)
            {
                if (remainingDifficulty > 0)
                {
                    float offenseToAdd = Mathf.Clamp(remainingDifficulty / 2f, 0f, 1f - offensive);
                    offensive += offenseToAdd;
                    float defenseToAdd = Mathf.Clamp((remainingDifficulty / 2f) + ((remainingDifficulty / 2) - offenseToAdd), 0f, 1f - defensive);
                    defensive += defenseToAdd;
                    //print($"Offense to add: {offenseToAdd}, Defense to add: {defenseToAdd}. Off + Def = {m_OffensiveDifficulty + m_DefensiveDifficulty}");
                    remainingDifficulty = _difficulty - offensive - defensive;
                }
            }

            return new Vector2(offensive, defensive);
        }

        private void OnPlayerDeath(int _playerIndex, string _message)
        {
            print(_message.Color(Color.green));

            // if this is the last rank allowed then we return
            if(m_PlayersRanks[_playerIndex] == m_EndingRank)
            {
                return;
            }

            StartCoroutine(SpawnPlayer(_playerIndex));
        }

        private IEnumerator SpawnPlayer(int _playerIndex)
        {
            // increment the index of the player's ship
            m_PlayersRanks[_playerIndex]++;

            // create the player's next ship
            PlayerComponents player = GameManager.i.PlayersManager.CreateRandomPlayerCharacter(_playerIndex, m_PlayersRanks[_playerIndex]);
            player.transform.position = GameManager.i.PlayersManager.Players[_playerIndex].RespawnStartPosition;
            
            // assign the onDeath method to the player ships so that they respawn with higher ranks afterwards
            GameManager.i.PlayersManager.Players[_playerIndex].Components.Health.OnPlayerDeathAction += OnPlayerDeath;
            yield return null;
        }
    }
}
