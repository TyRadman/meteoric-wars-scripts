using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpaceWar.Audios;

namespace SpaceWar
{
    /// <summary>
    /// A base class for all player collectable.
    /// </summary>
    public class Collectable : MonoBehaviour
    {
        private const float SIZE_CHANGE_AMOUNT = 0.1f;
        private const float FREQUENCY = 7f;
        public float CollectablePoints;
        [SerializeField] private Vector2 m_SpeedRange;
        private float m_CurrentSpeed;
        private float m_Scale = 1;
        [SerializeField] private ParticlePoolTag m_ImpactParticle;
        [SerializeField] private Vector2 m_ValueRange;
        public CollectableTag CollectableTag;
        public bool SizeChange = true;
        [SerializeField] protected Audio m_OnCollectionAudio;

        private void Update()
        {
            transform.Translate(m_CurrentSpeed * Time.deltaTime * Vector2.down);
            float size = Mathf.Sin(Time.time * FREQUENCY) * SIZE_CHANGE_AMOUNT + m_Scale;
            transform.localScale = new Vector2(size, size);
        }

        public void SetValues(float _value, float _size)
        {
            CollectablePoints = m_ValueRange.Lerp(_value);
            m_CurrentSpeed = m_SpeedRange.Lerp(_value);
            transform.localScale = Vector2.one * _size;
            m_Scale = _size;
        }

        public int GetAmount(float _tValue)
        {
            return (int)m_ValueRange.Lerp(_tValue);
        }

        public virtual void OnTriggerAction(PlayerComponents _player)
        {
            GameManager.i.AudioManager.PlayAudio(m_OnCollectionAudio);
            PoolingSystem.Instance.UseParticles(m_ImpactParticle, _player.transform.position, Quaternion.identity);
            gameObject.SetActive(false);
        }
    }
}