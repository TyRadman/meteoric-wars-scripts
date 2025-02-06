using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpaceWar.UI.HUD;

namespace SpaceWar
{
    public class PlayerCoins : MonoBehaviour, IController
    {
        [SerializeField] private int m_Coins = 0;
        [SerializeField] private PlayerComponents m_Components;
        [SerializeField] private ParticlesPooling m_CoinParticles;

        public void SetUp(IController components)
        {
            m_Components = components as PlayerComponents;

            AddCoins(0);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Coin"))
            {
                // make the coin disappear
                collision.gameObject.SetActive(false);
                // player effects
                PoolingSystem.Instance.UseParticles(m_CoinParticles, collision.transform.position);
            }
        }

        public void AddCoins(int _amount)
        {
            m_Coins += _amount;
            GameManager.i.HUDManager.UpdateCoins(m_Coins, m_Components.PlayerIndex);
        }

        public int GetCurrentCoins()
        {
            return m_Coins;
        }

        public void Activate()
        {

        }

        public void Deactivate()
        {
            throw new System.NotImplementedException();
        }

        public void Dispose()
        {
            throw new System.NotImplementedException();
        }
    }
}