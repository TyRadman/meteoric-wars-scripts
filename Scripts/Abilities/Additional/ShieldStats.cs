using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    public class ShieldStats : MonoBehaviour, IDamagable
    {
        [SerializeField] private float m_MaxHealth;
        [SerializeField] private float m_CurrentHealth;
        private IOnShieldDestroyed m_ShieldAbility;
        private bool m_TakeDamage = false;
        [SerializeField] private ParticlePoolTag m_OnDestroyTag;
        [SerializeField] private float m_RotationSpeed = 5f;
        [Header("Animation References")]
        [SerializeField] private Animation m_Anim;
        [SerializeField] private AnimationClip m_ShowClip;
        [SerializeField] private AnimationClip m_HideClip;
        [SerializeField] private AnimationClip m_DamageClip;
        [SerializeField] private AnimationClip m_DestroyClip;
        private bool m_IsActive = false;

        private void Update()
        {
            if (!m_IsActive) return;

            transform.Rotate(Vector3.forward * m_RotationSpeed * Time.deltaTime);
        }

        public void TakeDamage(float _damage)
        {
            if (!m_TakeDamage) return;

            m_CurrentHealth += _damage;
            TakeDamageVisual();

            if (m_CurrentHealth <= 0)
            {
                m_TakeDamage = false;
                m_ShieldAbility.OnShieldDestroyed();
                PoolingSystem.Instance.UseParticles(m_OnDestroyTag, transform.position, Quaternion.identity);
                return;
            }

            // play impact particles
        }

        public void SetUp(float _maxHealth, IOnShieldDestroyed _shieldAbilityStyle)
        {
            m_MaxHealth = _maxHealth;
            m_ShieldAbility = _shieldAbilityStyle;
            ResetHealth();
        }

        public void ResetHealth()
        {
            m_TakeDamage = true;
            m_CurrentHealth = m_MaxHealth;
        }

        public void ShowShield(bool _show)
        {
            m_IsActive = _show;
            m_Anim.clip = _show ? m_ShowClip : m_HideClip;
            m_Anim.Play();
        }

        public void HideShield()
        {
            m_Anim.clip = m_HideClip;
            m_Anim.Play();
        }

        public void TakeDamageVisual()
        {
            if (m_Anim.clip == m_DamageClip) m_Anim.Stop();
            else if (m_Anim.isPlaying) return;

            m_Anim.clip = m_DamageClip;
            m_Anim.Play();
        }

        public void DestroyAnimation()
        {
            m_IsActive = false;
            m_Anim.clip = m_DestroyClip;
            m_Anim.Play();
        }
    }
}