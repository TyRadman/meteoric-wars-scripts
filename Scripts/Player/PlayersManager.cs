using SpaceWar.UI.HUD;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpaceWar.Audios;

namespace SpaceWar
{
    public class PlayersManager : MonoBehaviour
    {
        [System.Serializable]
        public class AbilityUIElements
        {
            public List<PlayerAbilityUIElement> Elements;
        }

        public enum ShipType
        {
            Premade, Random
        }

        public ShipType TheShipType;
        public int NumberOfPlayers = 0;
        public KeysSet[] Keys;
        public KeysSet[] DefaultKeys;
        public Transform[] m_SpawningPoints;
        private PlayerShipsData m_PlayersData;
        public List<PlayerProfile> Players = new List<PlayerProfile>();
        public int LivesCount = 3;
        public List<Transform> PlayerAllies = new List<Transform>();
        public List<AbilityUIElements> AbilitySlots;
        [Header("Random")]
        [SerializeField] private ShipRank m_CurrentRank;
        [Range(0f, 2f)] [SerializeField] private float m_Difficulty = 1f;
        [SerializeField] private float m_SpeedMultiplier = 10f;
        [SerializeField] private Audio m_ShootingAudio;
        public bool DifferentColorPalettes = false;
        private const float RESPAWN_DISTANCE = 4f;

        public void SetUp(PlayerShipsData data)
        {
            m_PlayersData = data;
            CreateCharacters();
        }

        public PlayerComponents GetPlayer(int playerIndex)
        {
            return Players[playerIndex].Components;
        }

        public void CreateCharacters()
        {
            if (TheShipType == ShipType.Premade)
            {
                CreatePremadePlayers(m_PlayersData);
            }
            else
            {
                CreateRandomPlayers(m_PlayersData.PlayersCount);
            }

            // very temporary
            SniperMachineGunWeapon.PlayerRotation = Players[0].PlayerTransform.localEulerAngles;
        }

        public void SetUpInputProfiles()
        {
            //GameManager.i.InputController.SetUpProfiles(Players.Count);
        }

        public PlayerComponents CreateRandomPlayerCharacter(int _playerIndex, ShipRank _rank)
        {
            PlayerComponents player = CreateRadnomPlayer(_playerIndex, _rank);
            // very temporary
            SniperMachineGunWeapon.PlayerRotation = Players[0].PlayerTransform.localEulerAngles;
            return player;
        }

        #region Premade Players
        private void CreatePremadePlayers(PlayerShipsData _data)
        {
            m_PlayersData = _data;
            NumberOfPlayers = m_PlayersData.PlayersCount;

            // disable unused HUD

            for (int i = 0; i < m_PlayersData.PlayersCount; i++)
            {
                GameManager.i.HUDManager.EnablePlayerHUD(i, true);
                CreatePlayer(i);
            }

            KeyBindingManager.i.StartKeyBinding();
            // very temporary
            SniperMachineGunWeapon.PlayerRotation = Players[0].PlayerTransform.localEulerAngles;
        }

        private void CreatePlayer(int _index)
        {
            PlayerProfile playerProfile = new PlayerProfile();
            Players.Add(playerProfile);
            // select the player ship info profile (the SO)
            PlayerShipInfo shipInfo = m_PlayersData.ShipsInfo[m_PlayersData.PlayersCharacters[_index]];

            GameObject player = Instantiate(shipInfo.ShipPrefab, m_SpawningPoints[_index].position, Quaternion.identity);
            player.name = "Player " + _index;

            PlayerStats stats = player.GetComponent<PlayerStats>();
            PlayerComponents components = player.GetComponent<PlayerComponents>();

            components.PlayerIndex = _index;
            components.SetUp(null);

            stats.SetStartingLives(LivesCount);

            // set the player's max health
            stats.SetDamage(shipInfo.Damage);

            playerProfile.PlayerTransform = player.transform;
            // set up the input
            player.GetComponent<PlayerShipInput>().SetUpButtons(Keys[_index]);
            playerProfile.WeaponInfo = player.GetComponent<PlayerShipWeaponInfo>();
            playerProfile.Components = components;

            //  set up
            playerProfile.Components.SetUp(null);
            playerProfile.Components.Activate();
            // creates a list of abilities that are mixed between the player's special abilities and the common abilities
            AbilitiesManager.i.SetUpPlayerAbilitiesPrefabs(shipInfo);
            AbilitySlots[_index].Elements.ForEach(e => e.SetUp());
        }
        #endregion

        #region External
        public void UpgradePlayerShooting(int _level, int _playerIndex)
        {
            if (TheShipType == ShipType.Random) return;

            if (Players[_playerIndex].Components.ThePlayerShooting.WeaponType != WeaponTag.Custom)
            {
                Players[_playerIndex].Components.ShipShooter.SetUpWeapon((BulletWeapon)Players[_playerIndex].WeaponInfo.Weapons[_level]);
            }
            else
            {
                Weapon weapon;

                // if the ship has no weapons in its weapon info script then feed the method null for a weapon
                if (Players[_playerIndex].WeaponInfo.Weapons.Count == 0)
                {
                    weapon = null;
                }
                else
                {
                    weapon = Players[_playerIndex].WeaponInfo.Weapons[_level];
                }

                Players[_playerIndex].Components.ThePlayerShooting.CustomShooter.Upgrade(weapon, _level);
            }
        }

