using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    [CreateAssetMenu(menuName = "Weapons/Bullets Weapon")]
    public class BulletWeapon : Weapon
    {
        public PoolObjectTag BulletPoolType;
        public ParticlePoolTag MuzzleTag;

        #region Generals
        public enum ShootingTypes
        {
            Standard, AngleUpdatePerIndex, Chasing, Sniper, SniperMachineGun
        }
        #endregion

        [Tooltip("Angle Updated Shots must have only one shot in the shots list. If any other shots are included, they will not be considered.")]
        public ShootingTypes ShootingType;
        public float DamagePerShot;
        public ShootingMode TheShootingMode;
        public List<int> ShootingPointsIndices;
        public List<Shot> Shots = new List<Shot>();
        public UpdatedShots UpdatedShotsVariables;
        public ChasingWeaponValues ChasingWeapon;
        public AreaEffectVariables AreaEffect;
        public SniperMachineGunValues TheSniperMachineGunValues;
        public float Difficulty;
    }

    [System.Serializable]
    public struct UpdatedShots
    {
        public float AngleRange;
        public int ShotsNumber;
        [Range(0.01f, 10f)] public float CoolDownTimeBetweenShots;
        public bool HalfLoop;
    }

    [System.Serializable]
    public struct AreaEffectVariables
    {
        public float Area;
        public float AreaDamage;
        public float Duration;
    }

    [System.Serializable]
    public struct SniperMachineGunValues
    {
        public List<ShotGroup> ShotsGroup;
        public float TimeBetweenShots;
    }

    [System.Serializable]
    public class Shot
    {
        public float Angle;
        public float Speed;
        public float OffsetX;

        public void SetUp(float angle, float speed, float offsetX)
        {
            Angle = angle;
            Speed = speed;
            OffsetX = offsetX;
        }
    }

    [System.Serializable]
    public class ShotGroup
    {
        public List<Shot> Shots;
    }
}