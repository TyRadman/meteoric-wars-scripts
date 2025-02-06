using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    public class EnemyHealth : ShipHealth
    {
        [SerializeField] private EnemyStats m_Stats;
        [SerializeField] private EnemyComponents m_Components;

        private void OnEnable()
        {
            CurrentHealth = MaxHealth;
        }

        public override void TakeDamage(float _amount)
        {
            base.TakeDamage(_amount);

            if (!CanBeShot)
            {
                return;
            }

            if (m_Stats == null)
            {
                Debug.LogError($"Not stats at {gameObject.name}");
            }

            if (GameManager.i.EnemyHealthBar == null)
            {
                Debug.LogError($"Not Health at {gameObject.name}");
            }
            else
            {
                GameManager.i.EnemyHealthBar.UpdateHealthBar(CurrentHealth, MaxHealth, m_Components.EnemyName);
            }

            if (CurrentHealth <= 0f)
            {
                CanBeShot = false;
                m_Components.OnDeath();
            }
        }

        public void SetValues(ShipRankValues _values, float _defensiveDifficulty)
        {
            float maxHP = Mathf.Round(_values.HealthValue.Lerp(_defensiveDifficulty));

            if (GameManager.i != null)
            {
                if (GameManager.i.PlayersManager != null)
                {
                    maxHP *= GameManager.i.PlayersManager.Players.Count == 2 ? 1.5f : 1f;
                }
            }

            // setting health
            MaxHealth = (int)maxHP;
            CurrentHealth = (int)maxHP;
        }
    }
}