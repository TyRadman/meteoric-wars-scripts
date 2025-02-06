using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.ComponentModel;
using System.Reflection;

namespace SpaceWar
{
    public class Ability : MonoBehaviour, IAbility
    {
        public enum AbilityLevelUpType
        {
            Upgradable, Addaditve, None
        }

        public enum AbilityUsage
        {
            Usable, Effect, None
        }

        public int Price = 0;
        public int Amount = 0;
        [HideInInspector] public float CurrentAmount;
        public AbilityLevelUpType UpgradeType = AbilityLevelUpType.Upgradable;
        public AbilityUsage TheAbilityUsage = AbilityUsage.Usable;
        protected static float LeastUsageTime = 0.1f;
        [HideInInspector] public int AbilityLevel = -1;
        public AbilityTag Tag;
        [Header("Visuals")]
        public string Name;
        [TextArea(2, 10)] public string Description;
        public int CountDownTime = -1;
        public Sprite Image;
        [HideInInspector] public PlayerAbilityUIElement Slot;
        public bool IsAvailable = true;
        [Header("Levels Variables")]
        public List<AbilityInformation> AbilityInfo = new List<AbilityInformation>();
        public bool HasSlot = true;

        protected virtual void Awake()
        {
            AbilityLevel = -1;
        }

        public virtual void SetUp(Transform _ship = null)
        {
            if (UpgradeType != AbilityLevelUpType.Upgradable) return;

            SetLevelValues(AbilityLevel);
        }

        public void SetSlot(PlayerAbilityUIElement _slot)
        {
            if (UpgradeType == AbilityLevelUpType.Addaditve)
            {
                CurrentAmount += Amount;
            }
            else if (UpgradeType == AbilityLevelUpType.Upgradable)
            {
                AbilityLevel++;
            }

            Slot = _slot;
            Slot.IconImage.sprite = Image;
            Slot.ActivateSlot(true, AbilityLevel, TheAbilityUsage, UpgradeType, CurrentAmount.ToString());
        }

        public virtual void SetLevelValues(int _levelNumber)
        {

        }

        public virtual void UpdateAbility()
        {
            if (UpgradeType == AbilityLevelUpType.Upgradable)
            {
                SetLevelValues(AbilityLevel);
            }
        }

        /// <summary>
        /// Starts the countdown after which the ability is ready to use again. Call this when the ability is done being used.
        /// </summary>
        public virtual void RechargeAbility()
        {
            Slot.ActivateAbility(CountDownTime);
            // starts the countdown of enabling the ability
            Invoke(nameof(Deactivate), CountDownTime);
        }

        /// <summary>
        /// Called when the ability expires for whatever reason. Used when the amount of the ability ends or the ability is replaced.
        /// </summary>
        public virtual void RemoveAbility()
        {
            Slot.PlayerAbility.RemoveAbility(Tag);
        }

        public void AddAmount(float _amount)
        {
            CurrentAmount += _amount;
            Slot.SetAmount(CurrentAmount.ToString());

            if (CurrentAmount <= 0)
            {
                CurrentAmount = 0;
                RemoveAbility();
                Slot.SetAmount(string.Empty);
            }
        }

        public virtual void Activate()
        {

        }

        public virtual void ForceStop()
        {

        }

        public virtual void Dispose()
        {

        }

        public virtual void Deactivate()
        {
            IsAvailable = true;
        }

        protected virtual IEnumerator AbilityActivationProcess()
        {
            yield return null;
        }
    }

    public enum AbilityTag
    {
        Shield, Dash, Charge, Drones, CRRockets, CRTransformation, ASCrystalShiled, ASCrystalShots, GMShootingBall, GMExplodingBall,
        HealthPoints, ChasingRocketsShotgun, RedLaser, DFCrazyDrone, Skip, EnemyLaserShot, OrangeLaser, SummonShip, BPSuperShots,
        BPSuperLaser, None, DFSwarmAttack,
    }

    [System.Serializable]
    public struct AbilityInformation
    {
        public string Name;
        public string[] Values;
        public string ValueType;
        public Color ValueColor;
    }
}