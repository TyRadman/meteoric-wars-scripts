using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar.Shop
{
    public class ShopPlayerDetector : MonoBehaviour
    {
        [SerializeField] private Shop m_Shop;

        private void OnTriggerEnter2D(Collider2D other)
        {
            m_Shop.OnPlayerEnter(other.GetComponent<PlayerComponents>().PlayerIndex);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            m_Shop.OnPlayerExit(other.GetComponent<PlayerComponents>().PlayerIndex);
        }
    }
}
