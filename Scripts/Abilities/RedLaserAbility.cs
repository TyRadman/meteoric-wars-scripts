using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    public class RedLaserAbility : Ability
    {
        [Header("Special Variables")]
        [SerializeField] private LaserWeapon m_Weapon;
        [SerializeField] private LaserShooter m_Shooter;

        public override void SetUp(Transform _ship = null)
        {
            base.SetUp(_ship);
            // should not be like that, if so, then why?
            var shooters = Helper.ArrayToList(_ship.GetComponents<LaserShooter>());

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

            if (HasSlot)
            {
                IsAvailable = false;
                AddAmount(-1f);

                if (CurrentAmount > 0)
                {
                    Slot.StartAbilityUsageCountdown(m_Weapon.ShotDuration);
                    Invoke(nameof(RechargeAbility), m_Weapon.ShotDuration);
                }
            }

            m_Shooter.Shoot();
        }

        public LaserWeapon GetWeapon()
        {
            return m_Weapon;
        }

        public void SetLaserDuration(float _duration)
        {
            if (m_Weapon.ShotDuration != _duration)
            {
                LaserWeapon newWeapon = ScriptableObject.CreateInstance<LaserWeapon>();
                m_Weapon.CopyWeaponValues(newWeapon);
                m_Weapon = newWeapon;
            }

            m_Weapon.ShotDuration = _duration;
            m_Shooter.CurrentWeapon = m_Weapon;
        }

        public override void ForceStop()
        {
            base.ForceStop();
            m_Shooter.StopShooting();
        }
    }
}