        public Vector2 GetClosestPlayerToTransform(Vector2 _startPoint)
        {
            List<Transform> positions = new List<Transform>();

            for (int i = 0; i < Players.Count; i++)
            {
                if (Players[i].Components.IsAlive) positions.Add(Players[i].PlayerTransform);
            }

            for (int i = 0; i < PlayerAllies.Count; i++)
            {
                positions.Add(PlayerAllies[i]);
            }

            // if there are no targets, then return a random point
            if (positions.Count == 0) return LevelDimensions.Instance.GetRandomPointDownwards();

            // if there is only one target, then just return it without having to cache anything
            if (positions.Count == 1) return Players[0].PlayerTransform.position;

            int selectedIndex = -1;
            float minDistance = float.MaxValue;

            for (int i = 0; i < positions.Count; i++)
            {
                float currentDistance = Vector2.Distance(_startPoint, positions[i].position);

                if (currentDistance < minDistance)
                {
                    selectedIndex = i;
                    minDistance = currentDistance;
                }
            }

            return positions[selectedIndex].position;
        }

        public float GetPlayersHealth()
        {
            float health = 0f;

            for (int i = 0; i < Players.Count; i++)
            {
                health += Players[i].Components.Health.GetHealthT();
            }

            return health / Players.Count;
        }

        public Vector2 GetRandomPlayerPosition()
        {
            return Players.RandomItem().PlayerTransform.position;
        }

        public bool AllPlayersDead()
        {
            return Players.TrueForAll(p => !p.Components.IsAlive);
        }
        #endregion

        #region Random
        public void CreateRandomPlayers(int _playersCount)
        {
            for (int i = 0; i < _playersCount; i++)
            {
                CreateRadnomPlayer(i, m_CurrentRank);
            }
        }

        private PlayerComponents CreateRadnomPlayer(int _index, ShipRank _rank)
        {
            BulletUser userTag = (BulletUser)_index;
            PlayerProfile playerProfile = new PlayerProfile();

            // if there is already a profile, then we replace it, otherwise, we make a room for it
            if (Players.Exists(p => p.PlayerTransform.CompareTag(userTag.ToString()))) Players[_index] = playerProfile;
            else Players.Add(playerProfile);

            Vector2 difficulty = GameManager.i.SetUpDifficulty(m_Difficulty);
            ShipRankValues values = GameManager.i.GeneralValues.RankValues[(int)_rank];
            GameObject player = ShipBuilder.i.BuildPlayerShip(_rank, difficulty.x, difficulty.y, _index);
            playerProfile.Components = player.GetComponent<PlayerComponents>();
            playerProfile.PlayerTransform = player.transform;

            // change side related variables
            playerProfile.PlayerTransform.tag = userTag.ToString();
            playerProfile.PlayerTransform.gameObject.layer = userTag == BulletUser.Player ? GeneralValues.PlayerLayerInt : GeneralValues.EnemyLayerInt;
            playerProfile.Components.Stats.UserTag = userTag;
            playerProfile.Components.ShipShooter.UserTag = userTag;

            if (userTag == BulletUser.Enemy)
            {
                playerProfile.PlayerTransform.eulerAngles += Vector3.forward * 180f;
                playerProfile.PlayerTransform.position = Vector2.up * LevelDimensions.Instance.HalfHeight;
            }


            player.name = $"Player {_index + 1} ({_rank}";
            playerProfile.Components.PlayerIndex = _index;
            playerProfile.Components.Stats.SetStartingLives(0);

            // set speed
            float speed = values.RandomMovementInfo.SpeedRange.Lerp(difficulty.y);
            speed *= m_SpeedMultiplier;
            player.GetComponent<PlayerMovement>().SetOriginalShipMovementSpeed(speed);
            // set up the input
            player.GetComponent<PlayerShipInput>().SetUpButtons(Keys[_index]);
            playerProfile.WeaponInfo = player.GetComponent<PlayerShipWeaponInfo>();
            playerProfile.Components.PlayerIndex = _index;

            // health
            float health = Mathf.Round(values.HealthValue.Lerp(difficulty.y));
            playerProfile.Components.Health.SetMaxHealth(health);
            playerProfile.Components.IsAlive = true;

            //  set up abilities
            playerProfile.Components.Abilities.SetUpPlayerAbilities();

            if (_rank == ShipRank.Strong || _rank == ShipRank.Summoner)
            {
                Ability ability = EnemyAbilitiesManager.i.GetAbilityPrefab(_rank);
                playerProfile.Components.Abilities.AddAbility(ability);
            }

            AbilitySlots[_index].Elements.ForEach(e => e.SetUp());
            // set the players' position
            playerProfile.Components.transform.position = m_SpawningPoints[_index].position;
            float offset = (-Mathf.Sign(playerProfile.Components.transform.position.y) * RESPAWN_DISTANCE);
            print(offset);
            playerProfile.RespawnStartPosition = (Vector2)m_SpawningPoints[_index].position - Vector2.up * offset;

            if (_index == 1) playerProfile.Components.Movement.ReverseInput(false);

            // set up a weapon
            BulletWeapon weapon = WeaponsGenerator.i.CreateWeapon(values.Rank, difficulty.x);
            playerProfile.Components.ShipShooter.SetUpWeapon(weapon, m_ShootingAudio);
            return playerProfile.Components;
        }

        public void SetCurrentRank(ShipRank _rank)
        {
            m_CurrentRank = _rank;
        }
        #endregion
    }

    [System.Serializable]
    public class PlayerProfile
    {
        public Transform PlayerTransform;
        public KeySet Keys;
        public PlayerShipWeaponInfo WeaponInfo;
        public PlayerComponents Components;
        public Vector2 RespawnStartPosition;
    }
}