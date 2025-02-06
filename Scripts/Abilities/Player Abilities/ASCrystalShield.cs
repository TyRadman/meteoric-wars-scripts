using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    public class ASCrystalShield : Ability
    {
        [System.Serializable]
        public struct ASCrystalShieldLevels
        {
            public float Duration;
            public float DamageTaken;
            public int BulletsNumber;
        }

        [SerializeField] private ASCrystalShieldLevels[] m_Levels;
        [Header("Special references")]
        [SerializeField] private GameObject m_ShinyPrefab;
        private GameObject m_ShinyObject;
        private BulletsReflector m_Reflector;
        private PolygonCollider2D m_PlayerCollider;
        private float m_Duration;
        [SerializeField] private int m_NewLayerIndex;
        private LayerMask m_OldLayer;
        private GameObject m_Ship;


        public override void SetLevelValues(int _levelNumber)
        {
            base.SetLevelValues(_levelNumber);

            ASCrystalShieldLevels level = m_Levels[_levelNumber];
            m_Duration = level.Duration;
            m_Reflector.SetValues(level.DamageTaken, level.BulletsNumber);
        }

        public override void SetUp(Transform _ship = null)
        {
            m_ShinyObject = Instantiate(m_ShinyPrefab, _ship);
            m_Reflector = m_ShinyObject.GetComponent<BulletsReflector>();
            m_PlayerCollider = _ship.GetComponent<PolygonCollider2D>();
            m_Ship = _ship.gameObject;
            m_OldLayer = m_Ship.layer;
            // set reflector values that won't be changing when leveling up
            m_Reflector.ShipComponents = _ship.GetComponent<PlayerComponents>();
            m_ShinyObject.layer = _ship.gameObject.layer;
            m_ShinyObject.SetActive(false);

            // call the base method last because it needs to have the reflector cache before setting up level values
            base.SetUp(_ship);
        }

        public override void Activate()
        {
            base.Activate();

            if (!IsAvailable)
            {
                return;
            }

            IsAvailable = false;

            m_PlayerCollider.isTrigger = false;
            m_ShinyObject.SetActive(true);
            StartCoroutine(m_Reflector.Shine());
            m_Ship.layer = m_NewLayerIndex;
            // show usage remaining time
            Slot.StartAbilityUsageCountdown(m_Duration);
            Invoke(nameof(deactivateShield), m_Duration);
        }

        private void deactivateShield()
        {
            RechargeAbility();
            m_Ship.layer = m_OldLayer;
            m_PlayerCollider.enabled = true;
            m_ShinyObject.SetActive(false);
        }
    }
}