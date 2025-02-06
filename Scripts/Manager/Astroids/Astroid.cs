using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using SpaceWar.Audios;

namespace SpaceWar
{
    public class Astroid : AstroidParent, IDamagable
    {
        [Header("Stats")]
        [SerializeField] private float m_MaxHealth = 400;
        private float m_CurrentHealth;
        [SerializeField] private PolygonCollider2D m_Collider;
        private float m_Damage = 20f;
        [SerializeField] private AstroidDamageRenderer m_DamageRenderer;
        [SerializeField] private ScreenFreezer.FreezeStrength m_ScreenFreezeStrenth;
        private float m_Score;
        /// <summary>
        /// Coin value
        /// </summary>
        private const float m_CollectableValue = 0.35f;
        private Vector2Int m_CoinsCountRange;
        public bool CanHitPlayer = true;
        [HideInInspector] public SortingGroup Sort;
        [SerializeField] private Audio m_DeathAudio;

        public PolygonCollider2D GetCollider()
        {
            return m_Collider;
        }

        public void SetUpValues(Vector2 _speedRange, AstroidSize _size, float _maxHP, float _damage, ParticlesPooling _deathParticle, float _score, ScreenFreezer.FreezeStrength _screenFreeze, Vector2Int _coinsCountRange)
        {
            m_Score = _score;
            m_CoinsCountRange = _coinsCountRange;
            m_Damage = _damage;
            m_MaxHealth = _maxHP;
            m_CurrentHealth = m_MaxHealth;
            SpeedRange = _speedRange;
            Size = _size;
            m_DamageRenderer.SetDeathEffect(_deathParticle);
            m_ScreenFreezeStrenth = _screenFreeze;
            Sort = GetComponent<SortingGroup>();
        }

        public void TakeDamage(float _damage)
        {
            m_CurrentHealth -= _damage;

            if (m_CurrentHealth <= 0)
            {
                m_CurrentHealth = 0;

                // TODO: add score handling again
                // it could be a minion for a summoner ship which doesn't count
                //if (_playerIndex > -1) GameManager.i.PlayersManager.Players[_playerIndex].Components.Stats.AddScore(m_Score);

                OnAstroidDestroyed();
            }

            GameManager.i.EnemyHealthBar.UpdateHealthBar(m_CurrentHealth, m_MaxHealth, $"{Size} Astroid");
        }

        private void OnTriggerEnter2D(Collider2D _other)
        {
            if (_other.CompareTag("Player") && CanHitPlayer)
            {
                var damageDetector = _other.GetComponent<DamageDetector>();
                damageDetector.GotShot(m_Damage, transform.position);
            }

            if (_other.CompareTag("AstroidBoundry"))
            {
                DisableAstroid();
            }
        }

        private void OnAstroidDestroyed()
        {
            // freeze the screen if necessary
            ScreenFreezer.i.FreezeScreen(m_ScreenFreezeStrenth);
            // explosion particles
            m_DamageRenderer.PlayDeathParticles();
            // drop goodies
            int coinsCount = Random.Range(m_CoinsCountRange.x, m_CoinsCountRange.y);
            CollectableSpawner.i.SpawnCollectableWithValues(transform.position, CollectableTag.Coins, m_CollectableValue, coinsCount, m_Collider.bounds.size.x);
            GameManager.i.AudioManager.PlayAudio(m_DeathAudio);
            DisableAstroid();
        }

        private void DisableAstroid()
        {
            m_CurrentHealth = m_MaxHealth;
            gameObject.SetActive(false);
            IsActive = false;
        }

        public void SetUpCollider(List<PolygonCollider2D> _colliders)
        {
            Helper.SetPolygonCollider(m_Collider, _colliders);
        }

        public void SetBackGroundData(int _layer, float _size, float _speed, bool _canHitPlayer)
        {
            Sort.sortingOrder = _layer;
            transform.localScale = Vector3.one * _size;
            Rb.velocity *= _speed;
            CanHitPlayer = _canHitPlayer;
        }
    }
}