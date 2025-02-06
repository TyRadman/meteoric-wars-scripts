using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar.Shop
{
    public class ShopComponents : MonoBehaviour
    {
        [field: SerializeField] public Shop Shop;
        [field: SerializeField] public OffensiveShop OffensiveShop;
        [field: SerializeField] public ShopAnimation Animation;
        [field: SerializeField] public ShopLevels Levels;
        [field: SerializeField] public ShopDialogue Dialogue;
    }
}