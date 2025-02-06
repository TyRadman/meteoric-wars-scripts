using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SpaceWar
{
    public class PlayerShooting : MonoBehaviour, IController
    {
        private PlayerComponents m_Components;
        private Shooter m_Shooter;
        // a shooter for weapons collected as abilities
        private Shooter m_AbilitiesShooter;
        private LaserShooter m_AbilitiesLaserShooter;
        public CustomShooter CustomShooter;
        private GameObject m_AbilitiesShooterParent;
        private LaserShooter m_LaserShooter;
        public WeaponTag WeaponType;
        public bool CanShoot = false;
        public bool PlayerCanShootEffectShot = false;
        private bool m_IsShooting = false;

        public void SetUp(IController components)
        {
            m_Components = (PlayerComponents)components;
            CanShoot = true;
            m_Shooter = GetComponent<Shooter>();
            m_LaserShooter = GetComponent<LaserShooter>();
            SetUpSecondShooters();

            if (WeaponType == WeaponTag.Custom)
            {
                CustomShooter = GetComponent<CustomShooter>();
            }
        }

        #region Input
        public void SetUpInput(int playerIndex)
        {
            GamePlayInput c = InputController.Controls;
            InputActionMap playerMap = InputController.GetMap(playerIndex, ActionMap.GamePlay);
            playerMap.FindAction(c.Gameplay.Shoot.name).performed += StartShooting;
            playerMap.FindAction(c.Gameplay.Shoot.name).canceled += StopShooting;
        }

        public void DisposeInput(int playerIndex)
        {
            GamePlayInput c = InputController.Controls;
            InputActionMap playerMap = InputController.GetMap(playerIndex, ActionMap.GamePlay);
            playerMap.FindAction(c.Gameplay.Shoot.name).performed -= StartShooting;
            playerMap.FindAction(c.Gameplay.Shoot.name).canceled -= StopShooting;
        }
        #endregion

        #region IController
        public void Activate()
        {
            SetUpInput(m_Components.PlayerIndex);
        }

        public void Deactivate()
        {
            DisposeInput(m_Components.PlayerIndex);
        }

        public void Dispose()
        {
            DisposeInput(m_Components.PlayerIndex);
        }
        #endregion

        private void StartShooting(InputAction.CallbackContext context)
        {
            m_IsShooting = true;
            StartCoroutine(ShootingProcess());
        }

        private void StopShooting(InputAction.CallbackContext context)
        {
            m_IsShooting = false;
        }

        private IEnumerator ShootingProcess()
        {
            while (m_IsShooting)
            {
                Shoot();
                yield return null;
            }
        }

        private void SetUpSecondShooters()
        {
            m_AbilitiesShooterParent = new GameObject("Second Shooter");
            m_AbilitiesShooterParent.transform.parent = transform;
            m_AbilitiesShooterParent.transform.localPosition = Vector2.zero;
            m_AbilitiesShooter = m_AbilitiesShooterParent.AddComponent<Shooter>();
            m_AbilitiesLaserShooter = m_AbilitiesShooterParent.AddComponent<LaserShooter>();
            m_AbilitiesLaserShooter.SetShooter(m_AbilitiesShooter);
            m_AbilitiesShooter.SetUpShootingPoints(new List<Transform> { m_AbilitiesShooterParent.transform });
            m_AbilitiesLaserShooter.SetUpShootingPoints(m_AbilitiesShooterParent.transform);
            m_AbilitiesShooter.UserIndex = m_Shooter.UserIndex;
            m_AbilitiesShooter.Effects = m_Components.Effects;
        }

        public void SetShooter(Shooter _shooter)
        {
            m_Shooter = _shooter;
        }

        public void Shoot()
        {
            if (!CanShoot || !m_Components.IsAlive)
            {
                return;
            }

            // if the player shoots then cancel any upcoming regeneration
            m_Components.Health.StopRegenerating();

            if (WeaponType == WeaponTag.Bullets || WeaponType == WeaponTag.Rockets)
            {
                m_Shooter.Shoot();
            }
            else if (WeaponType == WeaponTag.Laser)
            {
                m_LaserShooter.Shoot();
            }
            else if (WeaponType == WeaponTag.Custom)
            {
                CustomShooter.Shoot();
            }
        }

        #region Abilities Bullets Shooter
        public void AbilityBulletShoot()
        {
            if (!CanShoot)
            {
                return;
            }

            m_AbilitiesShooter.Shoot();
        }

        public void SetAbilitiesBulletWeapon(BulletWeapon _weapon)
        {
            m_AbilitiesShooter.SetUpWeapon(_weapon);
        }
        #endregion

        #region Abilities Laser Shooter
        public void AbilityLaserShoot()
        {
            if (!CanShoot)
            {
                return;
            }

            m_AbilitiesLaserShooter.Shoot();
        }

        public void SetAbilitiesLaserWeapon(LaserWeapon _weapon)
        {
            if (m_AbilitiesLaserShooter.CurrentWeapon != _weapon)
            {
                m_AbilitiesLaserShooter.SetWeapon(_weapon);
            }
        }

        public LaserShooter GetLaserShooter()
        {
            return m_AbilitiesLaserShooter;
        }

        public void Enable(bool enable)
        {
            CanShoot = enable;
            m_IsShooting = false;
        }
        #endregion
    }
}