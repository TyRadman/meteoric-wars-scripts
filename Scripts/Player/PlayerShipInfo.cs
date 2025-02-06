using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    [CreateAssetMenu(menuName = "Others/Ship Info")]
    public class PlayerShipInfo : ScriptableObject
    {
        public string Name;
        [TextArea(2, 10)] public string Description;
        public float MovementSpeed;
        public float MaxHealth;
        public float Damage;
        public GameObject ShipPrefab;
        public GameObject ShipGraphics;
        public List<AbilityPrefab> Abilities;
    }
}