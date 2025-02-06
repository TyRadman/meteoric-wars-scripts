using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    public class GMShootingBall : Ability
    {
        [System.Serializable]
        public struct GreenMatterBallLevel
        {
            public int ShotsNumber;
        }

        [SerializeField] private GreenMatterBallLevel[] m_Levels;
        [Header("Special References")]
        [SerializeField] private GameObject m_GreenMatterBallPrefab;
        private GreenMatterBall m_GreenMatterBall;
        private Transform m_PlayerTransform;

        public override void SetLevelValues(int _levelNumber)
        {
            base.SetLevelValues(_levelNumber);

            GreenMatterBallLevel level = m_Levels[_levelNumber];
            m_GreenMatterBall.SetShotsNumber(level.ShotsNumber);
        }


        public override void SetUp(Transform _ship = null)
        {
            m_GreenMatterBall = Instantiate(m_GreenMatterBallPrefab).GetComponent<GreenMatterBall>();
            m_GreenMatterBall.SetUp(_ship.GetComponent<Shooter>().UserIndex);
            m_PlayerTransform = _ship;
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
            m_GreenMatterBall.StartShooting(m_PlayerTransform.position);
            float duration = m_GreenMatterBall.GetActionDuration();
            Slot.StartAbilityUsageCountdown(duration);
            // start cooling down after shield usage ends
            Invoke(nameof(RechargeAbility), duration);
        }

        public override void RechargeAbility()
        {
            m_GreenMatterBall.DisableGraphics();
            base.RechargeAbility();
        }
    }
}