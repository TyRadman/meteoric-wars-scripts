using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpaceWar.UI.HUD;
using UnityEngine.Events;

namespace SpaceWar
{
    public class PlayerHealth : ShipHealth, IController
    {
        public System.Action<int, string> OnPlayerDeathAction { get; set; }
        public bool IsAlive = false;
        public float Resilliance = 0f;

        private PlayerComponents m_Components;
        private Coroutine m_RegeneratingCoroutine;
        private WaitForSeconds m_PreRegenerationWait;
        private WaitForSeconds m_RegenerationWait;
        private float m_MaxGeneratableHealthAmount;
        private float m_CurrentGeneratableHealth;
        private float m_HealthRegenPerSecond;
        private int m_PlayerLayer;

        #region Constants
        // total life percentage that can be generated
        private const float HEALTH_REGENERATING_PERCENTRAGE = 0.1f;
        // total time required to fill all possible regenerated health (in seconds)
        private const int PRE_REGENERATION_TIME = 2;
        private const float HEALTH_GENERATED_PER_SECOND = 3f;
        private const float BETWEEN_DAMAGE_REST_TIME = 1f;
        private const int SAFE_LAYER_MASK = 8;
        #endregion

        protected override void Awake()
        {
            m_PreRegenerationWait = new WaitForSeconds(PRE_REGENERATION_TIME);
            m_RegenerationWait = new WaitForSeconds(1f);
            m_PlayerLayer = gameObject.layer;
        }

        public void SetUp(IController components)
        {
            m_Components = (PlayerComponents)components;
            SetMaxHealth(m_Components.ShipInfo.MaxHealth);
        }

        #region IController
        public void Activate()
        {
            SetCurrentHealthToMaxHealth();
            GameManager.i.HUDManager.UpdatePlayerHealth(CurrentHealth / MaxHealth, m_Components.PlayerIndex, $"{CurrentHealth:0} / {MaxHealth:0}");
        }

        public void Deactivate()
        {

        }

        public void Dispose()
        {

        }
        #endregion

        public override void SetMaxHealth(float _maxHealth)
        {
            base.SetMaxHealth(_maxHealth);
            SetCurrentHealthToMaxHealth();
            GameManager.i.HUDManager.UpdatePlayerHealth(CurrentHealth / MaxHealth, m_Components.PlayerIndex, $"{CurrentHealth:0} / {MaxHealth:0}");
        }

        public void ResetHealth()
        {
            // sets the max health again
            SetCurrentHealthToMaxHealth();
            // updates the health bar
            GameManager.i.HUDManager.UpdatePlayerHealth(CurrentHealth / MaxHealth, m_Components.PlayerIndex, $"{CurrentHealth:0} / {MaxHealth:0}");
        }

        public override void TakeDamage(float _amount)
        {
            GameManager.i.AudioManager.PlayAudio(GameManager.i.GeneralValues.HurtAudio);

            if (!CanBeShot)
            {
                return;
            }

            // if the ship has resilience then reduce that from the damage
            if (Resilliance > 0f)
            {
                _amount = _amount < 0f ? _amount * Resilliance : _amount;
            }

            CurrentHealth -= (int)_amount;

            // the player safety zone is activated if there is damage and there is still health left for the player
            if (_amount < 0 && CurrentHealth > 0)
            {
                GameManager.i.DamageIndicator.PlayDamageAnimation();
                SwitchLayers(SAFE_LAYER_MASK);
                m_Components.Effects.ShipSpriteFlicker(GameManager.i.GeneralValues.PlayerValues.PostDamageProtectionDuration);
                Invoke(nameof(SwitchLayerBackToPlayer), GameManager.i.GeneralValues.PlayerValues.PostDamageProtectionDuration);
            }

            if (CurrentHealth <= 0f)
            {
                OnPlayerDeathAction?.Invoke(m_Components.PlayerIndex, m_Components.gameObject.name);

                m_Components.Stats.OnDeath();
                CanBeShot = false;
            }

            #region Regeneration
            // if it's a damage taken. This is for generating health over time when the ship is not shooting
            if (_amount < 0)
            {
                // if a hit is taken, then stop regenerating
                StopRegenerating();

                // if the health dropped a lot, then the regeneratable health amount will also drop
                if (m_CurrentGeneratableHealth - CurrentHealth > m_MaxGeneratableHealthAmount)
                {
                    m_CurrentGeneratableHealth = CurrentHealth + m_MaxGeneratableHealthAmount;
                    GameManager.i.HUDManager.UpdatePlayerRegeneratedHealthBar(m_CurrentGeneratableHealth / MaxHealth, m_Components.PlayerIndex);
                }
            }
            else
            {
                // if the health was increased to a point where it's more than the regeneratable health, then increase the regeneratable health amount
                if (CurrentHealth > m_CurrentGeneratableHealth)
                {
                    m_CurrentGeneratableHealth = CurrentHealth;
                    GameManager.i.HUDManager.UpdatePlayerRegeneratedHealthBar(m_CurrentGeneratableHealth / MaxHealth, m_Components.PlayerIndex);
                }
            }
            #endregion

            //print($"index is {Components.PlayerIndex}. The health amount t: {CurrentHealth}/{MaxHealth}={CurrentHealth / MaxHealth}".Color(Color.green));
            GameManager.i.HUDManager.UpdatePlayerHealth(CurrentHealth / MaxHealth, m_Components.PlayerIndex, $"{CurrentHealth:0} / {MaxHealth:0}");
        }

