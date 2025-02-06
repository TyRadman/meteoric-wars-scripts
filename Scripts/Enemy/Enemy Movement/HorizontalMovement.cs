using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace SpaceWar
{
    public class HorizontalMovement : EnemyShipMovement
    {
        public enum HorizontalMovementStyle
        {
            Parade, ContinuousMovement
        }

        // [SerializeField] private float m_Difficulty = 0.5f;
        [Header("Modifiers")]
        [SerializeField] private HorizontalMovementStyle m_Style;
        [SerializeField] private int m_StepsOverScreen = 5;
        [Tooltip("Time it takes to complete one screen")]
        [Range(0.2f, 10f)] [SerializeField] private float m_Speed = 2f;
        [SerializeField] private float m_BetweenStepsWaitTime = 0.5f;
        [SerializeField] private int m_CurrentStepIndex = 0;
        private float m_StepSize;
        private ShipRank m_Rank;
        private int m_Direction = 1;
        private float m_TargetX;
        private int m_Steps;
        private float m_Offset;

        #region Constants
        public static Vector2Int STEP_OVER_SCREEN_RANGE = new Vector2Int(2, 12);
        private HorizontalMovementValues m_MovementValues;
        #endregion

        #region Set Up
        private void Start()
        {
            m_Offset = LevelDimensions.Instance.LevelWidth / 2;
            m_Rb = GetComponent<Rigidbody2D>();
        }

        public override void SetUpValues(Transform _ship, ShipRank _rank)
        {
            base.SetUpValues(_ship, _rank);
            m_Rank = _rank;
            SetUpMovementWithDifficulty();

            switch (m_Style)
            {
                case HorizontalMovementStyle.Parade:
                    {
                        ParadeStyleSetUp(_ship);
                        break;
                    }
                case HorizontalMovementStyle.ContinuousMovement:
                    {
                        ContinuousStyleSetUp(_ship);
                        break;
                    }
            }
        }

        public override void SetUpMovementWithDifficulty()
        {
            base.SetUpMovementWithDifficulty();
            m_Speed = m_MovementValues.Speed;
            m_StepsOverScreen = m_MovementValues.StepsOverScreen;
            m_BetweenStepsWaitTime = m_MovementValues.TimeBetweenSteps;
        }
        #endregion

        #region Styles Set Up
        private void ParadeStyleSetUp(Transform _ship)
        {

        }

        private void ContinuousStyleSetUp(Transform _ship)
        {
            m_BetweenStepsWaitTime = 0f;
        }

        public void SetHorizontalMovementProperties(HorizontalMovementValues _values)
        {
            m_MovementValues = _values;
        }
        #endregion

        #region Helping Methods
        public override void EntranceMovement(Vector3 _startPosition, Vector3 _endPosition)
        {
            base.EntranceMovement(_startPosition, _endPosition);
            m_CurrentStepIndex = GetClosestStep(_startPosition);
        }

        public void SetStyle(HorizontalMovementStyle _style)
        {
            m_Style = _style;
        }
        #endregion

        #region Movement Process
        public override void PerformMovement(Transform _ship)
        {
            base.PerformMovement(_ship);
            m_CurrentStepIndex = GetClosestStep(_ship.position);
            MoveToNextStep();
        }

        private void FixedUpdate()
        {
            if (m_IsMoving)
            {
                // check if we passed the next step, in which case, stop the movement and prepare to move to the next step
                if (transform.position.x < m_TargetX && m_Direction == -1 || transform.position.x > m_TargetX && m_Direction == 1)
                {
                    m_IsMoving = false;
                    m_Rb.velocity = Vector2.zero;
                    PauseMovement();
                }
            }
        }

        private int GetClosestStep(Vector2 _startPosition)
        {
            int index = -1;
            float minDifference = float.MaxValue;
            float startX = _startPosition.x;
            m_StepSize = LevelDimensions.Instance.LevelWidth / (m_StepsOverScreen - 1);

            for (int i = 0; i < m_StepsOverScreen; i++)
            {
                float difference = Mathf.Abs(startX - (i * m_StepSize - m_Offset));

                if (difference < minDifference)
                {
                    index = i;
                    minDifference = difference;
                }
            }

            return index;
        }

        private void PauseMovement()
        {
            Invoke(nameof(MoveToNextStep), m_BetweenStepsWaitTime);
        }

        private void MoveToNextStep()
        {
            m_CurrentStepIndex += m_Direction;

            if (m_CurrentStepIndex > m_StepsOverScreen - 1 || m_CurrentStepIndex < 0)
            {
                m_Direction = -m_Direction;
                // cancelling the addition that already occured and takes an extra step
                m_CurrentStepIndex += m_Direction * 2;
            }

            // gets the position of the next point
            m_TargetX = (m_CurrentStepIndex * m_StepSize) - m_Offset;
            // move the ship in the direction of the target
            m_Rb.velocity = new Vector2(Mathf.Sign(m_TargetX - transform.position.x) * m_Speed, 0);
            m_IsMoving = true;
        }

        public override void StopMovement()
        {
            base.StopMovement();
            CancelInvoke();
        }
        #endregion
    }
}