using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    public class PlayerWeapons : MonoBehaviour
    {
        private Shooter m_Shooter;
        private LaserShooter m_LaserShooter;
        private PlayerShooting m_PlayerShooting;
        private PlayerShipWeaponInfo m_WeaponInfo;
        private int m_CurrentWeaponIndex = -1;

        private void Awake()
        {
            m_Shooter = GetComponent<Shooter>();
            m_LaserShooter = GetComponent<LaserShooter>();
            m_PlayerShooting = GetComponent<PlayerShooting>();
            m_WeaponInfo = GetComponent<PlayerShipWeaponInfo>();
        }

        private void Start()
        {
            SwitchWeapon();
        }

        public void SwitchWeapon()
        {
            if (m_WeaponInfo.Weapons.Count == 0)
            {
                return;
            }

            m_CurrentWeaponIndex++;

            if (m_CurrentWeaponIndex > m_WeaponInfo.Weapons.Count - 1)
            {
                m_CurrentWeaponIndex = 0;
            }

            if (m_WeaponInfo.Weapons[m_CurrentWeaponIndex].TheWeaponTag == WeaponTag.Bullets)
            {
                m_Shooter.SetUpWeapon(m_WeaponInfo.Weapons[m_CurrentWeaponIndex] as BulletWeapon);
                m_PlayerShooting.WeaponType = WeaponTag.Bullets;
            }
            else if (m_WeaponInfo.Weapons[m_CurrentWeaponIndex].TheWeaponTag == WeaponTag.Rockets)
            {
                m_Shooter.SetUpWeapon(m_WeaponInfo.Weapons[m_CurrentWeaponIndex] as BulletWeapon);
                m_PlayerShooting.WeaponType = WeaponTag.Rockets;
            }
            else if (m_WeaponInfo.Weapons[m_CurrentWeaponIndex].TheWeaponTag == WeaponTag.Laser)
            {
                m_LaserShooter.SetWeapon(m_WeaponInfo.Weapons[m_CurrentWeaponIndex] as LaserWeapon);
                m_PlayerShooting.WeaponType = WeaponTag.Laser;
            }
        }
    }
}