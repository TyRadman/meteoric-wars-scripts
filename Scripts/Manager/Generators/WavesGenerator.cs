using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace SpaceWar
{
    using SpaceWar.Shop;

    public class WavesGenerator : MonoBehaviour
    {
        public enum PushesStyle
        {
            SingleType = 0, DoubleType = 1
        }

        [SerializeField] private List<ShipRankPerWaveChance> m_WavesInfo;
        [SerializeField] private LevelEnemySet m_Waves;
        [SerializeField] private float m_InitialDelayPerSpawn = 0.1f;
        [SerializeField] private LevelWavesInfo m_LevelWavesInfo;
        [field: SerializeField, Header("Custom Waves")] public bool UseCustomWaves { get; private set; } = false;
        [SerializeField] private LevelEnemySet m_CustomWaves;
        private int m_ShopShowRate = 3;

        private void Start()
        {
            m_ShopShowRate = ShopManager.i.ShopShowRate;
        }

        public LevelEnemySet GetEnemyWaves()
        {
            if (!UseCustomWaves)
            {
                CreateWavesInfo();
                CreateSetOfWaves();
                return m_Waves;
            }
            else
            {
                return m_CustomWaves;
            }
        }

        public void EnableCustomWaves(bool enable)
        {
            UseCustomWaves = enable;
        }

        private void CreateWavesInfo()
        {
            m_WavesInfo = new List<ShipRankPerWaveChance>();

            for (int i = 0; i < m_LevelWavesInfo.WavesNumber; i++)
            {
                float t = (float)i / ((float)m_LevelWavesInfo.WavesNumber - 1f);

                Vector2 pushRangeFloat = Vector2.Lerp(m_LevelWavesInfo.LeastPushRange, m_LevelWavesInfo.MostPushRange, t);
                Vector2Int pushRange = new Vector2Int((int)pushRangeFloat.x, (int)pushRangeFloat.y);

                List<ShipRank> allowedRanks = new List<ShipRank>();
                Vector2Int leastRanks = new Vector2Int((int)m_LevelWavesInfo.LeastRanksAllowed[0],
                    (int)m_LevelWavesInfo.MostRanksAllowed[0]);
                Vector2Int mostRanks = new Vector2Int((int)m_LevelWavesInfo.LeastRanksAllowed[m_LevelWavesInfo.LeastRanksAllowed.Count - 1],
                    (int)m_LevelWavesInfo.MostRanksAllowed[m_LevelWavesInfo.MostRanksAllowed.Count - 1]);
                int min = Mathf.FloorToInt(Mathf.Lerp(leastRanks.x, leastRanks.y, t));
                int max = Mathf.CeilToInt(Mathf.Lerp(mostRanks.x, mostRanks.y, t));
                Vector2Int selectedRanks = new Vector2Int(min, max);

                for (int j = selectedRanks.y; j >= selectedRanks.x; j--)
                {
                    allowedRanks.Add((ShipRank)j);
                }

                allowedRanks.Reverse();

                ShipRankPerWaveChance newInfo = new ShipRankPerWaveChance
                {
                    WaveNumber = i,
                    RanksAllowed = allowedRanks,
                    PushRange = pushRange
                };

                m_WavesInfo.Add(newInfo);
            }
        }

        // creates a set of waves
        public void CreateSetOfWaves()
        {
            m_Waves.EnemyWaves = new List<EnemyWave>();

            for (int i = 0; i < m_LevelWavesInfo.WavesNumber; i++)
            {
                m_Waves.EnemyWaves.Add(CreateWave(i));
            }
        }

        // creates a single wave
        private EnemyWave CreateWave(int _waveIndex)
        {
            if (_waveIndex > m_WavesInfo.Count - 1)
            {
                _waveIndex = m_WavesInfo.Count - 1;
            }

            // we interpolate between the first wave and last wave total values based on the wave index
            float t = _waveIndex / (float)(m_LevelWavesInfo.WavesNumber - 1);
            int waveValue = (int)Mathf.Lerp(m_LevelWavesInfo.DifficultyRange.x, m_LevelWavesInfo.DifficultyRange.y, t);

            // create a new wave and feed it the obtained value
            bool showShop = _waveIndex % m_ShopShowRate == 0;
            EnemyWave wave = new EnemyWave { EnemyPushes = CreateSetOfPushes(waveValue, _waveIndex), Value = waveValue, ShowShop = showShop };
            return wave;
        }

        // creates a set of pushes
        private List<EnemyPush> CreateSetOfPushes(int _value, int _waveIndex)
        {
            List<EnemyPush> pushes = new List<EnemyPush>();

            int pushesNumber;

            // if the current wave is greater than the last set of number of pushes then set it to the last range
            if (_waveIndex >= m_WavesInfo.Count)
            {
                pushesNumber = m_WavesInfo[m_WavesInfo.Count - 1].PushRange.RandomValue();
            }
            // otherwise, set the corresponding range to the wave pushes number
            else
            {
                pushesNumber = m_WavesInfo[_waveIndex].PushRange.RandomValue();
            }

            int valuePerPush = _value;

            if (pushesNumber > 1)
            {
                valuePerPush = (_value / pushesNumber) * 2;
            }

            for (int i = 0; i < pushesNumber; i++)
            {
                pushes.Add(CreatePush(valuePerPush, _waveIndex));
            }

            return pushes;
        }

        // problems to solve:
        // 1 - How to vary the types and to tell when to vary the types?
        // 2 - How to vary the initial spawn delay?
        private EnemyPush CreatePush(int _value, int _waveIndex)
        {
            var values = FindObjectOfType<GeneralValues>().RankValues;
            EnemyPush push = new EnemyPush { Enemies = new List<SingleEnemyPush>(), Value = _value };
            PushesStyle style = (PushesStyle)Random.Range(0, 2);
            List<ShipRank> allowedRanks = GetAllowedRanks(_value, _waveIndex, values);

            // first style
            if (style == PushesStyle.SingleType || allowedRanks.Count == 1)
            {
                CreateSingleTypePush(allowedRanks, _value, values, push);
            }
            // second style
            else if (style == PushesStyle.DoubleType)
            {
                CreateDoubleTypePush(allowedRanks, _value, values, push);
            }

            return push;
        }

        #region Push Styles Methods
        private void CreateSingleTypePush(List<ShipRank> _allowedRanks, int _value, List<ShipRankValues> _values, EnemyPush _push)
        {
            // select a random ship
            ShipRank selectedShipRank = _allowedRanks[Random.Range(0, _allowedRanks.Count)];
            int numberOfShips = _value / _values.Find(r => r.Rank == selectedShipRank).Value;
            int typeNumber = EnemySpawner.i.GetShipRankTypeRandomNumber(selectedShipRank);

            if (numberOfShips == 0)
            {
                numberOfShips = 1;
            }

            for (int i = 0; i < numberOfShips; i++)
            {
                // add a new enemy
                _push.Enemies.Add(new SingleEnemyPush
                { InitialDelay = m_InitialDelayPerSpawn * i, TypeNumber = typeNumber, Rank = selectedShipRank });
            }
        }

        private void CreateDoubleTypePush(List<ShipRank> _allowedRanks, int _value, List<ShipRankValues> _values, EnemyPush _push)
        {
            // select a random ship. We create a list of ranks just to exclude the selected type from the choices of the second type
            int firstIndex = Random.Range(0, _allowedRanks.Count);
            ShipRank firstRank = _allowedRanks[firstIndex];
            // select the ship model for this rank
            int firstTypeNumber = EnemySpawner.i.GetShipRankTypeRandomNumber(firstRank);

            // choose a random second rank
            int secondIndex = Random.Range(0, _allowedRanks.Count - 1);
            List<ShipRank> secondAllowedRanks = _allowedRanks.FindAll(r => r != firstRank);
            ShipRank secondRank = secondAllowedRanks[secondIndex];
            // get the model number of the ship
            int secondTypeNumber = EnemySpawner.i.GetShipRankTypeRandomNumber(secondRank);

            // gets the percentage of the first type
            float firstTypePart = Random.Range(0.4f, 0.6f);
            // gets the values of the first type
            int firstTypeValues = (int)(_value * firstTypePart);

            // gets the number of ships for the first type
            int numberOfShips = firstTypeValues / _values.Find(r => r.Rank == firstRank).Value;
            // caches the last initial delay
            float delay = 0f;

            if (numberOfShips == 0)
            {
                numberOfShips = 1;
            }

            // adds the ships to the list
            for (int i = 0; i < numberOfShips; i++)
            {
                delay += m_InitialDelayPerSpawn;
                // add a new enemy
                _push.Enemies.Add(new SingleEnemyPush
                { InitialDelay = delay, TypeNumber = firstTypeNumber, Rank = firstRank });
            }

            // repeat the same process for the second type of ships, but without randomizing, just taking the remaining values
            int secondTypeValues = _value - firstTypeValues;
            // gets the number of ships
            numberOfShips = secondTypeValues / _values.Find(r => r.Rank == secondRank).Value;

            if (numberOfShips == 0)
            {
                numberOfShips = 1;
            }

            // add ships
            for (int i = 0; i < numberOfShips; i++)
            {
                delay += m_InitialDelayPerSpawn;
                // add a new enemy
                _push.Enemies.Add(new SingleEnemyPush
                { InitialDelay = delay, TypeNumber = secondTypeNumber, Rank = secondRank });
            }
        }
        #endregion

        private List<ShipRank> GetAllowedRanks(int _value, int _waveIndex, List<ShipRankValues> _values)
        {
            ShipRankPerWaveChance info = m_WavesInfo[_waveIndex];

            // determine the allowed ranks
            List<ShipRank> allowedRanks = new List<ShipRank>();

            for (int i = 0; i < info.RanksAllowed.Count; i++)
            {
                if (_values.Find(s => s.Rank == info.RanksAllowed[i]).Value <= _value)
                {
                    allowedRanks.Add(info.RanksAllowed[i]);
                }
            }

            // if the value of the push is not enough to have any ships in it then pick the cheapest ship
            if (allowedRanks.Count == 0)
            {
                allowedRanks.Add(_values.FindAll(s => info.RanksAllowed.Exists(ss => ss == s.Rank))[0].Rank);
            }

            return allowedRanks;
        }
    }


    [System.Serializable]
    public struct ShipRankPerWaveChance
    {
        public int WaveNumber;
        public List<ShipRank> RanksAllowed;
        public Vector2Int PushRange;
    }

    [System.Serializable]
    public struct LevelWavesInfo
    {
        public int WavesNumber;
        public List<ShipRank> LeastRanksAllowed;
        public List<ShipRank> MostRanksAllowed;
        public Vector2Int LeastPushRange;
        public Vector2Int MostPushRange;
        public Vector2Int DifficultyRange;
    }
}