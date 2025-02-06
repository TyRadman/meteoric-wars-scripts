using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    public class ShipComponenets : MonoBehaviour, IController
    {
        [field: SerializeField] public virtual ShipHealth Health { set; get; }
        [field: SerializeField] public virtual ShipEffects Effects { set; get; }
        [field: SerializeField] public virtual Stats ShipStats { set; get; }
        [field: SerializeField] public virtual Shooter ShipShooter { set; get; }

        private List<DamageDetector> m_DamageDetectors = new List<DamageDetector>();

        protected virtual void Awake()
        {
            Effects = GetComponent<ShipEffects>();
            ShipStats = GetComponent<Stats>();
            ShipShooter = GetComponent<Shooter>();
            //Health = GetComponent<PlayerHealth>();
        }

        public virtual void SetUp(IController components)
        {
            // in case it didn't have damage detectors already cached
            if (m_DamageDetectors.Count == 0)
            {
                m_DamageDetectors.Add(GetComponent<DamageDetector>());
            }
        }

        private void Start()
        {
        }

        public virtual void GetComponents()
        {

        }

        public virtual void OnDeath()
        {

        }

        public void DisableDamageDetectors()
        {
            m_DamageDetectors.ForEach(d => d.Disable());
        }

        public void EnableColliders(bool _on)
        {
            m_DamageDetectors.ForEach(d => d.DetectorCollider.enabled = _on);
        }

        // when we want to spawn the ship again for whatever reason
        public virtual void ResetValues()
        {
            Health.SetCurrentHealthToMaxHealth();
            ShipShooter.EnableShooting();
        }

        public virtual void Activate()
        {
            throw new System.NotImplementedException();
        }

        public virtual void Deactivate()
        {
            throw new System.NotImplementedException();
        }

        public virtual void Dispose()
        {
            throw new System.NotImplementedException();
        }
    }
}