using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar.UI
{
    public interface IMenuWindow
    {
        public void Open(int playerIndex);
        public void Close(int playerIndex);
        public int PlayerIndex { get; }
        public void Navigate(Direction direction);
    }
}
