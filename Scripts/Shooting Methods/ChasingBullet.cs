using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    public class ChasingBullet : Bullet
    {
        public float Accuracy;
        private Transform m_ObjectToShoot;

        protected override void Awake()
        {
            base.Awake();
            m_ObjectToShoot = GameObject.FindGameObjectWithTag("Target").transform;
        }

        public override void SetUp(float _speed, BulletUser _target, float _damage, int _userIndex)
        {
            base.SetUp(_speed, _target, _damage, _userIndex);
            StartCoroutine(chaseTarget());
        }

        private IEnumerator chaseTarget()
        {
            while (true)
            {
                Vector3 dir = m_ObjectToShoot.position;
                dir.Normalize();
                float rotateAmount = Vector3.Cross(dir, transform.up).z;
                Rb.angularVelocity = -rotateAmount * Accuracy;
                Rb.velocity = transform.up;
                yield return null;
            }
        }

        protected override void OnTriggerEnter2D(Collider2D other)
        {
            base.OnTriggerEnter2D(other);

            StopAllCoroutines();
        }
    }
}