using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar.Shop
{
    public class ShopManager : Singlton<ShopManager>
    {
        public int ShopShowRate = 2;
        
        [HideInInspector] public ShopComponents ShopComponents;
        
        [SerializeField] private ShopComponents m_ShopReference;
        
        private const float COIN_VALUE = 0.35f;

        public void SetUp()
        {
            ShopComponents = Instantiate(m_ShopReference, transform.position, Quaternion.Euler(0f, 0f, 180f), transform);
            ShopComponents.name = "Galactical Shop";
            ShopComponents.gameObject.SetActive(false);
            ShopComponents.GetComponent<EnemyComponents>().EnemyStats.OnDeathAction += OnShopDeath;
            ShopComponents.Levels.LevelUp(0);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                SpawnShop();
            }
        }

        public void SpawnShop()
        {
            ShopComponents.gameObject.SetActive(true);
            ShopComponents.Shop.SpawnShop();
        }

        public void StopAggressiveMode()
        {
            if (!ShopComponents.Shop.IsPresent) return;

            ShopComponents.GetComponent<OffensiveShop>().StopAggressiveMode();
            ShopComponents.Shop.MoveShopAway();
        }

        public bool ShopIsPresent()
        {
            return ShopComponents.Shop.IsPresent;
        }

        public void OnShopDeath()
        {
            // spawn collectables
            CollectableSpawner.i.SpawnCollectableWithValues(ShopComponents.transform.position, CollectableTag.ShooterPoints, 1f);
            int coinsCount = ShopComponents.Levels.GetCurrentLevelValues().CoinsDropped;
            CollectableSpawner.i.SpawnCollectableWithValues(ShopComponents.transform.position, CollectableTag.Coins, COIN_VALUE, coinsCount, 2f);
            // stop aggressive mode
            StopAggressiveMode();
            // turn off the shop's presence so that it can be spawned again
            ShopComponents.Shop.IsPresent = false;
            // level up
            ShopComponents.Levels.LevelUp();
        }

        public void SetShopUsage(bool _isUsed)
        {
            ShopComponents.Shop.UsingShop = _isUsed;
        }
    }
}