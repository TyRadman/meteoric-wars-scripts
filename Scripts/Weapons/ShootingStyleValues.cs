using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    public abstract class ShootingStyleValues : ScriptableObject
    {
        protected const string MAIN_ROOT = Directories.MAIN + "ShootingStyles/";
        [Range(0f, 1f)] public float Chance = 0.5f;
        public Vector2 CoolDownRange = new Vector2() { x = 0.5f, y = 1f };
    }
}
