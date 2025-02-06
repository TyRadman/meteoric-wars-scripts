using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    public class ReflectingBullet : Bullet
    {
        [SerializeField] private TrailRenderer m_Trail;

        protected override void OnHit(Collider2D _other)
        {
            if (_other.CompareTag("Reflector"))
            {
                var bulletSpeed = Rb.velocity;

                if (_other.GetComponent<ReflectorSurface>().Type == ReflectorSurface.ReflectorDirection.Horizontal)
                {
                    // reverse the direction of the bullet (reflecting it considering the collider normals would be expensive to process for tons of bullets and it wouldn't make much of a difference)
                    Rb.velocity = new Vector2(bulletSpeed.x, -bulletSpeed.y);
                    // rotate the bullet
                    var angles = transform.eulerAngles;
                    angles.z = 180f - angles.z;
                    transform.eulerAngles = angles;
                }
                else
                {
                    Rb.velocity = new Vector2(-bulletSpeed.x, bulletSpeed.y);
                    // rotate the bullet
                    var angles = transform.eulerAngles;
                    angles.z = 180f - angles.z;
                    transform.eulerAngles = angles;
                }

                PoolingSystem.Instance.UseParticles(ParticlePoolTag.AlienShipImpact, transform.position, Quaternion.identity);
            }

            base.OnHit(_other);
        }

        public override void SetUp(float _speed, BulletUser _targetName, float _damage, int _userIndex)
        {
            base.SetUp(_speed, _targetName, _damage, _userIndex);

            m_Trail.Clear();
            m_Trail.emitting = true;
        }

        public override void DisableBullet()
        {
            base.DisableBullet();

            m_Trail.emitting = false;
        }
    }
}