using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    public class SniperMachineGunWeapon : WeaponShootingProcess
    {
        // private static Transform target;
        private SniperMachineGunValues m_Values;
        public static Vector3 PlayerRotation;

        protected override void Awake()
        {
            base.Awake();
            // target = GameObject.FindGameObjectWithTag("Target").transform;
        }

        public override void SetUpComponents(Shooter _shooter)
        {
            base.SetUpComponents(_shooter);
            m_Values = TheShooter.Weapon().TheSniperMachineGunValues;
            CoolingDownTime = m_Values.TimeBetweenShots * m_Values.ShotsGroup.Count + TheShooter.Weapon().CoolDownTime;
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
                        ShootingAction = ThroughSpecificShootingPoints;
                        break;
                    }
                case ShootingMode.ThroughtSpecificSPPerSet:
                    {
                        ShootingAction = ThroughSpecificShootingPoints;
                        break;
                    }
            }
        }


        private void PerEveryShootingPoint()
        {
            for (int i = 0; i < ShootingPoints.Count; i++)
            {
                StartCoroutine(ShotGroupShootingProcess(i));
            }
        }

        private void PerSpecificShootingPoints()
        {
            for (int i = 0; i < TheShooter.Weapon().ShootingPointsIndices.Count; i++)
            {
                StartCoroutine(ShotGroupShootingProcess(TheShooter.Weapon().ShootingPointsIndices[i]));
            }
        }

        private void ThroughAllShootingPoints()
        {
            for (int i = 0; i < m_Values.ShotsGroup.Count; i++)
            {
                StartCoroutine(ShotShootingProcess(GetNextShootingPointIndex(ShootingPoints.Count), i));
            }
        }

        private void ThroughSpecificShootingPoints()
        {
            int index = TheShooter.Weapon().ShootingPointsIndices[GetNextShootingPointIndex(TheShooter.Weapon().ShootingPointsIndices.Count)];
            StartCoroutine(ShotGroupShootingProcess(index));
        }
        #endregion

        #region Main Functionality
        private IEnumerator ShotGroupShootingProcess(int _shootingPointIndex)
        {
            WaitForSeconds wait = new WaitForSeconds(TheShooter.Weapon().TheSniperMachineGunValues.TimeBetweenShots);

            for (int i = 0; i < m_Values.ShotsGroup.Count; i++)
            {
                PlayAudio();
                Vector2 target = GetTarget(transform, TheShooter.UserTag);
                Vector2 direction = (target - (Vector2)transform.position).normalized;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;
                ShootShotGroup(m_Values.ShotsGroup[i].Shots, angle, _shootingPointIndex);
                yield return wait;
            }
        }

        private IEnumerator ShotShootingProcess(int _shootingPointIndex, int _shotIndex)
        {
            WaitForSeconds wait = new WaitForSeconds(TheShooter.Weapon().TheSniperMachineGunValues.TimeBetweenShots * _shotIndex);
            yield return wait;
            PlayAudio();
            Vector2 target = GetTarget(transform, TheShooter.UserTag);
            Vector2 direction = (target - (Vector2)transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;
            ShootShotGroup(m_Values.ShotsGroup[_shotIndex].Shots, angle, _shootingPointIndex);
        }

        private void ShootShotGroup(List<Shot> _shots, float _angle, int _shootingPointIndex)
        {
            for (int j = 0; j < _shots.Count; j++)
            {
                Vector3 finalAngle = PlayerRotation + Vector3.forward * (_angle + _shots[j].Angle);
                Quaternion dir = Quaternion.Euler(finalAngle);
                Vector2 pos = GetShootingPointPosition(_shootingPointIndex);
                TheShooter.Effects.ShootingMuzzle(pos, dir);
                PoolingSystem.Instance.UseBullet(TheShooter.Weapon().BulletPoolType, pos, dir, _shots[j].Speed, TheShooter.UserTag, TheShooter.Weapon().DamagePerShot, TheShooter.UserIndex);
            }
        }

        /// <summary>
        /// Gets the closest Target to the ship from its opposing side
        /// </summary>
        /// <param name="_bullet"></param>
        /// <param name="_shooter"></param>
        /// <returns></returns>
        public static Vector3 GetTarget(Transform _bullet, BulletUser _user)
        {
            if (_user == BulletUser.Player)
            {
                return WavesManager.i.GetClosestEnemyToPoint(_bullet);
            }
            // if it's an enemy
            else
            {
                return GameManager.i.PlayersManager.GetClosestPlayerToTransform(_bullet.position);
            }
        }
        #endregion
    }
}