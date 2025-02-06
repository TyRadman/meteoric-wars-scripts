using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    public class EnemyShipMovement : MonoBehaviour
    {
        public bool MovementTriggeredShooting = false;
        [HideInInspector] public EnemyComponents Components;
        protected Rigidbody2D m_Rb;
        private Vector2 m_LastTarget;
        private Vector2 m_LastDirection;
        protected bool m_IsMoving = false;
        private const float STOPPING_DISTANCE = 0.5f;

        protected virtual void Awake()
        {
            m_Rb = GetComponent<Rigidbody2D>();

            if (MovementTriggeredShooting)
            {
                GetComponent<EnemyShooting>().StopShooting();
                // if the shooting is controller by the enemy movement then there will no cool down time
                GetComponent<EnemyShooting>().Components.ShipShooter.Weapon().CoolDownTime = 0.01f;
            }

            Components = GetComponent<EnemyComponents>();
            // newly added
        }

        public virtual void SetUpValues(Transform _ship, ShipRank _rank)
        {

        }

        #region Movement Functions
        public virtual void PerformMovement(Transform _ship)
        {

        }

        public virtual void StopMovement()
        {
            m_IsMoving = false;
            m_Rb.velocity = Vector2.zero;
        }

        #region Entrance movement
        public virtual void EntranceMovement(Vector3 _startPosition, Vector3 _endPosition)
        {
            StartCoroutine(PerformEntranceMovement(_startPosition, _endPosition));
        }

        private IEnumerator PerformEntranceMovement(Vector3 _startPosition, Vector3 _endPosition)
        {
            float time = 0f;
            float totalTime = GameManager.i.GeneralValues.EntranceMovementSpeed;
            AnimationCurve speedChanges = GameManager.i.GeneralValues.EntranceMovementSpeedCurve;

            while (time < totalTime)
            {
                time += Time.deltaTime;
                float t = time / totalTime;
                transform.position = Vector3.Lerp(_startPosition, _endPosition, speedChanges.Evaluate(t));
                yield return null;
            }

            // start moving
            PerformMovement(transform);
            // start shooting
            Components.ShootingMethod.SetUp(Components);
            Components.ShootingMethod.PerformShooting();
        }
        #endregion

        #endregion
        protected virtual void TriggeredShooting()
        {
            Components.ShipShooter.Shoot();
        }

        /// <summary>
        /// Set up the movement method in a way that matches with difficulty given.
        /// </summary>
        /// <param name="_difficulty">A value from 0 to 1.</param>
        public virtual void SetUpMovementWithDifficulty()
        {

        }

        #region Additional External Methods
        /// <summary>
        /// Moves the ship to target with a given speed. Make sure you call it off after using
        /// </summary>
        /// <param name="_targetLocation"></param>
        /// <param name="_speed"></param>
        public void MoveToDirection(Vector2 _targetPosition, float _speed)
        {
            if (m_IsMoving)
            {
                StopMovement();
            }

            if (_targetPosition.x != m_LastTarget.x)
            {
                m_LastTarget = _targetPosition;
                // calculate movement direction towards target on the x-axis only
                m_LastDirection = new Vector2(_targetPosition.x - transform.position.x, 0).normalized;
                // calculate movement velocity
                Vector2 velocity = m_LastDirection * _speed;
                // set the y-velocity to the current y-velocity of the Rigidbody2D
                velocity.y = m_Rb.velocity.y;
                // set the velocity of the Rigidbody2D
                m_Rb.velocity = velocity;
            }

            if (transform.position.x < _targetPosition.x && m_LastDirection.x == -1 || transform.position.x > _targetPosition.x && m_LastDirection.x == 1)
            {
                m_Rb.velocity = new Vector2(0, m_Rb.velocity.y);
            }
        }

        public void StopMovementToDirection()
        {
            m_Rb.velocity = Vector2.zero;
        }
        #endregion
    }
}