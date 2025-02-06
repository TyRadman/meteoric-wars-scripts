using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    [CreateAssetMenu(fileName = "AngleShot_NAME", menuName = MAIN_ROOT + "Angled Shots")]
    public class AngleShootingStyle_SO : ShootingStyleValues
    {
        public Vector2 AnglesRange = new Vector2() { x = 30f, y = 60f };
        public Vector2Int ShotsCountRange = new Vector2Int() { x = 2, y = 3 };
    }
}
