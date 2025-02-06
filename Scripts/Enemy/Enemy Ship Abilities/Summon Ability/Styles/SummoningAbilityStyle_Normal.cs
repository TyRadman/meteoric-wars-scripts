using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    [CreateAssetMenu(menuName = "Enemy Abilities/Summon/Normal")]
    public class SummoningAbilityStyle_Normal : SummoningAbilityStyle
    {
        public override void SetUpValues()
        {
            base.SetUpValues();
        }

        public override void Perform(ShipSummon_EnemyAbility _enemyAbility)
        {
            base.Perform(_enemyAbility);
            // activate the skill
            _enemyAbility.TheAbility.Activate();
            // resume the ship's movement afterwards
            _enemyAbility.ResumeMovement();
        }
    }
}