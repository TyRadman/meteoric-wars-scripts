using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    public class Bullet : MonoBehaviour
    {
        private float m_Speed;
        [HideInInspector] public Rigidbody2D Rb;
        public SpriteRenderer BulletSprite;
        public BulletUser HittingTag;
        [HideInInspector] public bool IsActive = false;
        public static float Emission = 10f;
        public Color TheColor;
        public ParticlePoolTag ImpactParticleTag;
        public float Damage = 0f;
        protected int UserIndex = -1;

        protected virtual void Awake()
        {
            Rb = GetComponent<Rigidbody2D>();
        }

        public void SetUpColor(Color _col)
        {
            Material mat = BulletSprite.material;
            mat.SetColor("_EmissionColor", _col * Emission);
            TheColor = _col;
        }

        public virtual void SetUp(float _speed, BulletUser _targetName, float _damage, int _userIndex)
        {
            UserIndex = _userIndex;
            IsActive = true;
            m_Speed = _speed;
            HittingTag = _targetName;
            Damage = _damage;
            Rb.simulated = true;
            Rb.velocity = transform.up * m_Speed;
        }

        protected virtual void OnTriggerEnter2D(Collider2D _other)
        {
            OnHit(_other);
        }

        protected virtual void OnHit(Collider2D _other)
        {
            if (_other.CompareTag("Boundries"))
            {
                IsActive = false;
                DisableBullet();
            }

            if (_other.CompareTag(HittingTag.ToString()))
            {
                IsActive = false;
                var damageDetector = _other.GetComponent<DamageDetector>();

                if (damageDetector != null && damageDetector.TakesDamage)
                {
                    // if it's the player shooting then we pass the index of the player for the score
                    if (HittingTag == BulletUser.Enemy)
                    {
                        damageDetector.GotShot(Damage, _other.ClosestPoint(transform.position), UserIndex);
                    }
                    else
                    {
                        damageDetector.GotShot(Damage, _other.ClosestPoint(transform.position));
                    }

                    PoolingSystem.Instance.UseParticles(ImpactParticleTag, transform.position, Quaternion.identity);
                }
                else
                {
                    PoolingSystem.Instance.UseParticles(ImpactParticleTag, transform.position, Quaternion.identity);
                }

                DisableBullet();
            }
        }

        public virtual void DisableBullet()
        {
            Rb.simulated = false;
            BulletSprite.enabled = false;
        }
    }
}