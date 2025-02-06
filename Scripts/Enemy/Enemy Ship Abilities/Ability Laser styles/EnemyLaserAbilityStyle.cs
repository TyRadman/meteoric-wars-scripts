using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    public class EnemyLaserAbilityStyle : EnemyAbilityPerformer
    {
        public GameObject AbilityPrefab;
        public LaserIndicator Indicator;
        public Vector2 ShootingTimeRange;
        public Vector2 WarningTimeRange;
        public float WarningTime;
        public float Difficulty;

        [HideInInspector] public Ability TheAbility;
        [HideInInspector] public float ShootingTime;

        // we must cache the action, otherwise, stopping all coroutines when the ship is destroyed would stop all other active ships
        protected Coroutine ActionCoroutine;
        protected Transform Ship;

        [SerializeField] private GameObject m_AttackIndicator;
        [SerializeField] private EnemyLaserAbilityStyle m_SelectedStyle;
        
        private LaserIndicator m_Indicator;
        private RedLaserAbility m_LaserAbility;
        private ShipComponenets m_Components;

        public override void SetUp(ShipComponenets components)
        {
            m_Components = components;

            TheAbility = Instantiate(AbilityPrefab, m_Components.transform).GetComponent<RedLaserAbility>();
            m_LaserAbility = (RedLaserAbility)TheAbility;

            // create and set up the laser indicator
            m_Indicator = Instantiate(m_AttackIndicator, m_Components.transform).GetComponent<LaserIndicator>();
            m_Indicator.transform.localPosition = Vector2.zero;
            m_Indicator.transform.eulerAngles = m_Components.transform.eulerAngles;

            Ship = m_Components.transform.parent;
            Difficulty = Ship.GetComponent<EnemyStats>().Difficulty;
            TheAbility.HasSlot = false;
            TheAbility.SetUp(Ship);
        }

        public override void Perform()
        {

        }

        public virtual void Perform(LaserIndicator _indicator, RedLaserAbility _ability, EnemyShipAbilities _holder, Transform _ship)
        {

        }

        /// <summary>
        /// Stops the laser shooting process (must be perfected)Shoots a laser beam that hits all enemies on its way.
        /// </summary>
        public virtual void ForceStop()
        {
            if (ActionCoroutine != null)
            {
                GameManager.i.GeneralValues.StopCoroutine(ActionCoroutine);
            }

            GameManager.i.GeneralValues.StopAllCoroutines();
            Indicator.StopIndicator();
        }

        public virtual void SetUp(float _difficulty, RedLaserAbility _ability, Transform _ship)
        {
            WarningTime = WarningTimeRange.Lerp(_difficulty);
            ShootingTime = ShootingTimeRange.Lerp(_difficulty);
            _ability.SetLaserDuration(ShootingTime);
        }
    }
}