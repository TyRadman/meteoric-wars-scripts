using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    public class BulletsReflector : MonoBehaviour
    {
        [SerializeField] private ParticleSystem m_HitParticles;
        [SerializeField] private UnityEngine.Rendering.Universal.Light2D m_Light;
        [SerializeField] private float m_MinIntensity;
        [SerializeField] private float m_MaxIntensity;
        [SerializeField] private float m_DecrementRate;
        [SerializeField] private float m_SpeedMultiplier;
        public BulletUser ShieldTag;
        [HideInInspector] public PlayerComponents ShipComponents;
        #region Values that vary based on the skill level
        /// <summary>
        /// A value between zero and one.
        /// </summary>
        private float m_Resilience;
        private int m_ReflectedBulletsNumber = 0;
        #endregion
        private readonly float[] m_Angles = { 0f, 15f, -15f, 0f, 15f, -15f };

        public void SetValues(float _resilience, int _bulletsNumber)
        {
            m_Resilience = _resilience; m_ReflectedBulletsNumber = _bulletsNumber;
        }

        public IEnumerator Shine()
        {
            m_Light.intensity = 0f;

            while (m_Light.intensity < m_MinIntensity)
            {
                m_Light.intensity += m_DecrementRate;
                yield return null;
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("PlayerBullet"))
            {
                // cache the bullet
                var bullet = collision.GetComponent<Bullet>();
                string message = string.Empty;

                // if the bullet doesn't have the same tag as the owner of the shield (meaning it's not friendly fire)
                if (bullet.HittingTag == ShieldTag)
                {
                    for (int i = 0; i < m_ReflectedBulletsNumber; i++)
                    {
                        int n = m_ReflectedBulletsNumber;
                        int index = (n * (n + 1)) / 2 - n + i; // this should be changed. Even though it works for all possible number of array elements, it's quite expensive to calculate every time the ship gets hit. Change the array angles order to {0, 15, -15, 0, 15}
                        float additionalAngle = m_Angles[index];
                        message += additionalAngle + ", ";
                        Bullet newBullet = PoolingSystem.Instance.GetBullet(PoolObjectTag.CrystalShot, ShieldTag); ////
                                                                                                                   // reverse the direction of the bullet (reflecting it considering the collider normals would be expensive to process for tons of bullets and it wouldn't make much of a difference)
                        var bulletSpeed = bullet.Rb.velocity;
                        newBullet.Rb.velocity = new Vector2(bulletSpeed.x, -bulletSpeed.y) * m_SpeedMultiplier;
                        // rotate the bullet
                        var angles = bullet.transform.eulerAngles;
                        angles.z = 180f - angles.z + additionalAngle;
                        // newBullet.transform.eulerAngles = angles;

                        newBullet.transform.SetPositionAndRotation(bullet.transform.position, Quaternion.Euler(angles));
                        newBullet.SetUp(-bulletSpeed.y * m_SpeedMultiplier, Helper.ReverseUser(ShieldTag), bullet.Damage, ShipComponents.PlayerIndex);
                        // play the hitting particles
                        m_HitParticles.transform.position = bullet.transform.position;
                        m_HitParticles.Play();
                        // increase the intensity of the sprite light2D to indicate that it was hit
                        m_Light.intensity = m_MaxIntensity;
                        bullet.DisableBullet(); ////

                        // apply damage to the ship
                        if (ShipComponents.Health != null)
                        {
                            ShipComponents.Health.TakeDamage(bullet.Damage * m_Resilience);
                        }
                    }
                }
                else
                {
                    print($"{bullet.name} hit");
                }
            }
        }

        private void Update()
        {
            if (m_Light.intensity > m_MinIntensity)
            {
                m_Light.intensity -= m_DecrementRate;
            }
        }
    }
}