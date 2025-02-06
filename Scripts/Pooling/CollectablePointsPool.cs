using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    [System.Serializable]
    public class CollectablePointsPool
    {
        public CollectableTag Tag;
        public Collectable Prefab;
        public int InitialNumber;
        [HideInInspector] public List<Collectable> Objects = new List<Collectable>();
        [HideInInspector] public Transform Parent;
    }
}