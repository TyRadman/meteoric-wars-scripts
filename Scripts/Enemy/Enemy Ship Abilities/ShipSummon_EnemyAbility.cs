using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    public class ShipSummon_EnemyAbility : EnemyAbility
    {
        [SerializeField] private SummoningAbilityStyle m_SelectedStyle;

        public override void AwakeAbility()
        {
            TheAbility = Instantiate(AbilityPrefab, transform).GetComponent<SpawnShipAbility>();
            Ship = transform.parent;
            Difficulty = Ship.GetComponent<EnemyStats>().Difficulty;
            TheAbility.HasSlot = false;
            // we avoid setting up the ability to set it up later where we control the type of ship we can summon
            //TheAbility.SetUp(Ship);
            //base.AwakeAbility();
        }

        public override void SetUpAbility(float _difficulty, int _abilityIndex = -1)
        {
            base.SetUpAbility(_difficulty, _abilityIndex);

            m_SelectedStyle = Instantiate((SummoningAbilityStyle)m_Style, transform);
            // set the rank of the ships to be spawned
            SpawnShipAbility ability = (SpawnShipAbility)TheAbility;
            ability.SetSpawnedShipsRank(m_SelectedStyle.SummonedShipRank);
            // then set up the ability
            TheAbility.SetUp(Ship);
            // set values based on difficulty
            m_SelectedStyle.SetUpValues();
        }

        public override void PerformAbility()
        {
            //TheAbility.SetUp(Ship);
            base.PerformAbility();
            m_SelectedStyle.Perform(this);
        }

        public void ResumeMovement()
        {
            Invoke(nameof(ResumeTheMovement), 1f);
        }

        public override void ForceStopAbility()
        {
            base.ForceStopAbility();
        }

        private void ResumeTheMovement()
        {
            AbilitiesHolder.ResumeMovement();
            AbilitiesHolder.ResumeShooting();
            AbilitiesHolder.PerformAbilityAgain();
        }
    }
}