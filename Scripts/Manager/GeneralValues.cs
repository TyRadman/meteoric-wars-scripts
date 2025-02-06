using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpaceWar.Audios;

namespace SpaceWar
{
    public class GeneralValues : MonoBehaviour
    {
        [Header("Ranks Values")]
        public List<ShipRankValues> RankValues;
        [Header("Damage Values")]
        public Color DamageColor;
        public float DamageColorDuration = 0.05f;
        [Header("Entrance Movement")]
        public float EntranceMovementSpeed = 2f;
        public AnimationCurve EntranceMovementSpeedCurve;
        [Header("Bullets")]
        public float UpdateRate = 0.1f;
        // max values
        public float MaxHealth = 1000f;
        public float MaxSpeed = 1000f;
        public float MaxDamagePerSecond = 40f;
        // player pref keys
        [HideInInspector] public string[] LastSelectedShipIndexKey = new string[] { "LSSIK1", "LSSIK2" };
        [Header("Layers")]
        public LayerMask PlayerLayer;
        public LayerMask EnemyLayer;
        public static int PlayerLayerInt = 6;
        public static int EnemyLayerInt = 7;
        public static string PlayerTag = "Player";
        public static string EnemyTag = "Enemy";
        [Header("Display Abilities Colors")]
        public Color SetColor;
        public Color UpgradeColor;
        public Color ReplaceColor;
        public Color MoveColor;
        [Header("Collectables")]
        public float DropChanceMultiplier = 1;
        [Header("Level Up Particles")]
        public GameObject[] LevelUpParticles;
        [Header("Laser")]
        public LaserBullet LaserPrefab;
        [Header("General Audios")]
        public Audio HurtAudio;
        public Audio DeathAudio;
        public PlayerValues PlayerValues;
    }

    [System.Serializable]
    public class PlayerValues
    {
        public float PostDamageProtectionDuration;
    }
}