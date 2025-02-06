using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace SpaceWar
{
    public class PlayerAbilities : MonoBehaviour, IController, IInput
    {
        private const int ABILITIES_NUMBER = 4;
        private PlayerComponents m_Components;
        // always fixed to the number of possible abilities at once
        public List<AbilityInfo> Abilities = new List<AbilityInfo>();
        // to keep the heirarchy clean
        private Transform m_AbilitiesParent;
        private List<Ability> m_CreatedAbilities = new List<Ability>();
        private List<PlayerAbilityUIElement> m_AbilitySlots = new List<PlayerAbilityUIElement>();
        private const string ABILITY_INPUT_ID = "Ability_";

        /// <summary>
        /// Holds informations about abilities for the player such as the ability, the key to activate the ability, the slot its displayed in and the game object of the ability that is instantiated in the game.
        /// </summary>
        [System.Serializable]
        public class AbilityInfo
        {
            public Ability TheAbility;
            public KeyCode Key;
            public Transform Ship;
            public UnityAction AbilityAction;
            public PlayerAbilityUIElement Slot;
            public GameObject AbilityObject;

            public void SetSlot()
            {
                TheAbility.SetSlot(Slot);
            }

            public void UpdateAbility()
            {
                TheAbility.UpdateAbility();
            }
        }

        public void SetUp(IController components)
        {
            m_Components = (PlayerComponents)components;
            m_AbilitiesParent = new GameObject("Abilities").transform;
            m_AbilitiesParent.parent = transform;
            SetUpAbilitySlots();
            SetUpPlayerAbilities();

        }

        #region Input
        public void SetUpInput(int playerIndex)
        {
            GamePlayInput c = InputController.Controls;
            InputActionMap UIMap = InputController.GetMap(playerIndex, ActionMap.GamePlay);
            UIMap.FindAction(c.Gameplay.Ability_1.name).performed += ActivateAbility_One;
            UIMap.FindAction(c.Gameplay.Ability_2.name).performed += ActivateAbility_Two;
            UIMap.FindAction(c.Gameplay.Ability_3.name).performed += ActivateAbility_Three;
            UIMap.FindAction(c.Gameplay.Ability_4.name).performed += ActivateAbility_Four;
        }

        public void DisposeInput(int playerIndex)
        {
            GamePlayInput c = InputController.Controls;
            InputActionMap UIMap = InputController.GetMap(playerIndex, ActionMap.GamePlay);
            UIMap.FindAction(c.Gameplay.Ability_1.name).performed -= ActivateAbility_One;
            UIMap.FindAction(c.Gameplay.Ability_2.name).performed -= ActivateAbility_Two;
            UIMap.FindAction(c.Gameplay.Ability_3.name).performed -= ActivateAbility_Three;
            UIMap.FindAction(c.Gameplay.Ability_4.name).performed -= ActivateAbility_Four;
        }

        #region Input Helping Methods
        public void ActivateAbility_One(InputAction.CallbackContext _)
        {
            ActivateAbility(0);
        }

        public void ActivateAbility_Two(InputAction.CallbackContext _)
        {
            ActivateAbility(1);
        }

        public void ActivateAbility_Three(InputAction.CallbackContext _)
        {
            ActivateAbility(2);
        }

        public void ActivateAbility_Four(InputAction.CallbackContext _)
        {
            ActivateAbility(3);
        }
        #endregion
        #endregion

        private void SetUpAbilitySlots()
        {
            int playerIndex = m_Components.PlayerIndex;
            InputActionMap UIMap = InputController.GetMap(playerIndex, ActionMap.GamePlay);

            for (int i = 0; i < ABILITIES_NUMBER; i++)
            {
                PlayerAbilityUIElement slot = GameManager.i.PlayersManager.AbilitySlots[playerIndex].Elements[i];
                m_AbilitySlots.Add(slot);
                // set the key that activates the ability at the current slot
                string abilityInputKey = UIMap.FindAction($"{ABILITY_INPUT_ID}{i + 1}").GetBindingDisplayString(playerIndex);
                slot.SetKeyText(abilityInputKey);
            }
        }

        public void ActivateAbility(int abilityIndex)
        {
            if(Abilities[abilityIndex].TheAbility == null)
            {
                return;
            }

            Abilities[abilityIndex].TheAbility.Activate();
        }

        public void SetUpPlayerAbilities()
        {
            // creating the abilities holder and setting it as a child to the player
            m_AbilitiesParent = new GameObject("Abilities").transform;
            m_AbilitiesParent.parent = transform;

            if (AbilitiesManager.i == null)
            {
                return;
            }

            for (int i = 0; i < ABILITIES_NUMBER; i++)
            {
                AbilityInfo ability = new AbilityInfo
                {
                    Ship = transform,
                    Slot = m_AbilitySlots[i]
                };

                Abilities.Add(ability);
            }
        }

        #region Add Ability Functions
        /// <summary>
        /// Adds an ability to the player.
        /// </summary>
        /// <param name="_ability">The ability</param>
        /// <param name="_slotIndex">The slot index at which the ability will be displayed</param>
        public void AddAbility(Ability _ability, int _abilitySlotIndex = 0)
        {
            // creating an instance of the ability
            var ability = InstantiateAbilityObject(_ability);
            AbilityInfo abilityInfo = Abilities[_abilitySlotIndex];
            abilityInfo.AbilityObject = ability.gameObject;
            // add the abilities script to the list of abilities
            abilityInfo.TheAbility = ability.GetComponent<Ability>();
            // set up the ability
            abilityInfo.SetSlot();
            // set a slot for the ability
            abilityInfo.TheAbility.SetUp(transform);
            // set the action to be used by the input manager
            abilityInfo.AbilityAction = abilityInfo.TheAbility.Activate;
        }
        #endregion

        #region Move Ability
        public void MoveAbility(AbilityTag _tag, int _newIndex)
        {
            AbilityInfo previousAbility = Abilities.FindAll(a => a.TheAbility != null).Find(a => a.TheAbility.Tag == _tag);
            PlayerAbilityUIElement slot = previousAbility.TheAbility.Slot;
            AbilityInfo newAbility = Abilities[_newIndex];
            // transfer the ability object reference and the ability itself and level the ability up
            newAbility.AbilityObject = previousAbility.AbilityObject;
            newAbility.TheAbility = previousAbility.TheAbility;
            previousAbility.AbilityObject = null;
            previousAbility.TheAbility = null;
            // set the slot up and update the ability
            newAbility.SetSlot();
            newAbility.UpdateAbility();
            // clear previous slot
            slot.ClearSlot();
        }
        #endregion

        #region Update Ability Functions
        public void UpdateAbility(int _slotIndex)
        {
            var upgradeType = Abilities[_slotIndex].TheAbility.UpgradeType;
            Abilities[_slotIndex].SetSlot();
            Abilities[_slotIndex].UpdateAbility();
        }
        #endregion

        #region Replace Old Ability
        public void ReplaceAbility(AbilityPrefab _newAbility, int _selectedSlotIndex)
        {
            // remove any effects that the older ability might have had
            var abilityToRemove = Abilities[_selectedSlotIndex].TheAbility;
            abilityToRemove.ForceStop();

            // check if the ability already exists in a different slot
            if (Abilities.FindAll(a => a.TheAbility != null).Exists(a => a.TheAbility.Tag == _newAbility.Tag))
            {
                var existingAbility = Abilities.FindAll(a => a.TheAbility != null).Find(a => a.TheAbility.Tag == _newAbility.Tag);
                Abilities[_selectedSlotIndex].AbilityObject = existingAbility.AbilityObject;
                Abilities[_selectedSlotIndex].TheAbility = existingAbility.TheAbility;
                Abilities[_selectedSlotIndex].Slot.ClearSlot();
                Abilities[_selectedSlotIndex].SetSlot();
                Abilities[_selectedSlotIndex].UpdateAbility();
                // remove the ui of the already existing ability from its pervious slot
                existingAbility.Slot.ClearSlot();
                existingAbility.TheAbility = null;
                existingAbility.AbilityObject = null;
            }
            // if we're adding a new ability that we don't have currently
            else
            {
                var ability = InstantiateAbilityObject(_newAbility.Prefab);
                var abilityInfo = Abilities[_selectedSlotIndex];
                // reset the slot if there was a count down
                abilityInfo.TheAbility.Slot.ClearSlot();
                // add the abilities script to the list of abilities
                abilityInfo.TheAbility = ability.GetComponent<Ability>();
                // set a slot for the ability
                abilityInfo.SetSlot();
                // set up the ability
                abilityInfo.TheAbility.SetUp(transform);
                // set the action to be used by the input manager
                abilityInfo.AbilityAction = abilityInfo.TheAbility.Activate;
            }
        }
        #endregion

        #region Remove Ability
        public void RemoveAbility(AbilityTag _tag)
        {
            AbilityInfo previousAbility = Abilities.FindAll(a => a.TheAbility != null).Find(a => a.TheAbility.Tag == _tag);
            PlayerAbilityUIElement slot = previousAbility.TheAbility.Slot;
            // clear previous slot
            slot.ClearSlot();
            previousAbility.TheAbility.AbilityLevel = -1;
            previousAbility.AbilityObject = null;
            previousAbility.TheAbility = null;
        }
        #endregion

        private Ability InstantiateAbilityObject(Ability _prefab)
        {
            if (!m_CreatedAbilities.Exists(a => a.GetComponent<Ability>().Tag == _prefab.Tag))
            {
                Ability ability = Instantiate(_prefab, m_AbilitiesParent);
                m_CreatedAbilities.Add(ability);
                return ability;
            }
            else
            {
                var ability = m_CreatedAbilities.Find(a => a.GetComponent<Ability>().Tag == _prefab.GetComponent<Ability>().Tag);
                ability.GetComponent<Ability>().SetLevelValues(0);
                return ability;
            }
        }

        public int GetAbilityLevel(AbilityTag _abilityTag)
        {
            var abilities = Abilities.FindAll(a => a.TheAbility != null);

            // if the player has the ability then return its level, otherwise, return -1
            if (abilities.Count > 0 && abilities.Exists(a => a.TheAbility.Tag == _abilityTag))
            {
                return abilities.Find(a => a.TheAbility.Tag == _abilityTag).TheAbility.AbilityLevel;
            }

            return -1;
        }

        #region IController
        public void Activate()
        {
            SetUpInput(m_Components.PlayerIndex);
        }

        public void Deactivate()
        {
            DisposeInput(m_Components.PlayerIndex);
        }

        public void Dispose()
        {
            DisposeInput(m_Components.PlayerIndex);
        }
        #endregion
    }
}