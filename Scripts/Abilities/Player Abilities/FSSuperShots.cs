using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    public class FSSuperShots : Ability
    {
        [System.Serializable]
        public struct BPSuperShots
        {
            public int ShotsNumber;
            public float DamagePerBullet;
        }

        [SerializeField] private List<BPSuperShots> m_Levels;
        [SerializeField] private BulletWeapon m_Weapon;
        private Shooter m_Shooter;
        private GameObject m_ShooterHolder;
        private float m_ShootingTime;

        public override void SetUp(Transform _ship = null)
        {
            base.SetUp(_ship);
            m_ShooterHolder = new GameObject("Super Shots Holder");
            m_ShooterHolder.transform.parent = _ship;
            m_Shooter = m_ShooterHolder.AddComponent<Shooter>();
            m_Shooter.SetUpWeapon(m_Weapon);

            // setting up the shooter's references
            var playerShootingPoints = _ship.GetComponent<Shooter>().ShootingPoints;
            m_Shooter.Effects = _ship.GetComponent<Shooter>().Effects;
            m_Shooter.UserTag = _ship.GetComponent<Shooter>().UserTag;
            m_Shooter.UserIndex = _ship.GetComponent<Shooter>().UserIndex;

            for (int i = 0; i < playerShootingPoints.Count; i++)
            {
                m_Shooter.ShootingPoints.Add(playerShootingPoints[i]);
            }
        }

        public override void SetLevelValues(int _levelNumber)
        {
            base.SetLevelValues(_levelNumber);
            var values = m_Weapon.UpdatedShotsVariables;

            m_Weapon.UpdatedShotsVariables = new UpdatedShots
            {
                AngleRange = values.AngleRange,
                HalfLoop = values.HalfLoop,
                ShotsNumber = m_Levels[_levelNumber].ShotsNumber,
                CoolDownTimeBetweenShots = values.CoolDownTimeBetweenShots
            };

            m_Weapon.DamagePerShot = m_Levels[_levelNumber].DamagePerBullet;
            m_ShootingTime = m_Weapon.UpdatedShotsVariables.ShotsNumber * m_Weapon.UpdatedShotsVariables.CoolDownTimeBetweenShots;
        }

        public override void Activate()
        {
            base.Activate();

            if (!IsAvailable)
            {
                return;
            }

            IsAvailable = false;
            m_Shooter.Shoot();
            Slot.StartAbilityUsageCountdown(m_ShootingTime);
            Invoke(nameof(RechargeAbility), m_ShootingTime);
        }
    }
}