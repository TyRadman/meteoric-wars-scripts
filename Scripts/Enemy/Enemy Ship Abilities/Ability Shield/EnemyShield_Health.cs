using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    [CreateAssetMenu(menuName = "Enemy Abilities/Shield/Shield With Health")]
    public class EnemyShield_Health : EnemyShieldAbilityParent, IOnShieldDestroyed
    {
        private Shield_EnemyAbility m_Ability;
        private ShieldStats m_ShieldStats;
        [SerializeField] private Vector2 m_ShieldHealthRange;
        private float m_ShieldHealth;

        public override void Perform(Shield_EnemyAbility _ability)
        {
            base.Perform(_ability);
            m_ShieldStats.ResetHealth();
            _ability.m_ShieldAbility.Activate();
            _ability.AbilitiesHolder.ResumeMovement();
            _ability.AbilitiesHolder.ResumeShooting();
            m_Ability = _ability;
        }

        public void OnShieldDestroyed()
        {
            m_Ability.m_ShieldAbility.ShieldStats.DestroyAnimation();
            m_Ability.AbilitiesHolder.PerformAbilityAgain();
            m_Ability.ShipEffects.EnableColliders(true);
        }

        public override void ForceStop(Shield_EnemyAbility _ability)
        {
            base.ForceStop(_ability);
            _ability.m_ShieldAbility.ForceStop();
        }

        public override void SetShieldValue(float _difficulty, Shield_EnemyAbility _ability)
        {
            base.SetShieldValue(_difficulty, _ability);
            // enable the damage detector of the shield
            _ability.m_ShieldAbility.ShieldStats.GetComponent<DamageDetector>().TakesDamage = true;
            // set the life of the shield based on the difficulty
            m_ShieldHealth = m_ShieldHealthRange.Lerp(_difficulty);
            // set up the shield stats
            m_ShieldStats = _ability.m_ShieldAbility.ShieldStats;
            m_ShieldStats.SetUp(m_ShieldHealth, this);
        }
    }
}