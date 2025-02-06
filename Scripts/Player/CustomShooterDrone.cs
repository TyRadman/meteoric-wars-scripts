using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    public class CustomShooterDrone : CustomShooter
    {
        [System.Serializable]
        public struct DroneWeaponSetWithLevel
        {
            public Weapon TheWeapon;
            public int Level;
            public float Chance;
        }

        [SerializeField] private List<DroneWeaponSetWithLevel> m_MainWeapons;
        [SerializeField] private List<DroneWeaponSetWithLevel> m_SideWeapons;
        [SerializeField] private GameObject m_DronePrefab;
        [SerializeField] private int m_DronesNumber;
        [SerializeField] private float m_StartDelay = 2f;
        private List<DroneShooting> m_Drones = new List<DroneShooting>();
        private PlayerComponents m_Components;

        private void Awake()
        {
            m_Components = GetComponent<PlayerComponents>();
            GetComponent<PlayerShipInput>().EnableKeyUsage(KeyTag.NormalShot, false);
        }

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(m_StartDelay);
            CreateDrones(m_DronesNumber);
        }

        public void CreateDrones(int _number, bool _showDrones = true)
        {
            // disable the usage of the shooting key
            GetComponent<PlayerShipInput>().EnableKeyUsage(KeyTag.NormalShot, false);

            for (int i = 0; i < _number; i++)
            {
                var droneShooting = Instantiate(m_DronePrefab, transform).GetComponent<DroneShooting>();
                droneShooting.transform.parent = null;
                m_Drones.Add(droneShooting);
                Drone droneMovement = droneShooting.GetComponent<Drone>();
                droneMovement.SetParent(transform);
                droneShooting.SetMainWeapon(m_MainWeapons[0].TheWeapon);
                droneShooting.AddSideWeapons(new DroneShooting.DroneWeapon() { TheWeapon = m_SideWeapons[0].TheWeapon, ShootingChance = m_SideWeapons[0].Chance });
                // set the index of the drones' shooter as the player so that when they destroy an enemy, it counts as if the player destroyed an enemy
                droneShooting.TheShooter.SetUserIndex(m_Components.PlayerIndex);

                if (!_showDrones)
                {
                    droneShooting.gameObject.SetActive(false);
                }
                else
                {
                    droneMovement.ShowDrone(transform.position, ParticlePoolTag.RedDroneSpawnParticle);
                }
            }

            Invoke(nameof(enableUsingShootingKey), 1f);
        }

        private void enableUsingShootingKey()
        {
            GetComponent<PlayerShipInput>().EnableKeyUsage(KeyTag.NormalShot, true);
        }

        public override void Shoot()
        {
            base.Shoot();
            m_Drones.ForEach(d => d.Shoot());
        }

        public override void Upgrade(Weapon _newWeapon, int _level)
        {
            base.Upgrade(_newWeapon, _level);
            // find the weapon that has the matching level
            if (m_MainWeapons.FindAll(w => w.Level == _level).Count != 0)
            {
                SetWeaponForDrones(m_MainWeapons.Find(w => w.Level == _level));
            }

            if (m_SideWeapons.Exists(w => w.Level == _level))
            {
                SetSideWeapon(m_SideWeapons.Find(w => w.Level == _level));
            }
        }

        public void SetWeaponForDrones(DroneWeaponSetWithLevel _weapon)
        {
            // set the weapon for all drones
            foreach (DroneShooting drone in m_Drones)
            {
                drone.SetMainWeapon(_weapon.TheWeapon);
            }
        }

        public void SetSideWeapon(DroneWeaponSetWithLevel _weapon)
        {
            foreach (DroneShooting drone in m_Drones)
            {
                drone.RemoveAllSideWeapons();
                drone.AddSideWeapons(new DroneShooting.DroneWeapon { TheWeapon = _weapon.TheWeapon, ShootingChance = _weapon.Chance });
            }
        }

        public DroneShooting GetAndStopRandomDrone()
        {
            var drone = m_Drones[0];
            m_Drones.Remove(drone);
            return drone;
        }

        public void AddDrone(DroneShooting _drone)
        {
            m_Drones.Add(_drone);
        }

        public List<DroneShooting> GetDrones()
        {
            return m_Drones;
        }

        // for the ability
        public void ActivateAdditionalDrones()
        {
            for (int i = m_DronesNumber - 1; i < m_Drones.Count; i++)
            {
                m_Drones[i].gameObject.SetActive(true);
                m_Drones[i].GetComponent<Drone>().ShowDrone(transform.position, ParticlePoolTag.RedDroneSpawnParticle);
            }
        }
    }
}