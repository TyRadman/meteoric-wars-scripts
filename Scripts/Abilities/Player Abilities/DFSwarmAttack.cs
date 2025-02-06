using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    public class DFSwarmAttack : Ability
    {
        [SerializeField] private List<DFSwarmAttackLevel> m_Levels;
        [SerializeField] private int m_NumberOfDrones = 3;
        private float m_Duration;
        private Animation m_TransformationAnimation;
        [SerializeField] private AnimationClip m_DisappearAnimation;
        [SerializeField] private AnimationClip m_AppearAnimation;
        private List<Transform> m_DroneSpawnPositions = new List<Transform>();
        private List<DroneShooting> m_OriginalDrones;
        private List<DroneShooting> m_NewDrones = new List<DroneShooting>();
        private WaitForSeconds m_WaitForSeconds;
        private PlayerComponents m_Components;
        private Transform m_Ship;
        // original values
        private int m_OriginalLayer;
        private float m_OriginalAroundTheShipArea;
        private float m_OriginalParentFollowingSpeed;
        private float m_OriginalPlayerSpeed;
        private Vector2 m_OriginalFrequencySpeed;
        // new values
        private readonly int m_UnhittableLayer = 2;
        private float m_NewPlayerSpeed;
        [SerializeField] private float m_NewAroundTheShipArea = 2.5f;
        [SerializeField] private float m_NewParentFollowingSpeed = 0.04f;
        private Vector2 m_NewFrequencySpeed;

        [System.Serializable]
        public struct DFSwarmAttackLevel
        {
            public float Duration;
        }

        public override void SetLevelValues(int _levelNumber)
        {
            base.SetLevelValues(_levelNumber);

            DFSwarmAttackLevel level = m_Levels[_levelNumber];
            m_Duration = level.Duration;
        }

        public override void SetUp(Transform _ship = null)
        {
            base.SetUp(_ship);

            m_Ship = _ship;
            m_OriginalLayer = m_Ship.gameObject.layer;
            m_Components = _ship.GetComponent<PlayerComponents>();
            m_TransformationAnimation = _ship.GetChild(0).GetComponent<Animation>();
            CustomShooterDrone shooter = _ship.GetComponent<CustomShooterDrone>();
            shooter.CreateDrones(m_NumberOfDrones, false);
            m_OriginalDrones = shooter.GetDrones();
            m_WaitForSeconds = new WaitForSeconds(m_AppearAnimation.length);

            // add the newly made drones
            for (int i = 0; i < m_NumberOfDrones; i++)
            {
                m_NewDrones.Add(m_OriginalDrones[m_OriginalDrones.Count - 1 - i]);
                m_DroneSpawnPositions.Add(shooter.transform.GetChild(1).GetChild(i));
            }

            m_NewDrones.ForEach(d => m_OriginalDrones.Remove(d));

            m_OriginalAroundTheShipArea = m_OriginalDrones[0].GetMovement().GetFloatingDistance();
            m_OriginalParentFollowingSpeed = m_OriginalDrones[0].GetMovement().GetParentFollowingSpeed();
            m_OriginalFrequencySpeed = m_OriginalDrones[0].GetMovement().GetTargetFollowFrequency();
            m_NewFrequencySpeed = m_OriginalFrequencySpeed * 0.1f;
            // get the player's original speed
            m_OriginalPlayerSpeed = m_Components.Movement.GetMovementSpeed();
            m_NewPlayerSpeed = m_OriginalPlayerSpeed * 2f;
        }

        public override void Activate()
        {
            base.Activate();

            if (!IsAvailable)
            {
                return;
            }

            IsAvailable = false;
            StartCoroutine(ShowDrones());
        }

        public override void RechargeAbility()
        {
            base.RechargeAbility();
            StartCoroutine(HideDrones());
        }

        private IEnumerator ShowDrones()
        {
            PoolingSystem.Instance.UseParticles(ParticlePoolTag.DronesFighterDissolve, m_Components.transform.position, Quaternion.identity);
            m_Components.ThePlayerShooting.CanShoot = false;
            m_Components.Movement.Enable(false);
            m_Components.Effects.Thrusters.ForEach(t => t.Stop());
            m_TransformationAnimation.clip = m_DisappearAnimation;
            m_TransformationAnimation.Play();
            // prevent the effects class from the changing the sprite's color so that the ship completely disappears
            m_Components.Effects.SetAvailability(false);

            yield return m_WaitForSeconds;

            // add the new drones to the custom shooting drones
            m_OriginalDrones.AddRange(m_NewDrones);
            m_OriginalDrones.ForEach(d => d.GetMovement().SetValues(m_NewAroundTheShipArea, m_NewParentFollowingSpeed));
            m_OriginalDrones.ForEach(d => d.GetMovement().SetTargetFollowFrequency(m_NewFrequencySpeed));
            // set the players speed
            m_Components.Movement.SetSpeed(m_NewPlayerSpeed);
            // change the layer of the player so that it collides with the walls but not with the bullets
            m_Ship.gameObject.layer = m_UnhittableLayer;

            // make drones Appear
            for (int i = 0; i < m_NumberOfDrones; i++)
            {
                m_NewDrones[i].gameObject.SetActive(true);
                m_NewDrones[i].GetMovement().ShowDrone(m_DroneSpawnPositions[i].position, ParticlePoolTag.RedDroneSpawnParticle);
            }

            m_Components.ThePlayerShooting.CanShoot = true;
            m_Components.Movement.Enable(true);
            Slot.StartAbilityUsageCountdown(m_Duration);
            Invoke(nameof(RechargeAbility), m_Duration);
        }

        private IEnumerator HideDrones()
        {
            m_TransformationAnimation.clip = m_AppearAnimation;
            m_TransformationAnimation.Play();

            //hide the drones
            // here
            for (int i = 0; i < m_NumberOfDrones; i++)
            {
                m_NewDrones[i].GetMovement().HideDrone(ParticlePoolTag.RedDroneSpawnParticle);
                m_NewDrones[i].gameObject.SetActive(false);
            }

            yield return m_WaitForSeconds;

            // start the thrusters of the mother ship
            m_Components.Effects.Thrusters.ForEach(t => t.Play());
            m_NewDrones.ForEach(d => m_OriginalDrones.Remove(d));
            m_OriginalDrones.ForEach(d => d.GetMovement().SetValues(m_OriginalAroundTheShipArea, m_OriginalParentFollowingSpeed));
            m_OriginalDrones.ForEach(d => d.GetMovement().SetTargetFollowFrequency(m_OriginalFrequencySpeed));
            // reset the player's speed
            m_Components.Movement.SetSpeed(m_OriginalPlayerSpeed);
            // reset player's layer
            m_Ship.gameObject.layer = m_OriginalLayer;
            // enable shooting
            m_Components.ThePlayerShooting.CanShoot = true;
            m_Components.Effects.SetAvailability(false);
            //m_ShipMovement.EnableMovement(true);
        }
    }
}