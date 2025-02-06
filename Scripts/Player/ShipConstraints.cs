using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar.Unit
{
    public class ShipConstraints : MonoBehaviour, IController
    {
        private PlayerComponents m_Components;
        private List<Constraints> m_currentConstraints = new List<Constraints>();
        private Constraints m_currentConstraint = Constraints.None;
        public bool IsActive { get; private set; }

        public void SetUp(IController components)
        {
            m_Components = (PlayerComponents)components;
        }


        public void ApplyConstraints(bool enable, Constraints constraints)
        {
            if (enable)
            {
                m_currentConstraints.Add(constraints);
            }
            else
            {
                m_currentConstraints.Remove(constraints);
            }

            m_currentConstraint = GetCurrentConstraint();

            m_Components.Movement.Enable((m_currentConstraint & Constraints.Movement) == 0);
            m_Components.ThePlayerShooting.Enable((m_currentConstraint & Constraints.Shooting) == 0);
            m_Components.Health.CanTakeDamage((m_currentConstraint & Constraints.TakingDamage) == 0);
            //_components.SuperAbilities.EnableAbility((_currentConstraint & AbilityConstraint.SuperAbility) == 0);
        }

        private Constraints GetCurrentConstraint()
        {
            Constraints con = Constraints.None;

            for (int i = 0; i < m_currentConstraints.Count; i++)
            {
                con |= m_currentConstraints[i];
            }

            return con;
        }

        #region IController
        public void Activate()
        {
            IsActive = true;
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        public void Dispose()
        {
            IsActive = false;
        }
        #endregion
    }


    [System.Flags]
    public enum Constraints
    {
        None = 0,
        Movement = 1,
        Shooting = 2,
        TakingDamage = 4,
        SuperAbility = 8
    }
}
