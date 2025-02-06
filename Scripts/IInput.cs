using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    public interface IInput
    {
        public void SetUpInput(int playerIndex);
        public void DisposeInput(int playerIndex);
    }
}
