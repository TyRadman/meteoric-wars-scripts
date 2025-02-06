using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    public class DamageDetector : MonoBehaviour
    {
        [HideInInspector] public ShipComponenets Components;
        [HideInInspector] public Collider2D DetectorCollider;
        public bool TakesDamage = true;
        public static float LaserShootingFrequency = 0.2f;
        private bool m_CanGetShotByLasers = true;
        private IDamagable m_DamageTaker;

        private void Awake()
        {
            DetectorCollider = GetComponent<Collider2D>();
            m_DamageTaker = GetComponent<IDamagable>();

            // if we haven't set the components of the ship already then we get it (an attempt to save processing and getting components time)
            if (Components == null && GetComponent<ShipComponenets>() != null)
            {
                Components = GetComponent<ShipComponenets>();
            }
        }

        public virtual void GotShot(float _damage, Vector3 _impactPoint, int _playerIndex = -1)
        {
            PointsPopUp.i.CallText(_impactPoint, PointsPopUp.PopUpTextType.Damage, _damage);
            // deal damage
            m_DamageTaker.TakeDamage(_damage);

            // play particles if it's a ship
            if (Components == null)
            {
                return;
            }

            PoolingSystem.Instance.UseParticles(ParticlePoolTag.StandardImpact, _impactPoint, Quaternion.identity);
            Components.Effects.TakeDamageColorChanging();
        }

        public void GotLaserShot(float _damage, Vector3 _impactPoint, int _playerIndex)
        {
            if (!m_CanGetShotByLasers) return;

            m_CanGetShotByLasers = false;
            Invoke(nameof(EnableLaserShots), LaserShootingFrequency);
            PointsPopUp.i.CallText(transform.position, PointsPopUp.PopUpTextType.Damage, _damage);

            if (Components == null) return;

            // deal damage
            Components.Health.TakeDamage(_damage);
            // play particles
            PoolingSystem.Instance.UseParticles(ParticlePoolTag.StandardImpact, _impactPoint, Quaternion.identity);
            Components.Effects.TakeDamageColorChanging();
        }

        private void EnableLaserShots()
        {
            m_CanGetShotByLasers = true;
        }

        public void Enable()
        {
            DetectorCollider.enabled = true;
        }

        public void Disable()
        {
            DetectorCollider.enabled = false;
        }
    }
}