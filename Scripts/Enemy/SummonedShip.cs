using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    public class SummonedShip : MonoBehaviour
    {
        public int ID;

        /// <summary>
        /// Summoned when the summoned ship is destroyed
        /// </summary>
        public void OnDestroyed(BulletUser _userTag, Transform _ship)
        {
            ShipsPooling.i.ShipDestroyed(ID, _userTag, _ship);
        }
    }
}