using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    public class CollectableSpawner : Singlton<CollectableSpawner>
    {
        // TIPS: The info of the collectables are stored in the enemies' values, not in the this class
        [System.Serializable]
        public class CollectableInfo
        {
            public CollectableTag Tag;
            public float Value;
            public float Chance;
            public float Multiplier;
            public float CurrectChance;

            public void SetUp(ShipRankValues _value, float _multiplier, CollectableTag _tag)
            {
                Tag = _tag;
                var values = _value.CollectableInfo.Find(c => c.Tag == _tag);
                Value = values.Value;
                Chance = values.DropChance;
                Multiplier = Value * _multiplier;
                CurrectChance = Chance;
            }
        }

        [System.Serializable]
        public class CollectablesHolder
        {
            public List<CollectableInfo> Collectables = new List<CollectableInfo>();
        }

        [SerializeField] private Vector2 m_CollectablesSizeRange;
        [SerializeField] private Vector2 m_CollectablesSpeedRange;
        [SerializeField] private float m_ThresholdMultiplier;
        [Range(0.01f, 0.5f)] [SerializeField] private float m_FailedSpawnChanceIncrement = 0.05f;
        [Tooltip("The chance of dropping health when the player's HP is 1 point.")]
        [Range(0f, 1f)] [SerializeField] private float m_HighestHealthDropChance = 0.7f;
        [Header("Shooter Points")]
        [SerializeField] private Vector2Int m_ShooterPointsRange;
        private List<CollectablesHolder> m_Collectables = new List<CollectablesHolder>();
        private const float COLLECTABLE_SPAWNING_MOVEMENT_SPEED = 0.4f;

        private void Start()
        {
            var values = GameManager.i.GeneralValues.RankValues;

            // create collectable info for each type of enemy ships
            int collectablesNumber = System.Enum.GetNames(typeof(CollectableTag)).Length;

            for (int i = 0; i < collectablesNumber; i++)
            {
                m_Collectables.Add(new CollectablesHolder());
                CollectableTag tag = (CollectableTag)i;
                var collectables = m_Collectables[m_Collectables.Count - 1].Collectables;
                collectables.Clear();

                for (int j = 0; j < values.Count; j++)
                {
                    collectables.Add(new CollectableInfo());
                    collectables[j].SetUp(values[j], m_ThresholdMultiplier, tag);
                }
            }
        }

        public void SpawnCollectable(Vector2 _position, ShipRank _rank, float _area)
        {
            CollectableInfo collects;

            // first, shooter points are spawned because health points are more valuable and less useful at start
            if (Random.value > m_HighestHealthDropChance * (1 - GameManager.i.PlayersManager.GetPlayersHealth()))
            {
                collects = m_Collectables[(int)CollectableTag.ShooterPoints].Collectables[(int)_rank];
            }
            else
            {
                collects = m_Collectables[(int)CollectableTag.HealthPoints].Collectables[(int)_rank];
            }

            // spawn either health points or shooter points
            SpawnSpecificCollectable(_position, collects);

            var coins = m_Collectables[(int)CollectableTag.Coins].Collectables[(int)_rank];

            // chances of dropping coins
            if (Random.value > coins.Chance)
            {
                int amount = PoolingSystem.Instance.PoolingCollectables.Find(c => c.Tag == CollectableTag.Coins).Prefab.GetAmount(coins.Value);
                int amountPerCoin = PoolingSystem.Instance.PoolingCollectables.Find(c => c.Tag == CollectableTag.Coins).Prefab.GetAmount(0);
                int count = amount / amountPerCoin;
                SpawnCollectableWithValues(_position, CollectableTag.Coins, coins.Value, count, _area);
            }
        }

        public bool SpawnSpecificCollectable(Vector2 _position, CollectableInfo _collectableInfo)
        {
            // determines whether to drop a collectable or not
            if (Random.value > _collectableInfo.CurrectChance * GameManager.i.GeneralValues.DropChanceMultiplier)
            {
                // if the object is not going to spawn then the chances of having it spawned increase
                _collectableInfo.CurrectChance += _collectableInfo.CurrectChance * m_FailedSpawnChanceIncrement;
                return false;
            }

            // if the object is going to spawn then we reset the chances
            _collectableInfo.CurrectChance = _collectableInfo.Chance;

            // add some variety and make sure it doesn't go over 1
            float t = Mathf.Clamp01(_collectableInfo.Value + Random.Range(-_collectableInfo.Multiplier, _collectableInfo.Multiplier));
            // get the collectable from the pooling system and set its position
            Collectable collectable = PoolingSystem.Instance.UseCollectable(_collectableInfo.Tag);
            collectable.gameObject.SetActive(true);
            collectable.transform.position = _position;

            // set the value in points, speed and size of the collectable based on its t value
            float size = Mathf.Lerp(m_CollectablesSizeRange.x, m_CollectablesSizeRange.y, t);
            collectable.SetValues(t, size);
            return true;
        }

        /// <summary>
        /// Spawns a collectable with specified values
        /// </summary>
        /// <param name="_position"></param>
        /// <param name="_tag"></param>
        /// <param name="_value"></param>
        /// <returns></returns>
        public bool SpawnCollectableWithValues(Vector2 _position, CollectableTag _tag, float _value, int _count = 1, float _area = 0.2f)
        {
            for (int i = 0; i < _count; i++)
            {
                Collectable collectable = PoolingSystem.Instance.UseCollectable(_tag);
                collectable.gameObject.SetActive(true);
                Vector2 endPosition = _position + Random.insideUnitCircle * _area;
                StartCoroutine(MoveCollectableToPositionProcess(_position, endPosition, collectable.transform));

                float size = 1f;

                if (collectable.SizeChange)
                {
                    collectable.transform.localScale = Vector2.one * Mathf.Lerp(m_CollectablesSizeRange.x, m_CollectablesSizeRange.y, _value);
                    size = Mathf.Lerp(m_CollectablesSizeRange.x, m_CollectablesSizeRange.y, _value);
                }

                collectable.SetValues(_value, size);
            }

            return true;
        }

        private IEnumerator MoveCollectableToPositionProcess(Vector2 _start, Vector2 _end, Transform _object)
        {
            float time = 0f;
            AnimationCurve curve = GameManager.i.GeneralValues.EntranceMovementSpeedCurve;

            while (time < COLLECTABLE_SPAWNING_MOVEMENT_SPEED)
            {
                time += Time.deltaTime;
                float t = time / COLLECTABLE_SPAWNING_MOVEMENT_SPEED;
                _object.position = Vector2.Lerp(_start, _end, curve.Evaluate(t));
                yield return null;
            }
        }
    }

    public enum CollectableTag
    {
        ShooterPoints = 0, HealthPoints = 1, Coins = 2
    }
}