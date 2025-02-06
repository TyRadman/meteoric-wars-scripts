using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    public class SniperWeapon : WeaponShootingProcess
    {
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
                    shootingAimingBullet(j, i);
                }
            }
        }

        private void PerSpecificShootingPoints()
        {
            for (int i = 0; i < TheShooter.Weapon().ShootingPointsIndices.Count; i++)
            {
                for (int j = 0; j < TheShooter.Weapon().Shots.Count; j++)
                {
                    shootingAimingBullet(j, TheShooter.Weapon().ShootingPointsIndices[i]);
                }
            }
        }

        private void ThroughAllShootingPoints()
        {
            for (int i = 0; i < TheShooter.Weapon().Shots.Count; i++)
            {
                shootingAimingBullet(i, GetNextShootingPointIndex(ShootingPoints.Count));
            }
        }

        private void ThroughSpecificShootingPoints()
        {
            for (int i = 0; i < TheShooter.Weapon().Shots.Count; i++)
            {
                int index = GetNextShootingPointIndex(TheShooter.Weapon().ShootingPointsIndices.Count);
                shootingAimingBullet(i, TheShooter.Weapon().ShootingPointsIndices[index]);
            }
        }

        private void ThroughAllShootingPointsPerSet()
        {
            for (int i = 0; i < TheShooter.Weapon().Shots.Count; i++)
            {
                shootingAimingBullet(i, ShootingPointIndex);
            }

            ShootingPointIndex = GetNextShootingPointIndex(ShootingPoints.Count);
        }

        private void ThroughSpecificShootingPointPerSet()
        {
            for (int i = 0; i < TheShooter.Weapon().Shots.Count; i++)
            {
                shootingAimingBullet(i, TheShooter.Weapon().ShootingPointsIndices[ShootingPointIndex]);
            }

            ShootingPointIndex = GetNextShootingPointIndex(TheShooter.Weapon().ShootingPointsIndices.Count);
        }
        #endregion

        #region Main Functionality
        private void shootingAimingBullet(int _index, int _shootingPointIndex)
        {
            PlayAudio();
            Vector3 target = GetTarget(transform, TheShooter.UserTag);
            Vector2 direction = (target - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;
            Quaternion dir = Quaternion.Euler(transform.localEulerAngles + Vector3.forward * (angle + TheShooter.Weapon().Shots[_index].Angle));
            Vector3 pos = GetShootingPointPosition(_index, _shootingPointIndex);
            TheShooter.Effects.ShootingMuzzle(pos, dir);
            PoolingSystem.Instance.UseBullet(TheShooter.Weapon().BulletPoolType, pos, dir, TheShooter.Weapon().Shots[_index].Speed, TheShooter.UserTag, TheShooter.Weapon().DamagePerShot, TheShooter.UserIndex);
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