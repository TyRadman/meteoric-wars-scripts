using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    public class ShopDialogue : MonoBehaviour
    {
        [SerializeField] [TextArea(2, 10)] private List<string> m_Messages;
        [SerializeField] private Transform m_DialogueBoxPosition;
        [SerializeField] private float m_MessageDuration = 5f;

        public void TypeMessage()
        {
            DialogueManager.i.TypeMessage(m_Messages.RandomItem(), m_DialogueBoxPosition.position, m_MessageDuration);
        }
    }
}