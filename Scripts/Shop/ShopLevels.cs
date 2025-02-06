using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar.Shop
{
    /// <summary>
    /// Determines what properties and abilities the shop gets based on its level.
    /// </summary>
    public class ShopLevels : MonoBehaviour
    {
        [SerializeField] private ShopComponents m_Shop;

        [SerializeField] private List<ShopLevelSO> m_ShopLevels;

        [SerializeField] private PolygonCollider2D m_Collider;
        [SerializeField] private ShipEffects m_Effect;
        [SerializeField] private EnemyHealth m_Health;
        [SerializeField] private EnemyComponents m_Components;
        [SerializeField] private int m_LevelIndex = 0;

        private void SetLevel(int _level)
        {
            var values = m_ShopLevels[_level];
            // add animations for showing the weapon
            m_Shop.Animation.AddWeaponAnimations(values.ShowAnimationHashString.ToString(), values.HideAnimationHashString.ToString());
            // add collider points
            m_Collider.SetPath(1, m_ShopLevels[_level].Points);
            // set stats
            m_Health.SetMaxHealth(m_ShopLevels[_level].Health);
            m_Components.ShipShooter.SetUpWeapon(m_ShopLevels[_level].Weapon);
            // set abilities
            m_Components.Abilities.ClearAbilities();

            m_Components.Abilities.SetUpValues(values.SpecialAttackValues, values.Ability, values.AbilityStyleIndex);
        }

        public void LevelUp(int _level = -1)
        {
            if (_level == -1) m_LevelIndex++;
            else m_LevelIndex = _level;

            if (m_LevelIndex >= m_ShopLevels.Count) m_LevelIndex = m_ShopLevels.Count - 1;

            SetLevel(m_LevelIndex);
        }

        public ShopLevelSO GetCurrentLevelValues()
        {
            return m_ShopLevels[m_LevelIndex];
        }
    }
}