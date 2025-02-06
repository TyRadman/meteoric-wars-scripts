using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    public class SpriteTester : MonoBehaviour
    {
        [SerializeField] private Transform m_Child;
        [SerializeField] private SpriteRenderer m_Sprite;
        [SerializeField] private SpriteRandomPointPosition m_RandomizationPosition;

        public void SetChildAtRandomPosition()
        {
            m_Child.position = Helper.GetRandomPositionOnSprite(m_Sprite, m_RandomizationPosition);
        }
    }
}