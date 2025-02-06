using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    public class ShieldAbility : Ability
    {
        [SerializeField] private float m_ShieldDuration = 3f;
        [SerializeField] private GameObject m_ShieldPrefab;
        private GameObject m_Shield;
        public ShieldStats ShieldStats;
        [SerializeField] private List<ShieldLevel> m_Levels;

        [System.Serializable]
        public struct ShieldLevel
        {
            public int Duration;
            public int CoolDownTime;
        }

        protected override void Awake()
        {
            base.Awake();

            // creating the shield object in the game
            m_Shield = Instantiate(m_ShieldPrefab);
            ShieldStats = m_Shield.transform.GetChild(0).GetComponent<ShieldStats>();
        }

        public override void SetUp(Transform _ship)
        {
            base.SetUp(_ship);
            SetShieldParent(_ship);
            SetShieldSize(_ship);
            ShieldStats.tag = _ship.tag;

        }

        public override void Activate()
        {
            base.Activate();

            if (!IsAvailable)
            {
                return;
            }

            // we don't need to do all this if there are no slots i.e. if the ability is used by an enemy
            if (HasSlot)
            {
                IsAvailable = false;
                // show usage remaining time
                Slot.StartAbilityUsageCountdown(m_ShieldDuration);
                // start cooling down after shield usage ends
                Invoke(nameof(RechargeAbility), m_ShieldDuration);
            }
            // if the duration is zero, then the shield must be destroyed
            else if (m_ShieldDuration > 0)
            {
                Invoke(nameof(HideShield), m_ShieldDuration);
            }

            // activate the shield
            ShieldStats.ShowShield(true);
        }

        public override void RechargeAbility()
        {
            ShieldStats.ShowShield(false);
            base.RechargeAbility();
        }

        public override void SetLevelValues(int _levelNumber)
        {
            base.SetLevelValues(_levelNumber);

            ShieldLevel level = m_Levels[_levelNumber];
            m_ShieldDuration = level.Duration;
            CountDownTime = level.CoolDownTime;
        }

        public override void ForceStop()
        {
            base.ForceStop();
            ShieldStats.ShowShield(false);
        }

        #region Shield Functionality
        public void HideShield()
        {
            ShieldStats.ShowShield(false);
        }

        public void SetShieldParent(Transform _parent)
        {
            m_Shield.transform.parent = _parent;
            m_Shield.transform.localPosition = Vector2.zero;
        }

        public void SetShieldSize(Transform _parent)
        {
            float size = Helper.GetPolygonColliderSize(_parent.GetComponent<PolygonCollider2D>()) * 1.5f;
            m_Shield.transform.localScale = new Vector2(size, size);
        }

        public void SetShieldDuration(float _duration)
        {
            m_ShieldDuration = _duration;
        }
        #endregion
    }
}