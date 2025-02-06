using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    public class CRRockets : Ability
    {
        [System.Serializable]
        public struct CRRocketsLevel
        {
            public int NumberOfRocketsWaves;
            public float DamageArea;
        }

        [SerializeField] private List<CRRocketsLevel> m_Levels;
        [SerializeField] private BulletWeapon m_Weapon;
        [SerializeField] private float m_TimeBetweenWaves = 0.2f;
        private WaitForSeconds m_Wait;
        private Shooter m_Shooter;
        private GameObject m_ShooterHolder;
        private int NumberOfRocketWaves;

        protected override void Awake()
        {
            base.Awake();

            m_Wait = new WaitForSeconds(m_TimeBetweenWaves);
        }

        public override void SetUp(Transform _ship = null)
        {
            base.SetUp(_ship);
            m_ShooterHolder = new GameObject("CR Rockets shooter");
            m_ShooterHolder.transform.parent = _ship;
            m_Shooter = m_ShooterHolder.AddComponent<Shooter>();
            m_Shooter.SetUpWeapon(m_Weapon);

            var playerShootingPoints = _ship.GetComponent<Shooter>().ShootingPoints;
            m_Shooter.Effects = _ship.GetComponent<Shooter>().Effects;
            m_Shooter.UserTag = _ship.GetComponent<Shooter>().UserTag;
            m_Shooter.UserIndex = _ship.GetComponent<Shooter>().UserIndex;

            for (int i = 0; i < playerShootingPoints.Count; i++)
            {
                m_Shooter.ShootingPoints.Add(playerShootingPoints[i]);
            }
        }

        public override void Activate()
        {
            base.Activate();

            if (!IsAvailable)
            {
                return;
            }

            IsAvailable = false;
            StartCoroutine(shootingProcess());
        }

        public override void SetLevelValues(int _levelNumber)
        {
            base.SetLevelValues(_levelNumber);

            m_Weapon.AreaEffect.Area = m_Levels[_levelNumber].DamageArea;
            NumberOfRocketWaves = m_Levels[_levelNumber].NumberOfRocketsWaves;
        }

        private IEnumerator shootingProcess()
        {
            Slot.StartAbilityUsageCountdown(m_TimeBetweenWaves * NumberOfRocketWaves);

            for (int i = 0; i < NumberOfRocketWaves; i++)
            {
                m_Shooter.Shoot();
                yield return m_Wait;
            }

            RechargeAbility();
        }
    }
}