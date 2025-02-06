using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace SpaceWar
{
    public class DamageTester : DamageDetector, IDamagable
    {
        [SerializeField] private TextMeshPro m_Text;
        [SerializeField] private float m_Radius = 1f;
        [SerializeField] private CircleCollider2D m_Collider;
        [SerializeField] private Transform m_Graphics;
        [SerializeField] private List<float> m_Damages = new List<float>();
        [SerializeField] private float m_DamageAverageDuration = 1f;

        private void Update()
        {
            if (m_Damages.Count == 0)
            {
                m_Text.text = "0";
                return;
            }

            float damage = 0;
            m_Damages.ForEach(d => damage += d);
            damage /= m_DamageAverageDuration;
            m_Text.text = damage.ToString("0");
        }

        public void ChangeSize()
        {
            m_Collider.radius = m_Radius;
            m_Graphics.localScale = Vector2.one * m_Radius * 2;
        }

        public void TakeDamage(float _damage)
        {
            print($"Happened at {Time.time}");
            m_Damages.Add(-_damage);
            Invoke(nameof(RemoveDamage), m_DamageAverageDuration);
        }

        private void RemoveDamage()
        {
            m_Damages.RemoveAt(m_Damages.Count - 1);
        }
    }
}
