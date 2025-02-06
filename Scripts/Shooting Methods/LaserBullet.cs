using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    public class LaserBullet : MonoBehaviour
    {
        private float DamagePerSecond = 10f;
        [SerializeField] private LineRenderer m_Line;
        public BoxCollider2D DetectionCollider;
        [SerializeField] private Rigidbody2D m_Rb;
        [HideInInspector] public string m_HittingTag = "Empty";
        private bool m_IsActive = false;
        private int m_UserIndex = -1;

        public void SetWidth(float _size)
        {
            m_Line.widthMultiplier = _size;
            DetectionCollider.size = new Vector2(_size, DetectionCollider.size.y);
        }

        public void Activate(bool _activate)
        {
            m_Line.enabled = _activate;
            m_Rb.simulated = _activate;
            DetectionCollider.enabled = _activate;
            m_IsActive = _activate;
        }

        public void SetUp(float _damage, string _targetTag, int _userIndex, Gradient _color)
        {
            m_UserIndex = _userIndex;
            DamagePerSecond = _damage * DamageDetector.LaserShootingFrequency;
            m_HittingTag = _targetTag;
            m_Line.colorGradient = _color;
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (!m_IsActive)
            {
                return;
            }

            if (other.CompareTag(m_HittingTag))
            {
                var damageDetector = other.GetComponent<DamageDetector>();

                if (damageDetector != null && damageDetector.TakesDamage)
                {
                    damageDetector.GotLaserShot(DamagePerSecond, other.ClosestPoint(transform.position), m_UserIndex);
                }
                else
                {
                    PoolingSystem.Instance.UseParticles(ParticlePoolTag.StandardImpact, transform.position, Quaternion.identity);
                }
            }
        }

        public float GetLaserWidth()
        {
            return m_Line.widthMultiplier;
        }
    }
}