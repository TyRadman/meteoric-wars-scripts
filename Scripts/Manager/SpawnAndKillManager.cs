using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    public class SpawnAndKillManager : Singlton<SpawnAndKillManager>
    {
        [SerializeField] private Color m_DeathColor;
        [SerializeField] private float m_PriorSpawnWaitTime = 2f;
        [SerializeField] private float m_SpawnMovementDuration = 2f;
        private WaitForSeconds m_PriorSpawnWait;

        public const float SHRINK_TIME = 0.2f;
        private const float SPAWN_DISTANCE = 5f;

        protected override void Awake()
        {
            base.Awake();
            m_PriorSpawnWait = new WaitForSeconds(m_PriorSpawnWaitTime);
        }

        public void RevivePlayerShip(List<SpriteRenderer> _sprites, Transform _ship, float _scale, int _playerIndex)
        {
            _ship.gameObject.SetActive(true);
            _ship.localScale = Vector2.one * _scale;
            _ship.position = (Vector2)GameManager.i.PlayersManager.m_SpawningPoints[_playerIndex].position - Vector2.up * SPAWN_DISTANCE;
            _sprites.ForEach(s => s.color = Color.white);
        }

        public void MovePlayerToSpawnPosition(PlayerComponents _shipComponents)
        {
            StartCoroutine(movePlayerToPosition(_shipComponents));
        }

        private IEnumerator movePlayerToPosition(PlayerComponents _ship)
        {
            Transform shipTransform = _ship.transform;
            // disable player's shooting as he moves up
            _ship.ThePlayerShooting.CanShoot = false;

            yield return m_PriorSpawnWait;
            float time = 0f;
            Vector2 startingPosition = shipTransform.position;
            Vector2 endingPosition = (Vector2)shipTransform.position + Vector2.up * SPAWN_DISTANCE;

            while (time < m_SpawnMovementDuration)
            {
                time += Time.deltaTime;
                float t = time / m_SpawnMovementDuration;
                shipTransform.position = Vector2.Lerp(startingPosition, endingPosition, t);
                yield return null;
            }

            // enable player's shooting after he arrives
            _ship.ThePlayerShooting.CanShoot = true;
        }

        public float GetRevivingDuration()
        {
            return m_SpawnMovementDuration + m_PriorSpawnWaitTime;
        }

        /// <summary>
        /// Changes the color of the ship's sprites and shrinks the ship
        /// </summary>
        /// <param name="_sprites"></param>
        /// <param name="_ship"></param>
        /// <param name="_turnOff"></param>
        public void ShipDeath(List<SpriteRenderer> _sprites, Transform _ship, bool _turnOff = false)
        {
            // shrink the ship
            StartCoroutine(Shrink(_ship, _turnOff));
            // change its color to black
            _sprites.ForEach(s => s.color = m_DeathColor);
        }

        private IEnumerator Shrink(Transform _ship, bool _turnOff)
        {
            float time = 0f;
            Vector2 initialScale = _ship.localScale;

            while (time < SHRINK_TIME)
            {
                time += Time.deltaTime;
                float t = time / SHRINK_TIME;
                _ship.transform.localScale = Vector2.Lerp(initialScale, Vector2.zero, t);
                yield return null;
            }

            if (_turnOff)
            {
                _ship.gameObject.SetActive(false);
            }

            _ship.transform.localScale = Vector2.one;
        }
    }
}