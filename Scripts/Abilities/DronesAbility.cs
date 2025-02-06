using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    public class DronesAbility : Ability
    {
        [SerializeField] private GameObject m_DronePrefab;
        private Transform m_DronesParent;
        private List<DroneShooting> m_Drones = new List<DroneShooting>();
        [SerializeField] private List<DronesLevel> m_Levels;
        private int m_OwnerUserIndex = -1;

        [System.Serializable]
        public struct DronesLevel
        {
            public int DronesNumber;
            public float SuperChance;
            [Tooltip("In accordance to the number of drones")]
            public List<float> AttackRates;
        }

        public override void SetLevelValues(int _levelNumber)
        {
            base.SetLevelValues(_levelNumber);

            DronesLevel level = m_Levels[_levelNumber];
            m_Drones.ForEach(d => Destroy(d.gameObject));
            m_Drones.Clear();

            // creates new drones if there are no drones or if the required drones are more in number than the existing drones
            for (int i = 0; i < level.DronesNumber; i++)
            {
                createDrone();
            }

            // sets shooting rates for drones
            for (int i = 0; i < m_Drones.Count; i++)
            {
                m_Drones[i].SetShootingRate(level.AttackRates[i], level.SuperChance);
            }
        }

        public override void SetUp(Transform _ship = null)
        {
            // set the parent of the drones
            m_DronesParent = _ship;
            // cache the user index to pass it to the drones so that their kills count as the player's kills
            m_OwnerUserIndex = _ship.GetComponent<Shooter>().UserIndex;

            base.SetUp(_ship);
        }

        private void createDrone()
        {
            var drone = Instantiate(m_DronePrefab).GetComponent<DroneShooting>();
            drone.TheShooter.UserIndex = m_OwnerUserIndex;
            m_Drones.Add(drone);
            Drone droneMovement = drone.GetComponent<Drone>();
            droneMovement.SetParent(m_DronesParent);
            droneMovement.ShowDrone(m_DronesParent.position);
        }

        public override void ForceStop()
        {
            base.ForceStop();

            m_Drones.ForEach(d => d.StopDrone());
        }
    }
}