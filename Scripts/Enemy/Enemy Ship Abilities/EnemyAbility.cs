using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    /// <summary>
    /// Each child of this class must have a prefab of the ability that it must instantiate at start and set it to TheAbility variable in this class.
    /// The ability is always a child of the ship
    /// </summary>
    public class EnemyAbility : MonoBehaviour
    {
        [HideInInspector] public EnemyShipAbilities AbilitiesHolder;
        [HideInInspector] public Ability TheAbility;
        public GameObject AbilityPrefab;
        public Vector2 AbilityFrequencyRange;
        [HideInInspector] public Transform Ship;
        [SerializeField] private EnemyAbilityTag m_AbilityTag;
        protected EnemyAbilityPerformer m_Style;
        [HideInInspector] public float Difficulty;
        [HideInInspector] public int StyleIndex = -1;

        public virtual void AwakeAbility()
        {
            Ship = transform.parent;
            Difficulty = Ship.GetComponent<EnemyStats>().Difficulty;
            TheAbility.HasSlot = false;
            TheAbility.SetUp(Ship);
        }

        // the enemy ability needs an ability to perform. It's basically a user of an ability that a player can use
        public virtual void SetUpAbility(float _difficulty, int _abilityIndex = -1)
        {
            EnemyAbilityPerformer style;

            if (_abilityIndex == -1)
            {
                style = EnemyAbilitiesManager.i.AbilitiesStyles.Find(a => a.Tag == m_AbilityTag).Styles.RandomItem();
            }
            else
            {
                style = EnemyAbilitiesManager.i.AbilitiesStyles.Find(a => a.Tag == m_AbilityTag).Styles[_abilityIndex];
            }

            m_Style = style;
        }

        public virtual void PerformAbility()
        {

        }

        /// <summary>
        /// Stops any thing that the ability would've done before the ship was destroyed
        /// </summary>
        public virtual void ForceStopAbility()
        {
            TheAbility.ForceStop();
        }
    }
}