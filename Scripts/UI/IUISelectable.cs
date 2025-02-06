using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar.UI
{
    public interface IUISelectable
    {
        public List<NextSelectable> NextSelectables { get; }
        public void Highlight();
        public void Dehighlight();
    }
}
