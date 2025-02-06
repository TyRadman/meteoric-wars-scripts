using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    public class HealthPointsAbility : Ability
    {
        private PlayerHealth m_Health;

        public override void SetUp(Transform _ship = null)
        {
            base.SetUp(_ship);
            m_Health = _ship.GetComponent<PlayerHealth>();
            Slot.SetAmount(CurrentAmount.ToString());
        }

        public override void Activate()
        {
            base.Activate();
            float healthNeeded = m_Health.GetHealthNeededToMax();

            if (!IsAvailable || healthNeeded == 0)
            {
                return;
            }

            IsAvailable = false;

            // if the health needed to max the ship's health is greater than the amount that the ability holds then, fill the whole amount and remove the ability
            if (healthNeeded >= CurrentAmount)
            {
                m_Health.AddHealth(CurrentAmount);
                PointsPopUp.i.CallText(m_Health.transform.position, PointsPopUp.PopUpTextType.Life, CurrentAmount);
                AddAmount(-CurrentAmount);
            }
            // otherwise, add the amount needed and subtract it from the total amount available in the ability
            else
            {
                AddAmount(-healthNeeded);
                m_Health.AddHealth(healthNeeded);
                PointsPopUp.i.CallText(m_Health.transform.position, PointsPopUp.PopUpTextType.Life, healthNeeded);
                Slot.SetAmount(CurrentAmount.ToString());
            }

            if (CurrentAmount > 0)
            {
                Slot.StartAbilityUsageCountdown(LeastUsageTime);
                Invoke(nameof(RechargeAbility), LeastUsageTime);
            }
        }
    }
}