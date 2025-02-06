using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    public class FSSuperLaser : Ability
    {
        [System.Serializable]
        public struct FSSuperLaserValues
        {
            public float DamagePerSecond;
        }

        [SerializeField] private FSSuperLaserValues[] m_Levels;

        [Header("Special Variables")]
        [SerializeField] private LaserWeapon m_Weapon;
        [SerializeField] private LaserShooter m_Shooter;

        public override void SetLevelValues(int _levelNumber)
        {
            base.SetLevelValues(_levelNumber);
            m_Weapon.DamagePerSecond = m_Levels[_levelNumber].DamagePerSecond;
        }

        public override void SetUp(Transform _ship = null)
        {
            base.SetUp(_ship);
            // should not be like that, if so, then why?
            List<LaserShooter> shooters = Helper.ArrayToList(_ship.GetComponents<LaserShooter>());

            if (shooters.Exists(s => s.TheShooterType == LaserShooter.ShooterType.Ability))
            {
                m_Shooter = shooters.Find(s => s.TheShooterType == LaserShooter.ShooterType.Ability);
            }
            else
            {
                m_Shooter = _ship.gameObject.AddComponent<LaserShooter>();
                m_Shooter.UserTag = _ship.GetComponent<Stats>().GetUserTag();
            }

            m_Shooter.SetShooter(_ship.GetComponent<Shooter>());
            m_Shooter.SetUpShootingPoints(_ship);
            m_Shooter.SetWeapon(m_Weapon);

            if (HasSlot)
            {
                Slot.SetAmount(CurrentAmount.ToString());
            }
        }

        public override void Activate()
        {
            base.Activate();

            if (!IsAvailable)
            {
                return;
            }

            StartCoroutine(AbilityActivationProcess());
        }

        protected override IEnumerator AbilityActivationProcess()
        {
            float shotDuration = m_Weapon.ShotDuration;
            IsAvailable = false;

            Slot.StartAbilityUsageCountdown(m_Weapon.ShotDuration);
            m_Shooter.Shoot();

            yield return new WaitForSeconds(shotDuration);

            RechargeAbility();
        }

        public override void ForceStop()
        {
            base.ForceStop();
            StopAllCoroutines();
            m_Shooter.StopShooting();
        }

        public LaserWeapon GetWeapon()
        {
            return m_Weapon;
        }
    }
}