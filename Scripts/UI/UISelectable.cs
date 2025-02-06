using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar.UI
{
    public abstract class UISelectable : MonoBehaviour, IUISelectable
    {
        [field : SerializeField] public List<NextSelectable> NextSelectables { get; private set; }
        public bool IsAvailable = false;

        public virtual void Highlight()
        {

        }

        public virtual void Dehighlight()
        {

        }

        public virtual void OnClickStart()
        {

        }

        public virtual void OnClickPerformed()
        {

        }

        public virtual void OnClickFinished()
        {

        }
    }

    [System.Serializable]
    public class NextSelectable
    {
        public Direction Direction;
        public UISelectable Selectable;
    }

    public enum Direction
    {
        Left, Right, Up, Down, NONE
    }
}
