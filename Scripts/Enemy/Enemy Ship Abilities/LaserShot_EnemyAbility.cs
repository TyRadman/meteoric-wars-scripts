using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    public class LaserShot_EnemyAbility : EnemyAbility
    {
        private RedLaserAbility m_LaserAbility;
        [SerializeField] private GameObject m_AttackIndicator;
        [SerializeField] private EnemyLaserAbilityStyle m_SelectedStyle;
        private LaserIndicator m_Indicator;

        public override void AwakeAbility()
        {
            TheAbility = Instantiate(AbilityPrefab, transform).GetComponent<RedLaserAbility>();
            m_LaserAbility = (RedLaserAbility)TheAbility;
            // create and set up the laser indicator
            m_Indicator = Instantiate(m_AttackIndicator, transform).GetComponent<LaserIndicator>();
            m_Indicator.transform.localPosition = Vector2.zero;
            m_Indicator.transform.eulerAngles = transform.eulerAngles;
            base.AwakeAbility();
        }

        public override void SetUpAbility(float _difficulty, int _abilityIndex = -1)
        {
            base.SetUpAbility(_difficulty, _abilityIndex);
            // create the ability prefab
            m_SelectedStyle = Instantiate((EnemyLaserAbilityStyle)m_Style, transform);
            // set the durations of the styles
            m_SelectedStyle.SetUp(_difficulty, m_LaserAbility, transform.parent);
            // cache the indicator
            m_SelectedStyle.Indicator = m_Indicator;
        }

        public override void PerformAbility()
        {
            base.PerformAbility();

            m_SelectedStyle.Perform(m_Indicator, m_LaserAbility, AbilitiesHolder, transform.parent);
        }

        public override void ForceStopAbility()
        {
            base.ForceStopAbility();
            m_SelectedStyle.ForceStop();
        }
    }

    public enum LaserAbilityStyles
    {
        Normal = 0, PlayAim = 1, PersistentAim = 2, Swipe = 3, GodPersistentAim = 4
    }
}