using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SpaceWar
{
    public abstract class ShipHealth : MonoBehaviour, IDamagable
    {
        [SerializeField] protected float CurrentHealth;
        [SerializeField] protected float MaxHealth;
        [SerializeField] protected bool CanBeShot = true;
        public System.Action OnDeathAction { get; set; }

        protected virtual void Awake()
        {
            SetCurrentHealthToMaxHealth();
        }

        public virtual void TakeDamage(float _amount)
        {
            if (!CanBeShot)
            {
                return;
            }

            CurrentHealth -= _amount;

            if (CurrentHealth <= 0f)
            {
                CurrentHealth = 0;
                // death 
                OnDeathAction?.Invoke();
            }
        }

        public virtual void AddHealth(float _amount)
        {
            CurrentHealth += _amount;

            if (CurrentHealth > MaxHealth)
            {
                CurrentHealth = MaxHealth;
            }
        }

        public virtual void SetCurrentHealthToMaxHealth()
        {
            CurrentHealth = MaxHealth;
            CanBeShot = true;
        }

        public virtual void SetMaxHealth(float _maxHealth)
        {
            MaxHealth = (int)_maxHealth;
        }

        public virtual void SetMaxAndCurrentHealth(float _maxHealth, float _currentHealth)
        {
            MaxHealth = _maxHealth;
            CurrentHealth = _currentHealth;
        }

        public float GetHealth()
        {
            return CurrentHealth;
        }

        public float GetHealthT()
        {
            return CurrentHealth / MaxHealth;
        }

        public float GetMaxHealth()
        {
            return MaxHealth;
        }
    }
}