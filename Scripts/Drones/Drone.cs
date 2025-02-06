using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    public class Drone : MonoBehaviour
    {
        [SerializeField] private Transform m_Parent;
        private Transform m_Target;
        [SerializeField] private Vector2 m_TargetAvailabilityTimeRange;
        [SerializeField] private float m_HeightMovement = 0.5f;
        [SerializeField] private float m_HeightMovementFrequency = 1f;
        [SerializeField] private float m_SnappingToParentSpeed = 0.1f;
        [HideInInspector] public bool IsActive = true;
        [SerializeField] private float m_AroundTheShipArea = 1.5f;
        [SerializeField] private bool m_StartMovingAutomatically = true;
        private readonly Vector2 m_FrequencyRandomizationValues = new Vector2(0.7f, 1.3f);
        private DroneGraphics m_Graphics;
        private const float SHOWING_SCALE = 2f;
        private const float APPEARING_PARTICLES_HALF_DURATION = 0.5f;

        private void Awake()
        {
            m_Graphics = GetComponent<DroneGraphics>();
            m_Graphics.EnableGraphics(false);

            // randomize the up and down movement values
            SetHeightMovementFrequency(m_HeightMovementFrequency);

            if (m_Parent != null)
            {
                setUp();
            }
        }

        private void setUp()
        {
            m_Target = new GameObject("Drone Target").transform;
            m_Target.parent = m_Parent;

            if (m_StartMovingAutomatically)
            {
                StartMovement(1f);
            }
        }

        #region External Movement Methods
        public void StartMovement(float _delay)
        {
            Invoke(nameof(startMovementProcess), _delay);
        }

        public void StopMovement()
        {
            IsActive = false;
        }
        #endregion


        #region Movement Related Methods
        private void startMovementProcess()
        {
            setTargetPosition();
            StartCoroutine(movementProcess());
        }

        public void SetParent(Transform _parent)
        {
            m_Parent = _parent;
            setUp();
        }

        /// <summary>
        /// Gives the drone new position around its parent every once in a while
        /// </summary>
        private void setTargetPosition()
        {
            Vector2 newPos = Random.insideUnitCircle * m_AroundTheShipArea + (Vector2)m_Parent.position;
            m_Target.position = newPos;

            if (!IsActive)
            {
                return;
            }

            Invoke(nameof(setTargetPosition), m_TargetAvailabilityTimeRange.RandomValue());
        }

        /// <summary>
        /// Makes the drone follow its parent with some up and down motion
        /// </summary>
        /// <returns></returns>
        private IEnumerator movementProcess()
        {
            Vector2 position = transform.position;

            while (IsActive)
            {
                // snapping to the player
                position = Vector2.Lerp(position, m_Target.position, m_SnappingToParentSpeed);
                // move the drone up and down
                position.y += Mathf.Sin(Time.time * m_HeightMovementFrequency) * m_HeightMovement;
                // applying new position to the drone
                transform.localPosition = position;

                yield return null;
            }
        }
        #endregion

        #region Appear/Disappear Methods
        public void ShowDrone(Vector2 _position, ParticlePoolTag _particleTag = ParticlePoolTag.DroneSpawnParticle, bool _randomize = true)
        {
            Vector2 pos;
            IsActive = true;

            if (_randomize)
            {
                pos = Random.insideUnitCircle * SHOWING_SCALE + _position;
            }
            else
            {
                pos = _position;
            }

            m_Graphics.EnableGraphics(false);
            transform.position = pos;
            PoolingSystem.Instance.UseParticles(_particleTag, pos, Quaternion.identity, 2f);
            m_Graphics.EnableGraphics(true, APPEARING_PARTICLES_HALF_DURATION);
            StartMovement(1f);
        }

        public void HideDrone(ParticlePoolTag _particleTag = ParticlePoolTag.DroneSpawnParticle)
        {
            PoolingSystem.Instance.UseParticles(_particleTag, transform.position, Quaternion.identity, 2f);
            m_Graphics.EnableGraphics(false, APPEARING_PARTICLES_HALF_DURATION);
            StopMovement();
        }
        #endregion

        #region External Modifiers
        public void SetValues(float _followingArea, float _parentFollowingSpeed)
        {
            m_AroundTheShipArea = _followingArea;
            m_SnappingToParentSpeed = _parentFollowingSpeed;
        }

        public float GetFloatingDistance()
        {
            return m_AroundTheShipArea;
        }

        public float GetParentFollowingSpeed()
        {
            return m_SnappingToParentSpeed;
        }

        public void SetHeightMovementFrequency(float _heightMovementFrequency)
        {
            m_HeightMovementFrequency = _heightMovementFrequency;
            m_HeightMovementFrequency *= Random.Range(m_FrequencyRandomizationValues.x, m_FrequencyRandomizationValues.y);
        }

        public void SetTargetFollowFrequency(Vector2 _value)
        {
            m_TargetAvailabilityTimeRange = _value;
        }

        public Vector2 GetTargetFollowFrequency()
        {
            return m_TargetAvailabilityTimeRange;
        }
        #endregion
    }
}