using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    public interface IAbility
    {
        public void Activate();
        public void Deactivate();
        public void Dispose();
        public void ForceStop();
    }
}
