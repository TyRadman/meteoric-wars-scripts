using SpaceWar.Shop;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    public class AbilitiesManager : Singlton<AbilitiesManager>
    {
        [System.Serializable]
        public class AbilitiesPrefabsList
        {
            public List<AbilityPrefab> Abilities = new List<AbilityPrefab>();
        }

        public List<List<AbilityPrefab>> AbilitiesPrefabs = new List<List<AbilityPrefab>>();
        [SerializeField] private Vector2Int m_GeneralAbilitiesCountRange = new Vector2Int(2, 4);
        // holds all the abilities that can be displayed
        public List<AbilitiesPrefabsList> AbilitiesToDisplay = new List<AbilitiesPrefabsList>();
        [Header("References")]
        public List<AbilityPrefab> m_StandardAbilities;
        private const float DISPLAY_ONE_SHIP_ABILITY_CHANCE = 1f;
        private int MAX_SPECIAL_ABILITIES_NUMBER = 2;
        public const int MAX_LEVEL = 3;

        #region Set Up
        // sets a set of abilities for player based on what abilities they have, can have, and can't have
        public void SetUpPlayerAbilitiesPrefabs(PlayerShipInfo _info)
        {
            List<AbilityPrefab> allAbilities = new List<AbilityPrefab>();
            List<AbilityPrefab> shipAbilities = new List<AbilityPrefab>();
            List<AbilityPrefab> standardAbilities = new List<AbilityPrefab>();

            // add the player's special abilities
            for (int i = 0; i < _info.Abilities.Count; i++)
            {
                AbilityPrefab prefab = new AbilityPrefab { Prefab = _info.Abilities[i].Prefab, Tag = _info.Abilities[i].Tag, IsMaxed = false, IsGeneral = false };
                shipAbilities.Add(prefab);
            }

            allAbilities.AddRange(shipAbilities);

            // add all the other abilities
            for (int i = 0; i < m_StandardAbilities.Count; i++)
            {
                AbilityPrefab prefab = new AbilityPrefab { Prefab = m_StandardAbilities[i].Prefab, Tag = m_StandardAbilities[i].Tag, IsMaxed = false, IsGeneral = true };
                standardAbilities.Add(prefab);
            }

            allAbilities.AddRange(standardAbilities);

            // add the player abilities stock to the list
            AbilitiesPrefabs.Add(allAbilities);
            AbilitiesToDisplay.Add(new AbilitiesPrefabsList() { Abilities = new List<AbilityPrefab>() });
        }

        public void SetAbilitiesForLevel()
        {
            for (int i = 0; i < GameManager.i.PlayersManager.Players.Count; i++)
            {
                AbilitiesToDisplay[i].Abilities = GetSelectedAbilitiesToDisplay(i);
            }
        }
        #endregion

        #region Displaying Abilities

        /// <summary>
        /// Stores the abilities available for the selected player in a list and returns the abilities. Taking in consideration that maxed abilities have to be ignored.
        /// </summary>
        /// <returns></returns>
        private List<AbilityPrefab> GetSelectedAbilitiesToDisplay(int _playerIndex)
        {
            // the holder of the all the abilities that are gonna be displayed
            List<AbilityPrefab> selectedAbilities = new List<AbilityPrefab>();

            ///// SPECIAL ABILITIES /////
            // the available special abilities to display
            List<AbilityPrefab> availableSpecialAbilities = AbilitiesPrefabs[_playerIndex].FindAll(a => !a.IsGeneral).FindAll(a => !a.IsMaxed);
            int loops = Mathf.Min(MAX_SPECIAL_ABILITIES_NUMBER, availableSpecialAbilities.Count);

            // add the special abilities to display, it's either 2 or 1 or 0
            for (int i = 0; i < loops; i++)
            {
                // that way, there will be at least one guaranteed special ability
                if (Random.value <= 1 + (i * DISPLAY_ONE_SHIP_ABILITY_CHANCE))
                {
                    selectedAbilities.Add(availableSpecialAbilities.RandomItem(true));
                }
            }

            ///// GENERAL ABILITIES /////
            List<AbilityPrefab> availableGeneralAbilities = AbilitiesPrefabs[_playerIndex].FindAll(a => a.IsGeneral).FindAll(a => !a.IsMaxed);
            int numberOfGeneralAbilities = Mathf.Min(availableGeneralAbilities.Count, m_GeneralAbilitiesCountRange.RandomValue());

            for (int i = 0; i < numberOfGeneralAbilities; i++)
            {
                selectedAbilities.Add(availableGeneralAbilities.RandomItem(true));
            }

            // shuffle the abilities
            selectedAbilities.Shuffle();
            List<AbilityPrefab> abilities = new List<AbilityPrefab>();

            // cache them as Ability from the prefabs
            for (int i = 0; i < selectedAbilities.Count; i++)
            {
                AbilityPrefab selectedAbility = selectedAbilities[i];
                abilities.Add(selectedAbility);
            }

            return abilities;
        }
        #endregion

        public void OnAbilityObtained(AbilityPrefab _ability)
        {
            // level up the ability
            _ability.LevelUp();
            // disable it so it's not bought again
            _ability.Purchased = true;
        }
    }

    #region Structs
    [System.Serializable]
    public class AbilityPrefab
    {
        public AbilityTag Tag;
        public Ability Prefab;
        public bool IsMaxed;
        public bool IsGeneral;
        public int Level = 0;
        public bool Purchased = false;

        public void LevelUp()
        {
            if (Prefab.UpgradeType == Ability.AbilityLevelUpType.Upgradable)
            {
                Level++;

                if (Level >= AbilitiesManager.MAX_LEVEL) IsMaxed = true;
            }
        }
    }
    #endregion
}