using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpaceWar.Audios;

namespace SpaceWar
{
    public class RocketBullet : Bullet
    {
        private float m_EffectRadius;
        private float m_AreaDamage;
        private float m_DamageDuration;
        private const float TAKING_DAMAGE_RATE = 0.5f;
        private WaitForSeconds m_WaitBetweenDamages;
        [SerializeField] private List<ParticleSystem> m_Tails;
        [SerializeField] protected Audio ImpactAudio;
        [Header("Movement")]
        [SerializeField] private float m_Amplitude;
        [SerializeField] private float m_Frequency;
        private Collider2D m_Collider;
        [SerializeField] private Transform m_GraphicsTransform;


        protected override void Awake()
        {
            base.Awake();

            m_Collider = GetComponent<Collider2D>();
            m_WaitBetweenDamages = new WaitForSeconds(TAKING_DAMAGE_RATE);

            if (m_Tails != null)
            {
                m_Tails.ForEach(t => t.Stop());
            }
        }

        private void Update()
        {
            if (!IsActive)
            {
                return;
            }

            Vector2 colPos = m_Collider.offset;
            Vector2 transformPos = m_GraphicsTransform.localPosition;
            colPos.x = Mathf.Sin(Time.time * m_Frequency) * m_Amplitude;
            transformPos.x = colPos.x;
            m_Collider.offset = colPos;
            m_GraphicsTransform.localPosition = transformPos;
        }

        protected override void OnHit(Collider2D _other)
        {
            if (_other.CompareTag("Boundries"))
            {
                IsActive = false;
                DisableBullet();
            }

            if (_other.CompareTag(HittingTag.ToString()))
            {
                OnTargetHit();
            }
        }

        public virtual void OnTargetHit()
        {
            GameManager.i.AudioManager.PlayAudio(ImpactAudio);
            PoolingSystem.Instance.UseParticles(ImpactParticleTag, transform.position, Quaternion.identity, m_EffectRadius, m_DamageDuration);
            StartCoroutine(AreaDamageProcess());
            DisableBullet();
        }

        public override void DisableBullet()
        {
            base.DisableBullet();

            m_Tails.ForEach(t => t.Stop());
        }

        private IEnumerator AreaDamageProcess()
        {
            int loops = (int)(m_DamageDuration / TAKING_DAMAGE_RATE);
            LayerMask targetMask = HittingTag == BulletUser.Enemy ? GameManager.i.GeneralValues.EnemyLayer : GameManager.i.GeneralValues.PlayerLayer;
            print($"Shot at {Time.time}");

            for (int i = 0; i < loops; i++)
            {
                var effectedShips = Physics2D.OverlapCircleAll(transform.position, m_EffectRadius, targetMask);

                for (int j = 0; j < effectedShips.Length; j++)
                {
                    var detector = effectedShips[j].GetComponent<DamageDetector>();

                    if (detector != null && detector.TakesDamage)
                    {
                        detector.GotShot(m_AreaDamage, transform.position, UserIndex);
                    }
                    else
                    {
                        print($"Nothing shot {detector.name} at {Time.time}");
                    }
                }

                yield return m_WaitBetweenDamages;
            }

            IsActive = false;
        }

        public void SetUpRocketBullet(float _speed, BulletUser _targetName, float _damage, AreaEffectVariables _areaEffect, int _userIndex = -1)
        {
            SetUp(_speed, _targetName, _damage, _userIndex);

            if (m_Tails != null)
            {
                m_Tails.ForEach(t => t.Play());
            }

            m_EffectRadius = _areaEffect.Area;
            m_AreaDamage = _areaEffect.AreaDamage;
            m_DamageDuration = _areaEffect.Duration;
        }
    }
}