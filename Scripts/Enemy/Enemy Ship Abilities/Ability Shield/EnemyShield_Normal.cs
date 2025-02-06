using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    [CreateAssetMenu(menuName = "Enemy Abilities/Shield/Normal")]
    public class EnemyShield_Normal : EnemyShieldAbilityParent
    {
        public override void Perform(Shield_EnemyAbility _ability)
        {
            base.Perform(_ability);
            _ability.m_ShieldAbility.Activate();
            _ability.AbilitiesHolder.ResumeMovement();
            _ability.AbilitiesHolder.ResumeShooting();
            GameManager.i.GeneralValues.StartCoroutine(EndAbility(_ability));
        }

        private IEnumerator EndAbility(Shield_EnemyAbility _ability)
        {
            yield return new WaitForSeconds(ShieldDuration);
            _ability.AbilitiesHolder.PerformAbilityAgain();
            _ability.ShipEffects.EnableColliders(true);
        }

        public override void ForceStop(Shield_EnemyAbility _ability)
        {
            base.ForceStop(_ability);
            _ability.m_ShieldAbility.ForceStop();
        }
    }
}