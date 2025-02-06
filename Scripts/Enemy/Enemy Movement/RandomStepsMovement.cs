using System.Collections;
using UnityEngine;

namespace SpaceWar
{
    public class RandomStepsMovement : EnemyShipMovement
    {
        [SerializeField] private float m_Speed;
        [SerializeField] private float m_BetweenStepsWaitTime;
        [SerializeField] private float m_PlayerChasingChance;
        private Vector2 m_TargetPosition;
        private Transform m_ShipTransform;
        private WaitForSeconds m_WaitForMovementWait;
        private RandomMovementValues m_MovementValues;
        private float m_MovementRadius = 10f;

        public const float SPEED_SCALER = 10f;
        
        protected override void Awake()
        {
            base.Awake();
            m_ShipTransform = transform;
        }

        public void SetProperties(RandomMovementValues _values)
        {
            m_MovementValues = _values;
        }

        public override void SetUpValues(Transform _ship, ShipRank _rank)
        {
            base.SetUpValues(_ship, _rank);
            SetUpMovementWithDifficulty();
        }

        public override void SetUpMovementWithDifficulty()
        {
            base.SetUpMovementWithDifficulty();
            m_Speed = m_MovementValues.Speed;
            m_BetweenStepsWaitTime = m_MovementValues.TimeBetweenSteps;
            m_PlayerChasingChance = m_MovementValues.ChasingPlayerChances;
            m_WaitForMovementWait = new WaitForSeconds(m_BetweenStepsWaitTime);
        }

        private void FixedUpdate()
        {
            // check if we passed the next step, in which case, stop the movement and prepare to move to the next step
            if (m_IsMoving && Vector2.Distance(m_ShipTransform.position, m_TargetPosition) < 0.5f)
            {
                m_IsMoving = false;
                m_Rb.velocity = Vector2.zero;
                PauseMovement();
            }
        }

        public override void PerformMovement(Transform _ship)
        {
            base.PerformMovement(_ship);
            SetRandomTarget();
            MoveToPoint();
        }

        public void MoveToPoint()
        {
            Vector2 currentPosition = m_ShipTransform.position;
            Vector2 direction = (m_TargetPosition - currentPosition).normalized;
            m_Rb.velocity = m_Speed * direction;
            m_IsMoving = true;
        }

        private void SetRandomTarget()
        {
            float y = LevelDimensions.Instance.VerticalDimensions.RandomValue();
            // check if the ship will chase the player or go completely randomly
            float x = Random.value < m_PlayerChasingChance ? GameManager.i.PlayersManager.GetRandomPlayerPosition().x :
                LevelDimensions.Instance.HorizontalDimensions.RandomValue();

            Vector2 randomPoint = new Vector2(x, y);

            if (randomPoint.magnitude > m_MovementRadius)
            {
                Vector2 shipPosition = new Vector2(m_ShipTransform.position.x, m_ShipTransform.position.y);
                Vector2 direction = (new Vector2(x, y) - shipPosition).normalized * m_MovementRadius;
                randomPoint = shipPosition + direction;
            }

            m_TargetPosition = randomPoint;
        }

        private void ResumeMovement()
        {
            SetRandomTarget();
            MoveToPoint();
        }

        private void PauseMovement()
        {
            Invoke(nameof(ResumeMovement), m_BetweenStepsWaitTime);
        }

        public override void StopMovement()
        {
            base.StopMovement();
            CancelInvoke();
        }
    }
}