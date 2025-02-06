using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    public interface IInteractable
    {
        void Interact(int playerIndex);
        void CancelInteraction();
    }
}
