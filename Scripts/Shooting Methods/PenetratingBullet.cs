using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    public class PenetratingBullet : Bullet
    {
        protected override void OnHit(Collider2D _other)
        {
            if (_other.CompareTag("Boundries"))
            {
                IsActive = false;
                DisableBullet();
            }

            if (_other.CompareTag(HittingTag.ToString()))
            {
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
            }
        }
    }
}