using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    public class DFCrazyDrone : Ability
    {
        [System.Serializable]
        public struct DFCrazyDronesValues
        {
            public int CoolDownTime;
            public float Duration;
        }

        [SerializeField] private DFCrazyDronesValues[] m_Levels;
        [Header("Special Variables")]
        [SerializeField] private Weapon m_MainWeapon;
        [SerializeField] private DroneShooting.DroneWeapon[] m_SideWeapons;
        [SerializeField] private float m_FollowTargetSpeed = 0.5f;
        [SerializeField] private float m_VerticalPosition = -6f;
        [SerializeField] private float m_ShootingRate = 0.2f;
        [SerializeField] private float m_SuperShotChance = 0.3f;
        private Weapon m_PreviousWeapon;
        private List<DroneShooting.DroneWeapon> m_PreviousSideWeapons;
        private CustomShooterDrone m_DronesParent;
        private float m_Duration;
        private Drone m_DroneMovement;
        private DroneShooting m_DroneShooter;

        public override void SetLevelValues(int _levelNumber)
        {
            base.SetLevelValues(_levelNumber);
            DFCrazyDronesValues level = m_Levels[_levelNumber];
            m_Duration = level.Duration;
            CountDownTime = level.CoolDownTime;
        }

        public override void SetUp(Transform _ship = null)
        {
            base.SetUp(_ship);
            m_DronesParent = _ship.GetComponent<CustomShooterDrone>();
        }

        public override void Activate()
        {
            base.Activate();

            if (!IsAvailable)
            {
                return;
            }

            IsAvailable = false;
            startCrazyDrone();
            Slot.StartAbilityUsageCountdown(m_Duration);
            Invoke(nameof(RechargeAbility), m_Duration);
        }

        private void startCrazyDrone()
        {
            // select the drone
            m_DroneShooter = m_DronesParent.GetAndStopRandomDrone();
            m_DroneMovement = m_DroneShooter.GetComponent<Drone>();
            // cache last weapon
            m_PreviousWeapon = m_DroneShooter.GetMainWeapon();
            m_PreviousSideWeapons = m_DroneShooter.GetSideWeapons();
            // set new weapons
            m_DroneShooter.SetMainWeapon(m_MainWeapon);
            m_DroneShooter.SetShootingRate(m_ShootingRate, m_SuperShotChance);
            m_DroneShooter.RemoveAllSideWeapons();
            System.Array.ForEach(m_SideWeapons, w => m_DroneShooter.AddSideWeapons(w));
            // stop shooting (stopped the moment we remove the drone from the drones list)
            // stop movement 
            m_DroneMovement.StopMovement();
            // start color transition

            // start new movement
            StartCoroutine(movementProcess());
            // start shooting process
            m_DroneShooter.StartAutomaticShooting();
        }

        private void stopCrazyDrone()
        {
            // return the previous weapons
            m_DroneShooter.RemoveAllSideWeapons();
            m_DroneShooter.SetMainWeapon(m_PreviousWeapon);
            m_PreviousSideWeapons.ForEach(w => m_DroneShooter.AddSideWeapons(w));
            // re attach the drone to its parents
            m_DronesParent.AddDrone(m_DroneShooter);
            // start the drone's default movement 
            m_DroneMovement.StartMovement(0);
            // stop drone's shooting 
            m_DroneShooter.StopAutomaticShooting();
        }

        public override void RechargeAbility()
        {
            base.RechargeAbility();
            stopCrazyDrone();
        }

        private IEnumerator movementProcess()
        {
            float time = 0f;
            Transform droneTransform = m_DroneMovement.transform;

            while (time < m_Duration)
            {
                time += Time.deltaTime;
                // choose a target
                float targetPosX = SniperWeapon.GetTarget(droneTransform, BulletUser.Player).x;
                droneTransform.position = new Vector2(Mathf.Lerp(droneTransform.position.x, targetPosX, m_FollowTargetSpeed), m_VerticalPosition);
                yield return null;
            }
        }
    }
}