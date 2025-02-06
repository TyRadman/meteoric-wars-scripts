using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    public class ASReflectingCrystals : Ability
    {
        [System.Serializable]
        public struct ASReflectingCrystalsLevels
        {
            public int NumberOfCrystals;
            public float Damage;
        }

        private int m_ShotsNumber;
        [SerializeField] private ASReflectingCrystalsLevels[] m_Levels;
        [Header("Special References")]
        [SerializeField] private GameObject m_BulletPrefab;
        [SerializeField] private int m_MaxBulletsNumber = 12;
        [SerializeField] private float m_Angle;
        [SerializeField] private float m_BulletSpeed = 20f;
        private float m_Damage;
        private ReflectingBullet[] m_Bullets;

        public override void SetLevelValues(int _levelNumber)
        {
            base.SetLevelValues(_levelNumber);

            ASReflectingCrystalsLevels level = m_Levels[_levelNumber];
            m_Damage = level.Damage;
            m_ShotsNumber = level.NumberOfCrystals;
        }

        public override void SetUp(Transform _ship = null)
        {
            m_Bullets = new ReflectingBullet[m_MaxBulletsNumber];

            for (int i = 0; i < m_MaxBulletsNumber; i++)
            {
                var bullet = Instantiate(m_BulletPrefab, _ship).GetComponent<ReflectingBullet>();
                m_Bullets[i] = bullet;
                bullet.DisableBullet();
            }

            base.SetUp(_ship);
        }

        public override void Activate()
        {
            base.Activate();

            if (!IsAvailable)
            {
                return;
            }

            IsAvailable = false;
            float angle = m_Angle * 1.5f / (m_ShotsNumber * 2);
            float multiplier = -1;

            // set the angles of the bullets
            for (int i = 0; i < m_ShotsNumber; i++)
            {
                m_Bullets[i].transform.localPosition = Vector2.zero;
                var bulletAngle = m_Bullets[i].transform.eulerAngles;
                bulletAngle.z = (m_Angle - i * angle) * multiplier;
                multiplier *= -1;
                m_Bullets[i].transform.eulerAngles = bulletAngle;
                m_Bullets[i].BulletSprite.enabled = true;
                m_Bullets[i].SetUp(m_BulletSpeed, BulletUser.Enemy, m_Damage, 0);
            }

            Slot.StartAbilityUsageCountdown(LeastUsageTime);
            Invoke(nameof(RechargeAbility), LeastUsageTime);
        }
    }
}