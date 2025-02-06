using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    [System.Serializable]
    public class BulletPoolingObject
    {
        public PoolObjectTag Name;
        public BulletUser User;
        public Color BulletColor;
        public GameObject Prefab;
        public int InitialNumber;
        public ParticlePoolTag ImpactPoolTag;
        public List<Bullet> Objects;
        [HideInInspector] public Transform Parent;
    }

    public enum BulletUser
    {
        Player = 0, Enemy = 1
    }
}