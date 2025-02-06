using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SpaceWar
{
    [CreateAssetMenu(fileName = "PlayersData", menuName = "SpaceWar/Players Data")]
    public class PlayerShipsData : ScriptableObject
    {
        [System.Serializable]
        public class PlayerGraphics
        {
            public List<GameObject> Graphics;
        }

        public List<PlayerShipInfo> ShipsInfo;
        public List<PlayerGraphics> Graphics = new List<PlayerGraphics>();
        [SerializeField] private Transform[] m_DisplayPoints;
        [Range(1, 2)] public int PlayersCount = 0;
        [field : SerializeField] public int[] PlayersCharacters { get; set; } = new int[2];
        [field: SerializeField] public List<PlayerInputHandler> PlayerInputHandlers { get; private set; } = new List<PlayerInputHandler>();

        public void SetUp()
        {
            for (int j = 0; j < 2; j++)
            {
                List<GameObject> shipsGraphics = new List<GameObject>();
                Graphics.Add(new PlayerGraphics() { Graphics = shipsGraphics });

                for (int i = 0; i < ShipsInfo.Count; i++)
                {
                    shipsGraphics.Add(Instantiate(ShipsInfo[i].ShipGraphics, m_DisplayPoints[j]));
                }
            }

            Graphics.ForEach(s => s.Graphics.ForEach(g => g.SetActive(false)));
        }

        public void StartCreatingPlayers()
        {
            GameManager.i.PlayersManager.CreateCharacters();
        }

        public void StorePlayerData(int _playerIndex, int _shipIndex)
        {
            PlayersCharacters[_playerIndex] = _shipIndex;
        }

        public void AddPlayerInputHandler(PlayerInputHandler playerInputHandler)
        {
            if (PlayerInputHandlers.Exists(i => i == playerInputHandler))
            {
                return;
            }

            if (PlayerInputHandlers.Exists(i => i == null))
            {
                PlayerInputHandlers.RemoveAll(i => i == null);
            }

            PlayerInputHandlers.Add(playerInputHandler);
            PlayersCount = PlayerInputHandlers.Count;
        }

        public bool HasPlayer()
        {
            return PlayerInputHandlers.Count > 0 && !PlayerInputHandlers.Exists(p => p == null);
        }
    }
}