        public override void AddHealth(float _amount)
        {
            base.AddHealth(_amount);

            GameManager.i.HUDManager.UpdatePlayerHealth(CurrentHealth / MaxHealth, m_Components.PlayerIndex, $"{CurrentHealth:0} / {MaxHealth:0}");
        }

        private void SwitchLayers(int _layer)
        {
            gameObject.layer = _layer;
        }

        private void SwitchLayerBackToPlayer()
        {
            gameObject.layer = m_PlayerLayer;
        }

        private IEnumerator RegeneratingProcess()
        {
            yield return m_PreRegenerationWait;
            int loops = (int)((m_CurrentGeneratableHealth - CurrentHealth) / HEALTH_GENERATED_PER_SECOND);
            // rounding up with ints might leave some regeneratable health unregenerated. So we must do it manually after the for loop
            float differenceHealth = m_CurrentGeneratableHealth - CurrentHealth - loops * HEALTH_GENERATED_PER_SECOND;

            for (int i = 0; i < loops; i++)
            {
                AddHealth(HEALTH_GENERATED_PER_SECOND);
                PointsPopUp.i.CallText(transform.position, PointsPopUp.PopUpTextType.Life, HEALTH_GENERATED_PER_SECOND);
                yield return m_RegenerationWait;
            }

            if (differenceHealth > 0)
            {
                AddHealth(differenceHealth);
                PointsPopUp.i.CallText(transform.position, PointsPopUp.PopUpTextType.Life, differenceHealth);
            }
        }

        public void StopRegenerating()
        {
            // if there is a regeneration taking place, stop it
            if (m_RegeneratingCoroutine != null)
            {
                StopCoroutine(m_RegeneratingCoroutine);
            }

            // if there is a need to regenerate, start a count down for it
            if (m_CurrentGeneratableHealth > CurrentHealth && IsAlive)
            {
                m_RegeneratingCoroutine = StartCoroutine(RegeneratingProcess());
            }
        }

        public void StopRegeneration()
        {
            if (m_RegeneratingCoroutine != null) StopCoroutine(m_RegeneratingCoroutine);
        }

        public float GetHealthNeededToMax()
        {
            return MaxHealth - CurrentHealth;
        }

        public override void SetCurrentHealthToMaxHealth()
        {
            base.SetCurrentHealthToMaxHealth();
            m_CurrentGeneratableHealth = CurrentHealth;
            m_MaxGeneratableHealthAmount = MaxHealth * HEALTH_REGENERATING_PERCENTRAGE;
            GameManager.i.HUDManager.UpdatePlayerRegeneratedHealthBar(m_CurrentGeneratableHealth / MaxHealth, m_Components.PlayerIndex);
        }

        public void CanTakeDamage(bool enable)
        {
            CanBeShot = enable;
        }
    }
}