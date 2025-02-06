using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace SpaceWar
{
    public class ChasingWeapon : WeaponShootingProcess
    {
        private Transform target;

        protected override void Awake()
        {
            base.Awake();
            // target = GameObject.FindGameObjectWithTag("Target").transform;
        }

        public override void SetUpComponents(Shooter _shooter)
        {
            base.SetUpComponents(_shooter);

            CoolingDownTime = TheShooter.Weapon().CoolDownTime;
        }

        public override void StartShootingProcess()
        {
            base.StartShootingProcess();
        }

        #region Shooting Mode Methods
        public override void SetUpShootingAction()
        {
            base.SetUpShootingAction();

            switch (TheShooter.Weapon().TheShootingMode)
            {
                case ShootingMode.PerAllSP:
                    {
                        ShootingAction = PerEveryShootingPoint;
                        break;
                    }
                case ShootingMode.PerSpecificSP:
                    {
                        ShootingAction = PerSpecificShootingPoints;
                        break;
                    }
                case ShootingMode.ThroughAllSPPerShot:
                    {
                        ShootingAction = ThroughAllShootingPoints;
                        break;
                    }
                case ShootingMode.ThroughSpecificSPPerShot:
                    {
                        ShootingAction = ThroughSpecificShootingPoints;
                        break;
                    }
                case ShootingMode.ThroughAllSPPerSet:
                    {
                        ShootingAction = ThroughAllShootingPointsPerSet;
                        break;
                    }
                case ShootingMode.ThroughtSpecificSPPerSet:
                    {
                        ShootingAction = ThroughSpecificShootingPointPerSet;
                        break;
                    }
            }
        }

        private void PerEveryShootingPoint()
        {
            for (int i = 0; i < ShootingPoints.Count; i++)
            {
                for (int j = 0; j < TheShooter.Weapon().Shots.Count; j++)
                {
                    StartCoroutine(ChasingBulletProcess(i, j));
                }
            }
        }

        private void PerSpecificShootingPoints()
        {
            for (int i = 0; i < TheShooter.Weapon().ShootingPointsIndices.Count; i++)
            {
                StartCoroutine(ChasingBulletProcess(TheShooter.Weapon().ShootingPointsIndices[i]));
            }
        }

        private void ThroughAllShootingPoints()
        {
            StartCoroutine(ChasingBulletProcess(GetNextShootingPointIndex(ShootingPoints.Count)));
        }

        private void ThroughSpecificShootingPoints()
        {
            int index = GetNextShootingPointIndex(TheShooter.Weapon().ShootingPointsIndices.Count);
            StartCoroutine(ChasingBulletProcess(TheShooter.Weapon().ShootingPointsIndices[index]));
        }

        private void ThroughAllShootingPointsPerSet()
        {
            StartCoroutine(ChasingBulletProcess(ShootingPointIndex));
            ShootingPointIndex = GetNextShootingPointIndex(ShootingPoints.Count);
        }

        private void ThroughSpecificShootingPointPerSet()
        {
            StartCoroutine(ChasingBulletProcess(TheShooter.Weapon().ShootingPointsIndices[ShootingPointIndex]));
            ShootingPointIndex = GetNextShootingPointIndex(TheShooter.Weapon().ShootingPointsIndices.Count);
        }
        #endregion

        private IEnumerator ChasingBulletProcess(int _shootingPointIndex, int _shotIndex = 0)
        {
            PlayAudio();

            Bullet bullet = new Bullet();
            float speed = TheShooter.Weapon().Shots[_shotIndex].Speed;
            float bulletSpeed = TheShooter.Weapon().Shots[_shotIndex].Speed;
            BulletUser targetName = Helper.ReverseUser(TheShooter.UserTag);
            Vector3 pos = GetShootingPointPosition(0, _shootingPointIndex);
            // Quaternion dir = Quaternion.Euler(transform.up);
            Quaternion dir = Quaternion.Euler(transform.localEulerAngles + Vector3.forward * TheShooter.Weapon().Shots[_shotIndex].Angle);
            TheShooter.Effects.ShootingMuzzle(pos, dir);
            PoolObjectTag bulletType = TheShooter.Weapon().BulletPoolType;

            switch (TheShooter.Weapon().TheWeaponTag)
            {
                case WeaponTag.Bullets:
                    {
                        var bulleT = PoolingSystem.Instance.GetBullet(bulletType, TheShooter.UserTag);
                        bulleT.transform.SetPositionAndRotation(pos, dir);
                        bulleT.SetUp(bulletSpeed, targetName, TheShooter.Damage, TheShooter.UserIndex);
                        bullet = bulleT;
                        break;
                    }
                case WeaponTag.Rockets:
                    {
                        var bulleT = (RocketBullet)PoolingSystem.Instance.GetBullet(bulletType, TheShooter.UserTag);
                        bulleT.transform.SetPositionAndRotation(pos, dir);
                        bulleT.SetUpRocketBullet(bulletSpeed, targetName, TheShooter.Damage, TheShooter.Weapon().AreaEffect, TheShooter.UserIndex);
                        bullet = bulleT;
                        break;
                    }
            }

            float time = 0f;

            while (bullet.IsActive)
            {
                Vector3 target = GetTarget(bullet.transform);
                Vector2 direction = (target - bullet.transform.position).normalized;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                Quaternion rotationToTarget = Quaternion.AngleAxis(angle - 90, Vector3.forward);
                time += Time.deltaTime;
                // we want it to progress over time later
                float precision = TheShooter.Weapon().ChasingWeapon.Precision;
                bullet.transform.rotation = Quaternion.Slerp(bullet.transform.rotation, rotationToTarget, Time.deltaTime * precision);
                bullet.Rb.velocity = bullet.transform.up * speed;
                yield return UpdateRate;
            }
        }

        private Vector3 GetTarget(Transform _bullet)
        {
            // return target.position;
            // if the shooter is a player
            if (TheShooter.UserTag == BulletUser.Player)
            {
                // target.position = WavesManager.Instance.GetClosestEnemyToPoint(_bullet);
                return WavesManager.i.GetClosestEnemyToPoint(_bullet);
            }
            // if it's an enemy
            else
            {
                return GameManager.i.PlayersManager.GetClosestPlayerToTransform(_bullet.position);
            }
        }
    }
}