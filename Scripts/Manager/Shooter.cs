using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpaceWar.Audios;

namespace SpaceWar
{
    /// <summary>
    /// A class that is responsible of shooting bullets based on some properties in different styles
    /// </summary>
    public class Shooter : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private BulletWeapon m_CurrentWeapon;
        public ShipEffects Effects;
        public List<Transform> ShootingPoints = new List<Transform>();
        public bool CanShoot = true;
        private WeaponShootingProcess m_SelectedWeaponShooting;
        public BulletUser UserTag;
        public float Damage;
        public int UserIndex = -1;
        private Audio m_ShootingAudio;

        private void Awake()
        {
            Effects = GetComponent<ShipEffects>();
        }

        private void OnEnable()
        {
            if (UserTag == BulletUser.Enemy || m_SelectedWeaponShooting == null)
            {
                SetWeaponShooting();
            }
        }

        public void SetWeaponShooting()
        {
            if (m_CurrentWeapon == null)
            {
                return;
            }

            switch (m_CurrentWeapon.ShootingType)
            {
                case BulletWeapon.ShootingTypes.Standard:
                    {
                        if (GetComponent<StandardWeapon>() == null)
                        {
                            m_SelectedWeaponShooting = gameObject.AddComponent<StandardWeapon>();
                        }
                        else
                        {
                            m_SelectedWeaponShooting = GetComponent<StandardWeapon>();
                        }

                        m_SelectedWeaponShooting.SetUpComponents(this);
                        break;
                    }
                case BulletWeapon.ShootingTypes.AngleUpdatePerIndex:
                    {
                        if (GetComponent<UpdatingAnglesWeapon>() == null)
                        {
                            m_SelectedWeaponShooting = gameObject.AddComponent<UpdatingAnglesWeapon>();
                        }
                        else
                        {
                            m_SelectedWeaponShooting = GetComponent<UpdatingAnglesWeapon>();
                        }

                        m_SelectedWeaponShooting.SetUpComponents(this);
                        break;
                    }
                case BulletWeapon.ShootingTypes.Chasing:
                    {
                        if (GetComponent<ChasingWeapon>() == null)
                        {
                            m_SelectedWeaponShooting = gameObject.AddComponent<ChasingWeapon>();
                        }
                        else
                        {
                            m_SelectedWeaponShooting = GetComponent<ChasingWeapon>();
                        }

                        m_SelectedWeaponShooting.SetUpComponents(this);
                        break;
                    }
                case BulletWeapon.ShootingTypes.Sniper:
                    {
                        if (GetComponent<SniperWeapon>() == null)
                        {
                            m_SelectedWeaponShooting = gameObject.AddComponent<SniperWeapon>();
                        }
                        else
                        {
                            m_SelectedWeaponShooting = GetComponent<SniperWeapon>();
                        }

                        m_SelectedWeaponShooting.SetUpComponents(this);
                        break;
                    }
                case BulletWeapon.ShootingTypes.SniperMachineGun:
                    {
                        if (GetComponent<SniperMachineGunWeapon>() == null)
                        {
                            m_SelectedWeaponShooting = gameObject.AddComponent<SniperMachineGunWeapon>();
                        }
                        else
                        {
                            m_SelectedWeaponShooting = GetComponent<SniperMachineGunWeapon>();
                        }

                        m_SelectedWeaponShooting.SetUpComponents(this);
                        break;
                    }
            }

            m_SelectedWeaponShooting.HasAudio = UserTag == BulletUser.Player;

            if (GetComponent<DroneShooting>() != null)
            {
            }
        }

        public void Shoot()
        {
            if (!CanShoot)
            {
                return;
            }

            CanShoot = false;

            Invoke(nameof(EnableShooting), m_SelectedWeaponShooting.CoolingDownTime);

            m_SelectedWeaponShooting.StartShootingProcess();
        }

        public void EnableShooting()
        {
            CanShoot = true;
        }

        public void SetUpWeapon(BulletWeapon _weapon, Audio _audio = null)
        {
            m_CurrentWeapon = _weapon;
            Damage = _weapon.DamagePerShot;
            m_SelectedWeaponShooting = null;
            SetWeaponShooting();

            if (_audio != null) m_SelectedWeaponShooting.ShootingAudio = _audio;
            else m_SelectedWeaponShooting.ShootingAudio = _weapon.Audio;
        }

        public BulletWeapon Weapon()
        {
            return m_CurrentWeapon;
        }

        public void SetUpShootingPoints(List<Transform> _points)
        {
            ShootingPoints = new List<Transform>();
            ShootingPoints.AddRange(_points);
        }

        public void SetUserIndex(int _index)
        {
            UserIndex = _index;
        }
    }
}