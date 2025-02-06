using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar.UI
{
    public class MenuWindow : MonoBehaviour, IMenuWindow
    {
        public int PlayerIndex { get; protected set; }

        public virtual void Open(int playerIndex)
        {
            PlayerIndex = playerIndex;
        }

        public virtual void Close(int playerIndex)
        {

        }

        public virtual void Navigate(Direction direction)
        {

        }

        public virtual void Select()
        {

        }
        public virtual void Return()
        {

        }
    }
}
