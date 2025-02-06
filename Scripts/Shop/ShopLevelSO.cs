using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    [CreateAssetMenu(menuName = "Others/Shop Level Info")]
    public class ShopLevelSO : ScriptableObject
    {
        public enum WeaponAnimation
        {
            ShowWeapon, HideWeapon, ShowWeaponLvl2, HideWeaponLvl2
        }

        [Header("Stats")]
        public float Health;
        public BulletWeapon Weapon;
        [Header("Animation")]
        public WeaponAnimation ShowAnimationHashString;
        public WeaponAnimation HideAnimationHashString;
        [Header("Colliders")]
        public Vector2[] Points;
        [Header("Abilities")]
        public EnemyAbilityPrefab Ability;
        public SpecialAttackValues SpecialAttackValues;
        public int AbilityStyleIndex = 0;
        [Header("Drops")]
        [Tooltip("Each coins has the value of 5")]
        public int CoinsDropped;
    }
}