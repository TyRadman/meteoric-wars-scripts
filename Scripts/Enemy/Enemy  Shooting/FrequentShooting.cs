using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    public class FrequentShooting : EnemyShooting
    {
        public enum FrequentShooterStyles
        {
            RandomShots, CoolDownShots
        }

        [SerializeField] private FrequentShooterStyles m_Style;
        private bool m_IsRandom = false;
        [SerializeField] private Vector2 m_ShootingFrequency = new Vector2(1f, 2f);
        private Coroutine m_ShootingCoroutine;
        private WaitForSeconds m_Waiting;

        public override void SetUp(IController components)
        {
            base.SetUp(components);

            // based on styles
            // this style makes the delay between each shot random
            if (m_Style == FrequentShooterStyles.RandomShots)
            {
                m_IsRandom = true;
            }
            // this style is a continuous shooter as long as the weapon allows it
            else if (m_Style == FrequentShooterStyles.CoolDownShots)
            {
                m_Waiting = new WaitForSeconds(Components.ShipShooter.Weapon().CoolDownTime);
            }
        }

        public override void PerformShooting()
        {
            base.PerformShooting();
            m_ShootingCoroutine = StartCoroutine(ShootingLoop());
        }

        private IEnumerator ShootingLoop()
        {
            while (true)
            {
                ShootingProcess();

                if (m_IsRandom)
                {
                    m_Waiting = new WaitForSeconds(Random.Range(m_ShootingFrequency.x, m_ShootingFrequency.y));
                }

                yield return m_Waiting;
            }
        }

        public override void StopShooting()
        {
            base.StopShooting();

            if (m_ShootingCoroutine != null) StopAllCoroutines();
        }
    }
}