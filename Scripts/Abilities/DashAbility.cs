using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    public class DashAbility : Ability
    {
        [SerializeField] private List<DashLevel> m_Levels;
        [Header("Special Variables")]
        [SerializeField] private AnimationCurve m_AccelerationCurve;
        [SerializeField] private float m_DashDistance = 5f;
        [SerializeField] private float m_DashingDuration = 0.5f;
        [SerializeField] private float m_AfterDashProtectionTime = 0.1f;
        private PlayerMovement m_Movement;
        private WaitForSeconds m_AfterDashWait;
        [Header("Effects")]
        [SerializeField] private GameObject m_DashingParticlesPrefab;
        [SerializeField] private GameObject m_DashingTrailPrefab;
        private ParticleSystem m_DashingParticles;
        private TrailRenderer m_DashingTrail;
        [SerializeField] private LayerMask m_Hittable;
        [SerializeField] private LayerMask m_PlayersLayer;
        [SerializeField] private LayerMask m_IgnoredLayer;

        [System.Serializable]
        public struct DashLevel
        {
            public int CoolDownTime;
            public float AfterDashProtectionTime;
        }

        public override void SetUp(Transform _ship = null)
        {
            base.SetUp(_ship);
            // cache the player movement
            m_Movement = _ship.GetComponent<PlayerMovement>();
            // create instances of the dashing effects
            m_DashingParticles = Instantiate(m_DashingParticlesPrefab, _ship).GetComponent<ParticleSystem>();
            m_DashingTrail = Instantiate(m_DashingTrailPrefab, _ship).GetComponent<TrailRenderer>();
            m_DashingTrail.emitting = false;
        }

        public override void Activate()
        {
            base.Activate();

            // we cache the last direction the player used
            if (IsAvailable)
            {
                if (m_Movement.StoreLastDirections())
                {
                    StartCoroutine(dashingProcess());
                    IsAvailable = false;
                }
            }
        }

        private IEnumerator dashingProcess()
        {
            float duration = m_DashingDuration;
            Transform ship = m_Movement.transform;
            Vector2 startPosition = ship.position;
            Vector2 direction = m_Movement.GetDirection();
            RaycastHit2D hit = Physics2D.Raycast(ship.position, direction, m_DashDistance, m_Hittable);
            float radian = Mathf.Atan2(direction.y, direction.x);
            Vector2 endPosition = startPosition + m_DashDistance * new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));

            // if the ray detected a wall, then the dash goes there
            if (hit.collider != null)
            {
                float t = (Mathf.InverseLerp(startPosition.x, endPosition.x, hit.point.x) + (Mathf.InverseLerp(startPosition.y, endPosition.y, hit.point.y))) / 2;
                duration = Mathf.Abs(Mathf.Lerp(0f, duration, t));
                endPosition = hit.point;
            }

            // starts the countdown of the ability duration
            Slot.StartAbilityUsageCountdown(duration);
            // disables the collider of the player so that no damage is taken
            m_Movement.gameObject.layer = (int)Mathf.Log(m_IgnoredLayer.value, 2);
            // m_PlayerBoxCollider.enabled = false;
            // start playing the particles
            m_DashingParticles.Play();
            // enable line renderer
            m_DashingTrail.emitting = true;
            // the process of the dash
            m_Movement.PerformDash(duration, m_AccelerationCurve, endPosition);
            yield return new WaitForSeconds(duration);
            // disables the trail
            m_DashingTrail.emitting = false;
            // waits for bit before enable the player's collider to take damage
            yield return m_AfterDashWait;
            // enables the player's collider
            m_Movement.gameObject.layer = (int)Mathf.Log(m_PlayersLayer.value, 2);
            // starts the countdown of the UI
            RechargeAbility();
        }

        public override void SetLevelValues(int _levelNumber)
        {
            base.SetLevelValues(_levelNumber);

            DashLevel level = m_Levels[_levelNumber];
            m_AfterDashProtectionTime = level.AfterDashProtectionTime;
            m_AfterDashWait = new WaitForSeconds(m_AfterDashProtectionTime);
            CountDownTime = level.CoolDownTime;
        }
    }
}