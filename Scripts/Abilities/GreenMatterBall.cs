using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    public class GreenMatterBall : MonoBehaviour
    {
        [SerializeField] private BulletWeapon m_Weapon;
        [SerializeField] private int m_ShotsNumber = 20;
        [SerializeField] private float m_TimeBetweenShots = 0.1f;
        [SerializeField] private Vector2 m_SizeRange;
        [SerializeField] private Transform m_Graphics;
        [SerializeField] private float m_TransformationDuration = 1f;
        [SerializeField] private float m_DistanceToMoveForward = 2f;
        [SerializeField] private SpriteRenderer m_Renderer;
        private Shooter m_Shooter;
        private WaitForSeconds m_WaitingTime;
        private float m_SizeChunk;

        private void Awake()
        {
            m_Shooter = GetComponent<Shooter>();
            m_WaitingTime = new WaitForSeconds(m_TimeBetweenShots);
            m_SizeChunk = (m_SizeRange.y - m_SizeRange.x) / m_ShotsNumber;
            m_Shooter.SetUpWeapon(m_Weapon);
            m_Renderer.enabled = false;
        }

        public void SetUp(int _playerIndex)
        {
            m_Shooter.UserIndex = _playerIndex;
        }

        public void StartShooting(Vector2 _startPosition)
        {
            // enable the graphics
            m_Renderer.enabled = true;
            // set the size of the ball
            m_Graphics.localScale = Vector2.zero;
            // start shooting
            StartCoroutine(growAndMoveBall(_startPosition));
        }

        private IEnumerator shootingPorcess()
        {
            for (int i = m_ShotsNumber; i >= 0; i--)
            {
                m_Shooter.Shoot();
                m_Graphics.localScale = (i * m_SizeChunk) * Vector2.one;
                yield return m_WaitingTime;
            }
        }

        private IEnumerator growAndMoveBall(Vector2 _startPosition)
        {
            float time = 0f;
            Vector2 scale = Vector2.one * m_SizeRange.y;
            Vector2 finalPosition = _startPosition + Vector2.up * m_DistanceToMoveForward;

            while (time < m_TransformationDuration)
            {
                time += Time.deltaTime;
                float t = time / m_TransformationDuration;
                m_Graphics.localScale = Vector2.Lerp(Vector2.zero, scale, t);
                transform.position = Vector2.Lerp(_startPosition, finalPosition, t);
                yield return null;
            }

            StartCoroutine(shootingPorcess());
        }

        public void SetShotsNumber(int _shotsNumber)
        {
            m_ShotsNumber = _shotsNumber;
            m_SizeChunk = (m_SizeRange.y - m_SizeRange.x) / m_ShotsNumber;
        }

        /// <summary>
        /// Returns the time it takes the green ball to shoot all shots and vanish.
        /// </summary>
        public float GetActionDuration()
        {
            return m_TransformationDuration + m_ShotsNumber * m_TimeBetweenShots;
        }

        public void DisableGraphics()
        {
            m_Renderer.enabled = false;
        }
    }
}