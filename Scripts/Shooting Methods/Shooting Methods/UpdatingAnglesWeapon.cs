using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    public class UpdatingAnglesWeapon : WeaponShootingProcess
    {
        private float m_AngleIncrement;
        private float m_StartingAngle;
        private WaitForSeconds m_CoolDownWaiting;

        public override void SetUpComponents(Shooter _shooter)
        {
            base.SetUpComponents(_shooter);

            // if the weapon does a full loop i.e starts shooting from the left to the right and then back to the left, then we double the duration of the shots' cool down time
            if (TheShooter.Weapon().UpdatedShotsVariables.HalfLoop)
            {
                CoolingDownTime = TheShooter.Weapon().CoolDownTime + TheShooter.Weapon().UpdatedShotsVariables.CoolDownTimeBetweenShots * TheShooter.Weapon().UpdatedShotsVariables.ShotsNumber;
            }
            else
            {
                CoolingDownTime = TheShooter.Weapon().CoolDownTime + TheShooter.Weapon().UpdatedShotsVariables.CoolDownTimeBetweenShots * TheShooter.Weapon().UpdatedShotsVariables.ShotsNumber * 2;
            }

            m_AngleIncrement = TheShooter.Weapon().UpdatedShotsVariables.AngleRange / (TheShooter.Weapon().UpdatedShotsVariables.ShotsNumber - 1);
            m_StartingAngle = TheShooter.Weapon().UpdatedShotsVariables.AngleRange / -2;
            m_CoolDownWaiting = new WaitForSeconds(TheShooter.Weapon().UpdatedShotsVariables.CoolDownTimeBetweenShots);
        }

        #region Shooting Mode Methods
        public override void SetUpShootingAction()
        {
            base.SetUpShootingAction();

            switch (TheShooter.Weapon().TheShootingMode)
            {
                case ShootingMode.PerAllSP:
                    {
                        ShootingAction = ShootForEveryShootingPoint;
                        break;
                    }
                case ShootingMode.PerSpecificSP:
                    {
                        ShootingAction = ShootForEverySpecifiedShootingPoint;
                        break;
                    }
                case ShootingMode.ThroughAllSPPerShot:
                    {
                        ShootingAction = ShootFromShootingPointsOneByOne;
                        break;
                    }
                case ShootingMode.ThroughSpecificSPPerShot:
                    {
                        ShootingAction = ShootFromSpecificShootingPointsOneByOne;
                        break;
                    }
                case ShootingMode.ThroughAllSPPerSet:
                    {
                        ShootingAction = ShootFromShootingPointsOneByOne;
                        break;
                    }
                case ShootingMode.ThroughtSpecificSPPerSet:
                    {
                        ShootingAction = ShootFromSpecificShootingPointsOneByOne;
                        break;
                    }
            }
        }

        private void ShootForEveryShootingPoint()
        {
            int loops = ShootingPoints.Count;

            for (int i = 0; i < loops; i++)
            {
                StartCoroutine(UpdatedAngleShootingProcess(i));
            }
        }

        private void ShootForEverySpecifiedShootingPoint()
        {
            int loops = TheShooter.Weapon().ShootingPointsIndices.Count;

            for (int i = 0; i < loops; i++)
            {
                StartCoroutine(UpdatedAngleShootingProcess(TheShooter.Weapon().ShootingPointsIndices[i]));
            }
        }

        private void ShootFromShootingPointsOneByOne()
        {
            StartCoroutine(UpdatedAngleShootingProcess(GetNextShootingPointIndex(ShootingPoints.Count)));
        }

        private void ShootFromSpecificShootingPointsOneByOne()
        {
            StartCoroutine(UpdatedAngleShootingProcess(TheShooter.Weapon().ShootingPointsIndices[ShootingPointIndex]));
        }
        #endregion

        #region Shooting Process
        private IEnumerator UpdatedAngleShootingProcess(int _shootingPointIndex)
        {
            float totalAngles;
            // determines whether the shooting flow goes left or right
            float multiplier;
            int shots = TheShooter.Weapon().UpdatedShotsVariables.ShotsNumber;
            PoolObjectTag bulletTag = TheShooter.Weapon().BulletPoolType;
            WeaponTag weaponTag = TheShooter.Weapon().TheWeaponTag;
            int loops = TheShooter.Weapon().UpdatedShotsVariables.HalfLoop ? 1 : 2;

            for (int i = 0; i < loops; i++)
            {
                // if it's the starting loop, then the shots start going left, otherwise, right
                if (i % 2 == 0)
                {
                    totalAngles = m_StartingAngle;
                    multiplier = 1f;
                }
                else
                {
                    totalAngles = -m_StartingAngle - m_AngleIncrement;
                    multiplier = -1f;
                    shots--;
                }

                for (int j = 0; j < shots; j++)
                {
                    // play audio
                    PlayAudio();

                    // rotation
                    Quaternion muzzleDir = Quaternion.Euler(transform.localEulerAngles);
                    // position
                    Vector3 pos = GetShootingPointPosition(0, _shootingPointIndex);
                    // graphics 
                    TheShooter.Effects.ShootingMuzzle(pos, muzzleDir, TheShooter.Weapon().MuzzleTag);
                    ChooseBullet(pos, totalAngles, 0, bulletTag, weaponTag);
                    // will add up to the total value
                    totalAngles += m_AngleIncrement * multiplier;
                    yield return m_CoolDownWaiting;
                }
            }
        }

        private void ChooseBullet(Vector3 _position, float _angle, int _shotIndex, PoolObjectTag _bulletTag, WeaponTag _weaponTag)
        {
            int shotsNumber = TheShooter.Weapon().Shots.Count;

            for (int i = 0; i < shotsNumber; i++)
            {
                BulletUser targetName = Helper.ReverseUser(TheShooter.UserTag);
                float bulletSpeed = TheShooter.Weapon().Shots[i].Speed;
                Quaternion direction = Quaternion.Euler(transform.localEulerAngles + Vector3.forward * (_angle + TheShooter.Weapon().Shots[i].Angle));

                switch (_weaponTag)
                {
                    case WeaponTag.Bullets:
                        {
                            var bullet = PoolingSystem.Instance.GetBullet(_bulletTag, TheShooter.UserTag);
                            bullet.transform.SetPositionAndRotation(_position, direction);
                            bullet.SetUp(bulletSpeed, targetName, TheShooter.Damage, TheShooter.UserIndex);
                            break;
                        }
                    case WeaponTag.Rockets:
                        {
                            var bullet = (RocketBullet)PoolingSystem.Instance.GetBullet(_bulletTag, TheShooter.UserTag);
                            bullet.transform.SetPositionAndRotation(_position, direction);
                            bullet.SetUpRocketBullet(bulletSpeed, targetName, TheShooter.Damage, TheShooter.Weapon().AreaEffect);
                            break;
                        }
                }
            }
        }
        #endregion
    }
}