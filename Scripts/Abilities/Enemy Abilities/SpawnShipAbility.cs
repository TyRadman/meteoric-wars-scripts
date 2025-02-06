using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    public class SpawnShipAbility : Ability
    {
        public int MaxShipsNumber = 5;
        [SerializeField] private ShipRank m_SummonedShipRank;
        private Vector2 m_SpawningDistanceRange = new Vector2 { x = 1, y = 2 };
        [HideInInspector] public int ShipsID;
        private int m_SpawnedCounter = 0;
        private BulletUser m_ShipsTag;
        private Transform m_ShipTransform;

        protected override void Awake()
        {
            base.Awake();
        }

        public override void SetUp(Transform _ship = null)
        {
            base.SetUp(_ship);
            m_ShipTransform = _ship;
            m_ShipsTag = _ship.GetComponent<Stats>().GetUserTag();
            CreateShips();
        }

        public override void Activate()
        {
            base.Activate();

            if (!IsAvailable || m_SpawnedCounter >= MaxShipsNumber)
            {
                return;
            }

            if (HasSlot)
            {
                IsAvailable = false;
                Slot.StartAbilityUsageCountdown(0f);
                RechargeAbility();
            }

            SpawnShip();
        }

        private void CreateShips()
        {
            // cache the ships to add a movement system to them
            int shipColor = m_ShipsTag == BulletUser.Player ? 0 : 1;
            GameObject shipToSpawn = EnemySpawner.i.GetShipPrefab(m_SummonedShipRank, shipColor);
            List<EnemyComponents> ships = ShipsPooling.i.CreateShips(MaxShipsNumber, shipToSpawn, this, m_SummonedShipRank);
            EnemyMovementGenerator.i.SetUpMovementForSummonedShips(ships);
            // make the ship choose sides
            ships.ForEach(s => s.GetComponent<EnemyStats>().SetUserTag(m_ShipsTag));
            ships.ForEach(s => s.gameObject.SetActive(false));
        }

        private void SpawnShip()
        {
            float spawningDistance = m_SpawningDistanceRange.RandomValue();
            ShipsPooling.i.SpawnShip(ShipsID, m_ShipTransform, spawningDistance);
            m_SpawnedCounter++;
        }

        public void ShipDestroyed()
        {
            m_SpawnedCounter--;
        }

        public void SetSpawnedShipsRank(ShipRank _rank)
        {
            m_SummonedShipRank = _rank;
        }
    }
}