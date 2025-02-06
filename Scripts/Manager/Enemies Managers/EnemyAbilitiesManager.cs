using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    public class EnemyAbilitiesManager : Singlton<EnemyAbilitiesManager>
    {
        [System.Serializable]
        public struct AbilityStyles
        {
            public EnemyAbilityTag Tag;
            public List<EnemyAbilityPerformer> Styles;
        }

        [System.Serializable]
        public class RankAbility
        {
            public ShipRank Rank;
            public EnemyAbilityPrefab AbilityPrefab;
        }

        public EnemyAbilityPrefab m_SummonerAbility;
        public List<EnemyAbilityPrefab> Abilities;
        [SerializeField] private List<RankAbility> m_ShipAbilities = new List<RankAbility>();
        [Header("Strong Ship Abilities")]
        public List<AbilityStyles> AbilitiesStyles;

        protected override void Awake()
        {
            base.Awake();
            SelectAbilitiesForShips();
        }

        public void AddAbilities(EnemyShipAbilities _ship, ShipRank _rank)
        {
            SpecialAttackValues specialAttackInfo = GameManager.i.GeneralValues.RankValues.Find(v => v.Rank == _rank).SpecialAttackInfo;

            if (_rank == ShipRank.Summoner)
            {
                _ship.SetUpValues(specialAttackInfo, m_SummonerAbility);
            }
            else
            {
                _ship.SetUpValues(specialAttackInfo, m_ShipAbilities.Find(a => a.Rank == _rank).AbilityPrefab);
            }
        }

        public Ability GetAbilityPrefab(ShipRank _rank)
        {
            return m_ShipAbilities.Find(a => a.Rank == _rank).AbilityPrefab.Prefab.GetComponent<EnemyAbility>().AbilityPrefab.GetComponent<Ability>();
        }

        private void SelectAbilitiesForShips()
        {
            m_ShipAbilities.Add(new RankAbility() { Rank = ShipRank.Summoner, AbilityPrefab = m_SummonerAbility });
            m_ShipAbilities.Add(new RankAbility() { Rank = ShipRank.Strong, AbilityPrefab = Abilities.RandomItem() });
            //m_ShipAbilities.Add(ShipRank.Summoner, m_SummonerAbility);
            //m_ShipAbilities.Add(ShipRank.Strong, Abilities.RandomItem());
        }
    }

    public enum EnemyAbilityTag
    {
        LaserShot, Summon, Shield
    }
}