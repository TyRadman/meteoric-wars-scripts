using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    public class GMExplodingBall : Ability
    {

        [System.Serializable]
        public struct CRRocketsLevel
        {
            [Range(0f, 1f)] public float HealthDropped;
            public float DamageArea;
        }

        [SerializeField] private List<CRRocketsLevel> m_Levels;
        [SerializeField] private BulletWeapon m_Weapon;
        private Shooter m_Shooter;
        private GameObject m_ShooterHolder;
        private float m_HealthPointsValue;

        public override void SetLevelValues(int _levelNumber)
        {
            base.SetLevelValues(_levelNumber);
            CRRocketsLevel level = m_Levels[_levelNumber];
            m_Weapon.AreaEffect.Area = level.DamageArea;
            m_HealthPointsValue = level.HealthDropped;
        }

        public override void SetUp(Transform _ship = null)
        {
            base.SetUp(_ship);
            m_ShooterHolder = new GameObject("GM Exploding Ball Shooter");
            m_ShooterHolder.transform.parent = _ship;
            m_Shooter = m_ShooterHolder.AddComponent<Shooter>();
            m_Shooter.SetUpWeapon(m_Weapon);
            m_Shooter.Effects = _ship.GetComponent<Shooter>().Effects;
            m_Shooter.UserTag = _ship.GetComponent<Shooter>().UserTag;
            m_Shooter.UserIndex = _ship.GetComponent<Shooter>().UserIndex;
            m_Shooter.ShootingPoints.Add(_ship.GetComponent<Shooter>().ShootingPoints[0]);

            // set the drop health points event to all the bullet (which is one in this particular scenraio)
            var bullets = PoolingSystem.Instance.GetBullets(m_Weapon.BulletPoolType, BulletUser.Player);

            foreach (Bullet bullet in bullets)
            {
                var eventBullet = (RocketBulletEvent)bullet;
                eventBullet.SetAction(dropHealthPoints);
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
            m_Shooter.Shoot();
            Slot.StartAbilityUsageCountdown(LeastUsageTime);
            Invoke(nameof(RechargeAbility), LeastUsageTime);
        }

        private void dropHealthPoints(Vector2 _position)
        {
            CollectableSpawner.i.SpawnCollectableWithValues(_position, CollectableTag.HealthPoints, m_HealthPointsValue);
        }
    }
}