using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SpaceWar
{
    public class PlayerMovement : MonoBehaviour, IController, IInput
    {
        private PlayerComponents m_Componenets;
        [SerializeField] private float m_CurrentMovementSpeed = 10f;
        private float m_OriginalMovementSpeed = 10f;
        [SerializeField] private Rigidbody2D m_Rb;
        private float m_HorizontalValue;
        private float m_VerticalValue;
        // dash values
        public bool IsDashing = false;
        private float m_DirectionMultiplier = 1f;
        private bool m_CanMove = true;
        private Vector2 m_Velocity;
        private bool m_IsMoving = false;

        public void SetUp(IController components)
        {
            m_Componenets = (PlayerComponents)components;
            m_Rb = GetComponent<Rigidbody2D>();
            SetOriginalShipMovementSpeed(m_Componenets.ShipInfo.MovementSpeed);
        }

        #region Input
        public void SetUpInput(int playerIndex)
        {
            GamePlayInput c = InputController.Controls;
            InputActionMap playerMap = InputController.GetMap(playerIndex, ActionMap.GamePlay);
            playerMap.FindAction(c.Gameplay.Movement.name).performed += OnMovementInput;
            playerMap.FindAction(c.Gameplay.Movement.name).canceled += OnMovementStop;
        }

        public void DisposeInput(int playerIndex)
        {
            GamePlayInput c = InputController.Controls;
            InputActionMap playerMap = InputController.GetMap(playerIndex, ActionMap.GamePlay);
            playerMap.FindAction(c.Gameplay.Movement.name).performed -= OnMovementInput;
            playerMap.FindAction(c.Gameplay.Movement.name).canceled -= OnMovementStop;
        }

        public void OnMovementInput(InputAction.CallbackContext cxt)
        {
            if (IsDashing || !m_CanMove)
            {
                return;
            }

            m_Velocity = cxt.ReadValue<Vector2>();
        }

        public void OnMovementStop(InputAction.CallbackContext cxt)
        {
            m_Velocity = Vector2.zero;
        }
        #endregion

        #region IController
        public void Activate()
        {
            SetUpInput(m_Componenets.PlayerIndex);
        }

        public void Deactivate()
        {
            DisposeInput(m_Componenets.PlayerIndex);
        }

        public void Dispose()
        {
            DisposeInput(m_Componenets.PlayerIndex);
        }
        #endregion

        private void Update()
        {
            
        }

        private void FixedUpdate()
        {
            if (IsDashing)
            {
                return;
            }

            MovementProcess();
        }

        public void ReverseInput(bool _reverse)
        {
            m_DirectionMultiplier = _reverse ? 1f : -1f;
        }

        private void MovementProcess()
        {
            m_Rb.velocity = m_CurrentMovementSpeed * m_DirectionMultiplier * Time.deltaTime * m_Velocity;
        }

        public bool StoreLastDirections()
        {
            // if there is no direction specified by the player then there is no dash to perform
            return m_HorizontalValue != 0f || m_VerticalValue != 0f;
        }

        public void PerformDash(float _duration, AnimationCurve _curve, Vector2 _endPosition)
        {
            StartCoroutine(DashPerformance(_duration, _curve, _endPosition));
        }

        private IEnumerator DashPerformance(float _duration, AnimationCurve _curve, Vector2 _endPosition)
        {
            float time = 0f;
            Vector2 startPosition = transform.position;

            // turn off the rigidbody
            m_HorizontalValue = 0f;
            m_VerticalValue = 0f;
            IsDashing = true;

            while (time < _duration)
            {
                time += Time.deltaTime;
                float t = time / _duration;
                transform.position = Vector2.Lerp(startPosition, _endPosition, _curve.Evaluate(t));
                yield return null;
            }

            IsDashing = false;
            transform.position = _endPosition;
        }

        /// <summary>
        /// To Set the ship's default speed
        /// </summary>
        /// <param name="_speed"></param>
        public void SetOriginalShipMovementSpeed(float _speed)
        {
            m_CurrentMovementSpeed = _speed;
            m_OriginalMovementSpeed = _speed;
        }
        /// <summary>
        /// Temporarily set the speed
        /// </summary>
        /// <param name="_speed"></param>
        public void SetSpeed(float _speed)
        {
            m_CurrentMovementSpeed = _speed;
        }
        /// <summary>
        /// Return the movement speed back to the default
        /// </summary>
        public void ResetSpeed()
        {
            m_CurrentMovementSpeed = m_OriginalMovementSpeed;
        }

        public float GetMovementSpeed()
        {
            return m_CurrentMovementSpeed;
        }

        public void Enable(bool _enable)
        {
            m_CanMove = _enable;
            m_Rb.velocity = Vector2.zero;
            m_HorizontalValue = 0f;
            m_VerticalValue = 0f;
        }

        public Vector2 GetDirection()
        {
            return new Vector2(m_HorizontalValue, m_VerticalValue);
        }
    }
}