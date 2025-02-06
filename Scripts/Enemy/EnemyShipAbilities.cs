using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    /// <summary>
    /// The controller and performer of the enemies' abilities
    /// </summary>
    public class EnemyShipAbilities : MonoBehaviour, IController
    {
        private List<EnemyAbility> m_Abilities = new List<EnemyAbility>();
        private List<EnemyAbilityPerformer> m_AbilitiesOwned = new List<EnemyAbilityPerformer>();
        private Transform m_AbilitiesHolder;
        private EnemyAbility m_SelectedAbility;
        private EnemyComponents m_Components;
        // values that can be adjusted based on difficulty
        private Vector2 m_SpecialAttackFrequency;

        public void SetUp(IController components)
        {
            if(components is not EnemyComponents enemyComponents)
            {
                LogsManager.LogWrongType(typeof(EnemyComponents).Name, components.GetType().Name);
                return;
            }

            m_Components = components as EnemyComponents;
            m_Components.HasAbility = true;
        }

        /// <summary>
        /// Sets up and adds an ability to the ship
        /// </summary>
        /// <param name="_info"></param>
        /// <param name="_ability"></param>
        /// <param name="_styleIndex"></param>
        public void SetUpValues(SpecialAttackValues _info, EnemyAbilityPrefab _ability, int _styleIndex = -1)
        {
            // setting the abilities
            EnemyAbility ability = Instantiate(_ability.Prefab, transform).GetComponent<EnemyAbility>();
            ability.transform.parent = transform;
            ability.transform.localPosition = Vector2.zero;
            ability.transform.eulerAngles = transform.eulerAngles;
            ability.AbilitiesHolder = this;
            m_Abilities.Add(ability);
            ability.gameObject.SetActive(true);
            ability.StyleIndex = _styleIndex;
            ability.AbilityFrequencyRange = _info.Frequency;
            ability.AwakeAbility();
            ability.SetUpAbility(1, ability.StyleIndex);
        }

        public void SetUpValues(EnemyAbilityPerformer abilityPerformer)
        {
            EnemyAbilityPerformer ability = Instantiate(abilityPerformer);
            ability.SetUp(m_Components);
        }

        public void ClearAbilities()
        {
            for (int i = 0; i < m_Abilities.Count; i++)
            {
                Destroy(m_Abilities[i].gameObject);
            }

            m_Abilities.Clear();
        }

        /// <summary>
        /// Starts the ship's performance for the ability
        /// </summary>
        public void ActivateShip()
        {
            m_SelectedAbility = m_Abilities[Random.Range(0, m_Abilities.Count)];
            Invoke(nameof(ActivateSpecialAttack), m_SelectedAbility.AbilityFrequencyRange.RandomValue());
        }

        /// <summary>
        /// Happens a couple of times during the ship's lifecycle
        /// </summary>
        private void ActivateSpecialAttack()
        {
            m_Components.Movement.StopMovement();
            m_Components.ShootingMethod.StopShooting();
            m_SelectedAbility.PerformAbility();
        }

        // these must be called by the end of the ability. The order depends on the ability
        #region After math calls
        public void ResumeMovement()
        {
            m_Components.Movement.PerformMovement(transform);
        }

        public void ResumeShooting()
        {
            m_Components.ShootingMethod.PerformShooting();
        }

        public void PerformAbilityAgain()
        {
            CancelInvoke();
            m_SelectedAbility = m_Abilities[Random.Range(0, m_Abilities.Count)];
            Invoke(nameof(ActivateSpecialAttack), m_SelectedAbility.AbilityFrequencyRange.RandomValue());
        }
        #endregion

        public void StopSpecialAttackQueue()
        {
            m_Abilities.ForEach(a => a.ForceStopAbility());
            CancelInvoke();
        }

        #region
        public void Activate()
        {

        }

        public void Deactivate()
        {

        }

        public void Dispose()
        {

        }
        #endregion
    }
}