using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    public class DroneShooting : MonoBehaviour
    {
        [System.Serializable]
        public struct DroneWeapon
        {
            public Weapon TheWeapon;
            [Range(0f, 1f)] public float ShootingChance;
        }

        [SerializeField] private Weapon m_MainWeapon;
        [SerializeField] private List<DroneWeapon> m_SideWeapons = new List<DroneWeapon>();
        public float m_ShootingFrequency = 2f;
        [SerializeField] private bool m_ShootAutomatically = true;
        private WaitForSeconds m_ShootingWait;
        public Shooter TheShooter;
        private LaserShooter m_LaserShooter;
        private Drone m_DroneMovement;
        private bool m_CanShoot = true;
        private float m_WeaponCoolDown;

        private void Awake()
        {
            TheShooter = GetComponent<Shooter>();
            m_LaserShooter = GetComponent<LaserShooter>();
            m_DroneMovement = GetComponent<Drone>();
            m_ShootingWait = new WaitForSeconds(m_ShootingFrequency);
            m_LaserShooter.SetUpShootingPoints(TheShooter.ShootingPoints[0]);
            SetUpDroneWeapons();
        }

        private IEnumerator Start()
        {
            if (m_ShootAutomatically)
            {
                yield return m_ShootingWait;
                StartCoroutine(shootingProcess());
            }

            yield return null;
        }

        #region Set Ups
        private void SetUpDroneWeapons()
        {
            // set up the main weapon

        }

        public void SetShootingRate(float _rate, float _superShootingChance)
        {
            m_ShootingFrequency = _rate;

            for (int i = 0; i < m_SideWeapons.Count; i++)
            {
                var previousInfo = m_SideWeapons[i];
                m_SideWeapons[i] = new DroneWeapon { TheWeapon = previousInfo.TheWeapon, ShootingChance = _superShootingChance };
            }

            m_ShootingWait = new WaitForSeconds(m_ShootingFrequency);
        }

        public void SetMainWeapon(Weapon _weapon)
        {
            m_MainWeapon = _weapon;
        }

        public void RemoveAllSideWeapons()
        {
            m_SideWeapons.Clear();
            m_SideWeapons = new List<DroneWeapon>();
        }

        public void AddSideWeapons(DroneWeapon _weapon)
        {
            if (m_SideWeapons.Exists(w => w.TheWeapon == _weapon.TheWeapon))
            {
                var oldOne = m_SideWeapons.IndexOf(m_SideWeapons.Find(w => w.TheWeapon == _weapon.TheWeapon));
                m_SideWeapons[oldOne] = new DroneWeapon { TheWeapon = _weapon.TheWeapon, ShootingChance = _weapon.ShootingChance };
            }
            else
            {
                m_SideWeapons.Add(_weapon);
            }
        }
        #endregion

        #region Shooting Process
        public void StartAutomaticShooting()
        {
            StartCoroutine(shootingProcess());
        }

        public void StopAutomaticShooting()
        {
            StopAllCoroutines();
        }

        private IEnumerator shootingProcess()
        {
            yield return new WaitForSeconds(0.5f);
            m_DroneMovement.IsActive = true;

            while (m_DroneMovement.IsActive)
            {
                if (m_CanShoot)
                {
                    Shoot();
                }

                yield return m_ShootingWait;
            }
        }

        public void Shoot()
        {
            if (!m_CanShoot)
            {
                return;
            }

            m_CanShoot = false;
            Weapon selectedWeapon;

            // if the drone doesn't have side weapons, then there is no need to do extra caching
            if (m_SideWeapons.Count == 0)
            {
                selectedWeapon = m_MainWeapon;
            }
            else
            {
                float sideWeaponChance = Random.value;
                List<DroneWeapon> weapons = m_SideWeapons.FindAll(w => w.ShootingChance >= sideWeaponChance);

                // if the drone is going to use its main weapon
                if (weapons.Count == 0)
                {
                    selectedWeapon = m_MainWeapon;
                }
                else
                {
                    var selected = weapons[Random.Range(0, weapons.Count)];
                    selectedWeapon = selected.TheWeapon;
                }
            }

            ShootAccordingToShooter(selectedWeapon);
            Invoke(nameof(EnableShooting), m_WeaponCoolDown);
        }

        private void ShootAccordingToShooter(Weapon _weapon)
        {
            if (_weapon.TheWeaponTag == WeaponTag.Bullets || _weapon.TheWeaponTag == WeaponTag.Rockets)
            {
                BulletWeapon weapon = (BulletWeapon)_weapon;
                TheShooter.SetUpWeapon(weapon);
                setCoolDownValueBullets(weapon);
                TheShooter.Shoot();
            }
            else if (_weapon.TheWeaponTag == WeaponTag.Laser && !m_LaserShooter.IsShooting)
            {
                LaserWeapon weapon = (LaserWeapon)_weapon;
                m_LaserShooter.SetWeapon(weapon);
                setCoolDownValueLaser(weapon);
                m_LaserShooter.Shoot();
            }
        }

        private void EnableShooting()
        {
            m_CanShoot = true;
        }

        private void setCoolDownValueBullets(BulletWeapon _weapon)
        {
            if (_weapon.ShootingType == BulletWeapon.ShootingTypes.AngleUpdatePerIndex)
            {
                if (_weapon.UpdatedShotsVariables.HalfLoop)
                {
                    m_WeaponCoolDown = _weapon.CoolDownTime + _weapon.UpdatedShotsVariables.CoolDownTimeBetweenShots * _weapon.UpdatedShotsVariables.ShotsNumber;
                }
                else
                {
                    m_WeaponCoolDown = _weapon.CoolDownTime + _weapon.UpdatedShotsVariables.CoolDownTimeBetweenShots * _weapon.UpdatedShotsVariables.ShotsNumber * 2;
                }
            }
            else
            {
                m_WeaponCoolDown = _weapon.CoolDownTime;
            }
        }

        private void setCoolDownValueLaser(LaserWeapon _weapon)
        {
            m_WeaponCoolDown = _weapon.CoolDownTime;
        }
        #endregion

        #region External Access
        public Weapon GetMainWeapon()
        {
            return m_MainWeapon;
        }

        public List<DroneWeapon> GetSideWeapons()
        {
            return m_SideWeapons;
        }

        public void StopDrone()
        {
            m_CanShoot = false;
            StopAllCoroutines();
            m_DroneMovement.HideDrone();
        }

        public Drone GetMovement()
        {
            return m_DroneMovement;
        }
        #endregion
    }
}