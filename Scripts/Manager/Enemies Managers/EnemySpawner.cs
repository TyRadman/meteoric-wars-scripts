using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    public class EnemySpawner : Singlton<EnemySpawner>
    {
        [System.Serializable]
        public class ShipRankCount
        {
            public ShipRank Rank;
            public Vector2Int NumberRange;
            public int SelectedNumber;
        }

        private ShipBuilder m_ShipBuilder;
        [SerializeField] private List<ShipSetRequest> m_ShipRequests;
        [SerializeField] private Transform[] m_SpawningPoints;
        [SerializeField] private List<ShipReference> m_ShipReferences = new List<ShipReference>();
        [SerializeField] private Vector2 m_SpawningHeightRange;
        private List<CreatedEnemyReference> m_CreatedEnemies = new List<CreatedEnemyReference>();
        private Transform m_EnemiesParent;
        [SerializeField] private List<ShipRankCount> m_ShipRanksNumberRanges;

        protected override void Awake()
        {
            base.Awake();
            m_ShipBuilder = FindObjectOfType<ShipBuilder>();
            m_EnemiesParent = new GameObject("Enemies").transform;

            // set the references numbers to different ranks
            for (int i = 0; i < 5; i++)
            {
                ShipRank rank = (ShipRank)i;
                int typeNumber = m_ShipRanksNumberRanges.Find(r => r.Rank == rank).NumberRange.RandomValue();
                m_ShipRanksNumberRanges.Find(r => r.Rank == rank).SelectedNumber = typeNumber;
            }
        }

        public GameObject GetShipPrefab(ShipRank _rank, int _colorType = 0)
        {
            return m_ShipReferences.Find(s => s.Rank == _rank && s.ColorNumber == _colorType).ShipPrefab;
        }

        public void AddPrefab(GameObject _ship, ShipRank _rank)
        {
            int typeNumber = m_ShipReferences.FindAll(s => s.Rank == _rank).Count;
            m_ShipReferences.Add(new ShipReference { Rank = _rank, ShipPrefab = _ship, TypeNumber = typeNumber });
            _ship.SetActive(false);
        }

        #region Editor Functions
        public void ClearPrefabs()
        {
            m_ShipReferences.Clear();
        }

        // editor only (not included in the main game)
        public void SpawnOnDifficulty()
        {
            // FindObjectOfType<LevelsManager>().SendDifficultyToEnemySpawner();
        }
        #endregion

        #region Waves Functionality
        // levels are gonna be easy, medium and hard. Easy levels will always have one of a kind of each rank. Medium will have a chance of getting more than one kind (this applies to the enemies creation and the waves creation.) While it's mandatory to have multiple types in hard levels.
        /// <summary>
        /// Creates the ship references and puts them in the waves provided, then returns the set of ships in waves. This can be optimized later to have enemy pools
        /// </summary>
        /// <param name="_enemyWaves"></param>
        /// <returns></returns>
        public List<EnemyWave> CreateShipsReferences(LevelEnemySet _enemyWaves, float _offensive, float _defensive, int _colorTypeNumber)
        {
            // 5 is a constant for the type of enemies there are
            // for all difficulties we create a ship of every type
            for (int k = 0; k < _colorTypeNumber; k++)
            {
                if (k == 1) m_ShipBuilder.SetNewColorPalette();

                for (int i = 0; i < 5; i++)
                {
                    ShipRank rank = (ShipRank)i;
                    int referencesNumber = m_ShipRanksNumberRanges.Find(r => r.Rank == rank).SelectedNumber;

                    for (int j = 0; j < referencesNumber; j++)
                    {
                        // create a ship prefab of every type
                        GameObject ship = m_ShipBuilder.BuildShip(rank, _offensive, _defensive);

                        float health = ship.GetComponent<EnemyHealth>().GetMaxHealth();
                        int typeNumber = m_ShipReferences.FindAll(s => s.Rank == rank).Count;
                        m_ShipReferences.Add(new ShipReference { Rank = rank, ShipPrefab = ship, TypeNumber = typeNumber, Health = health, ColorNumber = k });
                        ship.SetActive(false);
                    }
                }
            }

            // make copies of the ships based on the waves
            for (int i = 0; i < _enemyWaves.EnemyWaves.Count; i++)
            {
                for (int j = 0; j < _enemyWaves.EnemyWaves[i].EnemyPushes.Count; j++)
                {
                    for (int x = 0; x < _enemyWaves.EnemyWaves[i].EnemyPushes[j].Enemies.Count; x++)
                    {
                        SingleEnemyPush enemy = _enemyWaves.EnemyWaves[i].EnemyPushes[j].Enemies[x];
                        GameObject createdShip = BuildShip(enemy.Rank, enemy.TypeNumber);
                        createdShip.GetComponent<EnemyComponents>().Rank = enemy.Rank;
                        createdShip.name += $"({enemy.TypeNumber})";

                        // if there is no set that contains this enemy then create it and add the enemy afterwards
                        if (!m_CreatedEnemies.Exists(e => enemy.Rank == e.Rank))
                        {
                            m_CreatedEnemies.Add(new CreatedEnemyReference
                            { Rank = enemy.Rank, Ships = new List<GameObject>() });
                        }

                        // add the created enemy ship
                        m_CreatedEnemies.Find(e => enemy.Rank == e.Rank).Ships.Add(createdShip);
                    }
                }
            }

            return _enemyWaves.EnemyWaves;
        }

        public GameObject BuildShip(ShipRank _rank, int _typeNumber)
        {
            ShipReference selectedShip = m_ShipReferences.Find(s => s.Rank == _rank && s.TypeNumber == _typeNumber);

            if (selectedShip.ShipPrefab == null)
            {
                Debug.LogError($"Ship with rank {_rank} and type number {_typeNumber} doesn't exist");
                Debug.Break();
            }

            // to be cached in a list
            string name = selectedShip.ShipPrefab.name;
            var createdShip = Instantiate(selectedShip.ShipPrefab, m_EnemiesParent);
            createdShip.name = name + $" ({_rank})";
            // the created ship is weaponized
            createdShip.GetComponent<Shooter>().SetUpWeapon(WeaponsGenerator.i.Weapons.FindAll(w => w.Rank == _rank).RandomItem().Weapon);
            float health = m_ShipReferences.Find(s => s.Rank == _rank && s.TypeNumber == _typeNumber).Health;
            createdShip.GetComponent<EnemyHealth>().SetMaxAndCurrentHealth(health, health);

            return createdShip;
        }

        public List<EnemyComponents> GetShipsFromPush(EnemyPush _push)
        {
            List<EnemyComponents> enemies = new List<EnemyComponents>();

            for (int i = 0; i < _push.Enemies.Count; i++)
            {
                ShipRank rank = _push.Enemies[i].Rank;
                var enemyship = m_CreatedEnemies.Find(s => s.Rank == rank).Ships[0].GetComponent<EnemyComponents>();
                enemies.Add(enemyship);
                m_CreatedEnemies.Find(s => s.Rank == rank).Ships.Remove(enemyship.gameObject);
            }

            return enemies;
        }

        public List<float> GetPushDelays(EnemyPush _push)
        {
            List<float> delays = new List<float>();

            for (int i = 0; i < _push.Enemies.Count; i++)
            {
                delays.Add(_push.Enemies[i].InitialDelay);
            }

            return delays;
        }

        public void ShipRankAddtions(ShipRank _rank, EnemyComponents _ship)
        {
            switch (_rank)
            {
                case ShipRank.Summoner:
                    {
                        break;
                    }
            }
        }

        public int GetShipRankTypeRandomNumber(ShipRank _rank)
        {
            int number = Random.Range(0, m_ShipRanksNumberRanges.Find(r => r.Rank == _rank).SelectedNumber);
            return number;
        }
        #endregion
    }
    // structs section
    [System.Serializable]
    public struct ShipSetRequest
    {
        public ShipRank ShipRank;
        public int NumberOfShips;
    }

    [System.Serializable]
    public class ShipReference
    {
        public ShipRank Rank;
        public GameObject ShipPrefab;
        public int TypeNumber;
        public int ColorNumber = 0;
        public string Name;
        public float Health;
    }

    [System.Serializable]
    public struct CreatedEnemyReference
    {
        public ShipRank Rank;
        public List<GameObject> Ships;
    }

    [System.Serializable]
    public struct LevelEnemySet
    {
        public string Name;
        public int LevelNumber;
        public List<EnemyWave> EnemyWaves;
    }
}