using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    public interface IPooler
    {
        public void CreatePools();
        public void AddItemToPool();
    }
}
