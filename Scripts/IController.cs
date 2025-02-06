using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    public interface IController
    {
        public void SetUp(IController components);
        public void Activate();
        public void Deactivate();
        public void Dispose();
    }
}
