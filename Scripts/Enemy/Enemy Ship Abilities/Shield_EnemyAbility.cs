using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    public class Shield_EnemyAbility : EnemyAbility
    {
        [HideInInspector] public ShieldAbility m_ShieldAbility;
        private EnemyShieldAbilityParent m_SelectedStyle;
        [HideInInspector] public ShipComponenets ShipEffects;

        public override void AwakeAbility()
        {
            TheAbility = Instantiate(AbilityPrefab, transform).GetComponent<Ability>();
            m_ShieldAbility = (ShieldAbility)TheAbility;
            base.AwakeAbility();
            //SetUpAbility(Difficulty, StyleIndex);
        }

        public override void SetUpAbility(float _difficulty, int _abilityIndex = -1)
        {
            base.SetUpAbility(_difficulty, _abilityIndex);

            m_SelectedStyle = Instantiate((EnemyShieldAbilityParent)m_Style, transform);
            // set values based on difficulty
            m_SelectedStyle.SetShieldValue(_difficulty, this);
            // cache the ship effects, to be able to turn off its colliders
            ShipEffects = Ship.GetComponent<ShipComponenets>();
        }

        public override void PerformAbility()
        {
            base.PerformAbility();
            m_SelectedStyle.Perform(this);
        }

        public override void ForceStopAbility()
        {
            base.ForceStopAbility();
            m_SelectedStyle.ForceStop(this);
        }
    }
}