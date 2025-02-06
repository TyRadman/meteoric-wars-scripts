using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SpaceWar.UI
{
    public class GridElement : MonoBehaviour
    {
        [SerializeField]
        public List<NextSelectable> NextGrids = new List<NextSelectable>() { new NextSelectable() { Direction = Direction.Left}, new NextSelectable() { Direction = Direction.Right },
        new NextSelectable(){Direction = Direction.Up }, new NextSelectable(){Direction = Direction.Down } };
        public Button Button;
        public bool InvokeButton = false;
        public bool SelectButton = false;

        public void Press()
        {
            if (SelectButton) Button.Select();
            if (InvokeButton) Button.onClick.Invoke();
        }
    }
}
