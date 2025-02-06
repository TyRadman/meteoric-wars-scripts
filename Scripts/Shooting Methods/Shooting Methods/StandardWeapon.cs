using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    public class StandardWeapon : WeaponShootingProcess
    {
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
            PlayAudio();

            for (int i = 0; i < ShootingPoints.Count; i++)
            {
                for (int j = 0; j < TheShooter.Weapon().Shots.Count; j++)
                {
                    ShootingStaightBullet(j, i);
                }
            }
        }

        private void PerSpecificShootingPoints()
        {
            PlayAudio();

            for (int i = 0; i < TheShooter.Weapon().ShootingPointsIndices.Count; i++)
            {
                for (int j = 0; j < TheShooter.Weapon().Shots.Count; j++)
                {
                    ShootingStaightBullet(j, TheShooter.Weapon().ShootingPointsIndices[i]);
                }
            }
        }

        private void ThroughAllShootingPoints()
        {
            PlayAudio();

            for (int i = 0; i < TheShooter.Weapon().Shots.Count; i++)
            {
                ShootingStaightBullet(i, GetNextShootingPointIndex(ShootingPoints.Count));
            }
        }

        private void ThroughSpecificShootingPoints()
        {
            PlayAudio();

            for (int i = 0; i < TheShooter.Weapon().Shots.Count; i++)
            {
                int index = GetNextShootingPointIndex(TheShooter.Weapon().ShootingPointsIndices.Count);
                ShootingStaightBullet(i, TheShooter.Weapon().ShootingPointsIndices[index]);
            }
        }

        private void ThroughAllShootingPointsPerSet()
        {
            PlayAudio();

            for (int i = 0; i < TheShooter.Weapon().Shots.Count; i++)
            {
                ShootingStaightBullet(i, ShootingPointIndex);
            }

            ShootingPointIndex = GetNextShootingPointIndex(ShootingPoints.Count);
        }

        private void ThroughSpecificShootingPointPerSet()
        {
            PlayAudio();

            for (int i = 0; i < TheShooter.Weapon().Shots.Count; i++)
            {
                ShootingStaightBullet(i, TheShooter.Weapon().ShootingPointsIndices[ShootingPointIndex]);
            }

            ShootingPointIndex = GetNextShootingPointIndex(TheShooter.Weapon().ShootingPointsIndices.Count);
        }
        #endregion

        #region Shooting Process
        private void ShootingStaightBullet(int _shotIndex, int _shootingPointIndex)
        {
            Quaternion dir = Quaternion.Euler(transform.localEulerAngles + Vector3.forward * TheShooter.Weapon().Shots[_shotIndex].Angle);
            Vector3 pos = GetShootingPointPosition(_shotIndex, _shootingPointIndex);
            TheShooter.Effects.ShootingMuzzle(pos, dir, TheShooter.Weapon().MuzzleTag);
            float bulletSpeed = TheShooter.Weapon().Shots[_shotIndex].Speed;
            BulletUser targetName = Helper.ReverseUser(TheShooter.UserTag);
            PoolObjectTag bulletType = TheShooter.Weapon().BulletPoolType;

            switch (TheShooter.Weapon().TheWeaponTag)
            {
                case WeaponTag.Bullets:
                    {
                        var bullet = PoolingSystem.Instance.GetBullet(bulletType, TheShooter.UserTag);
                        bullet.transform.SetPositionAndRotation(pos, dir);
                        bullet.SetUp(bulletSpeed, targetName, TheShooter.Damage, TheShooter.UserIndex);
                        break;
                    }
                case WeaponTag.Rockets:
                    {
                        var bullet = (RocketBullet)PoolingSystem.Instance.GetBullet(bulletType, TheShooter.UserTag);
                        bullet.transform.SetPositionAndRotation(pos, dir);
                        bullet.SetUpRocketBullet(bulletSpeed, targetName, TheShooter.Damage, TheShooter.Weapon().AreaEffect, TheShooter.UserIndex);
                        break;
                    }
            }
        }
        #endregion
    }
}