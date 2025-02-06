using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    /// <summary>
    /// This Class Refreshes the color of the buttons of the slots selection during the abilities selection. This is because the color is changed by the animator.
    /// </summary>
    public class ButtonColorRefresher : MonoBehaviour
    {
        private AbilitySlotDisplay m_Slot;

        private void Awake()
        {
            m_Slot = transform.parent.GetComponent<AbilitySlotDisplay>();
        }
    }
}