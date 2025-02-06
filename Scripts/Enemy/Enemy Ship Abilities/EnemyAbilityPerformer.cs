using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    public abstract class EnemyAbilityPerformer : ScriptableObject
    {
        public abstract void SetUp(ShipComponenets components);
        public abstract void Perform();
    }
}