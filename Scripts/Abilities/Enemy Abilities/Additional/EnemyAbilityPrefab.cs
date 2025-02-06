using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    /// <summary>
    /// This one has the gameobject that has the enemyAbility, which has a gameobject prefab for the original ability that can be used by either the player or the enemy
    /// </summary>
    [CreateAssetMenu(menuName = "Others/Enemy Ability")]
    public class EnemyAbilityPrefab : ScriptableObject
    {
        public EnemyAbilityTag Tag;
        public GameObject Prefab;
    }
}