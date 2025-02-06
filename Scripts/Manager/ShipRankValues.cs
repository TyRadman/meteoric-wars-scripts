using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    [CreateAssetMenu(menuName = "Others/Ship Rank")]
    public class ShipRankValues : ScriptableObject
    {
        public ShipRank Rank;
        [Header("Visuals")]
        public Vector2 ShipSize;
        public Vector2Int AdditionalGraphicsLayersRange;
        [Header("Stats")]
        public Vector2 HealthValue;
        public float Score;
        public ScreenFreezer.FreezeStrength FreezeStrength;
        [Header("Shooters")]
        public Vector2Int ShootersNumber;
        [Header("Waves Generation")]
        [Range(1, 15)] public int Value = 1;
        [Header("Weapon")]
        public Vector2Int DamageRange;
        public List<WeaponStyleChances> StyleChances;
        [SerializeReference] public List<ShootingStyleValues> NewStyleChances;
        public Vector2 PerShooterChanceRange;
        public Vector2 BulletSpeedRange;
        public Vector2Int ShotsNumberRange;
        public AngledShots AngledShotsInfo;
        public UpdatedAnglesWeaponVariables UpdatedAnglesValues;
        public SniperMachineGunValuesObject SniperMachineGunValues;
        [Header("Summoners Special Values")]
        public Vector2Int SummonedMinionsRange;
        [Header("After Death Collectables")]
        public List<CollectableInfo> CollectableInfo;
        [Header("Movement")]
        public MovementInformation HorizontalMovementInfo;
        public RandomMovement RandomMovementInfo;
        [Header("Special Attacks")]
        public SpecialAttackValues SpecialAttackInfo;
    }

    #region Movement structures
    [System.Serializable]
    public struct MovementInformation
    {
        [Header("Horizontal Movement")]
        public Vector2 SpeedRange;
        public Vector2 WaitingTime;
        public Vector2Int StepsRange;
    }

    [System.Serializable]
    public struct RandomMovement
    {
        [Header("Random Movement")]
        public Vector2 SpeedRange;
        public Vector2 WaitingTime;
        public Vector2 ChasingPlayerChance;
    }
    #endregion

    #region Shooting Structures
    [System.Serializable]
    public struct UpdatedAnglesWeaponVariables
    {
        public Vector2Int ShotsNumberRange;
        public Vector2 AngleRange;
        public Vector2 CoolDownRange;
        public float PerShooterChance;
        public bool HalfLoop;
    }

    [System.Serializable]
    public struct SniperShootingVariables
    {
        // the speed here should be lower than usual speeds because this weapon is stronger
        public Vector2 Speed;
        public Vector2Int NumberOfShots;
        public Vector2Int BulletsPerShot;
        public Vector2 TimeBetweenShots;
        public Vector2 Angle;
    }


    [System.Serializable]
    public struct AngledShots
    {
        public Vector2 AnglesRange;
        public Vector2Int ShotsRange;
    }
    #endregion

    #region Other Structures
    public enum ShipRank
    {
        Minion = 0, Small = 1, Medium = 2, Strong = 3, Summoner = 4, Shop = 5, Traveller = 6, Boss = 7
    }

    [System.Serializable]
    public struct WeaponStyleChances
    {
        public WeaponsGenerator.WeaponStyles Style;
        [Range(0, 1)] public float Chance;
        public Vector2 CoolDownRange;
    }

    [System.Serializable]
    public struct SpecialAttackValues
    {
        public Vector2 Frequency;
    }
    #endregion

    #region Collectables
    [System.Serializable]
    public struct CollectableInfo
    {
        public CollectableTag Tag;
        public float DropChance;
        public float Value;
    }
    #endregion
}