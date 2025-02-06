using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    [CreateAssetMenu(menuName = "Enemy Abilities/Laser/Normal")]
    public class LaserAbilityStyle_Normal : EnemyLaserAbilityStyle
    {
        public override void Perform(LaserIndicator _indicator, RedLaserAbility _ability, EnemyShipAbilities _holder, Transform _ship)
        {
            base.Perform(_indicator, _ability, _holder, _ship);
            ActionCoroutine = GameManager.i.GeneralValues.StartCoroutine(performanceProcess(_indicator, _ability, _holder, _ship));
        }

        private IEnumerator performanceProcess(LaserIndicator _indicator, RedLaserAbility _ability, EnemyShipAbilities _holder, Transform _ship)
        {
            // start indicator flashing
            Indicator.transform.localPosition = Vector2.zero;
            Indicator.StartIndicator(WarningTime);
            yield return new WaitForSeconds(WarningTime);
            _ability.Activate();
            yield return new WaitForSeconds(_ability.GetWeapon().ShotDuration + 1f);
            _holder.ResumeMovement();
            _holder.ResumeShooting();
            _holder.PerformAbilityAgain();
        }
    }
}