using SpaceWar.Shop;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SpaceWar.UI.Shop
{
    public class AbilitiesShopMenu : MenuWindow, IInput
    {
        private PlayerCoins m_PlayerCoins;
        private int m_SelectedAbilityIndex = 0;
        [Header("References")]
        //[SerializeField] private ShopButton m_ReturnButton;
        [SerializeField] private List<AbilitySelectionUnit> m_AbilitySelectionSlots;
        private AbilitySelectionUnit m_CurrentSelectable;
        [SerializeField] private GameObject m_Content;
        [SerializeField] private AbilitiesInfoDisplayer m_InfoDisplayer;
        [SerializeField] private SlotsSelectionMenuManager m_SlotsMenu;
        [Header("Animation")]
        [SerializeField] private Animation m_Animation;
        [SerializeField] private AnimationClip m_ShowAnimation;
        [SerializeField] private AnimationClip m_HideAnimation;
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI m_PlayerNumberText;
        [SerializeField] private TextMeshProUGUI m_MessageText;
        [SerializeField] private TextMeshProUGUI m_InstructionsText;
        [SerializeField] private TextMeshProUGUI m_CoinsText;
        private const float DISPLAY_MESSAGE_DURATION = 3f;
        private const string PLAYER_NUMBER_PREFIX = "Player";

        public void SetUp()
        {
            m_Content.SetActive(false);
            m_SlotsMenu.SetUp();
            DisableMessageText();
        }

        public override void Open(int playerIndex)
        {
            base.Open(playerIndex);
            print("Opened");
            SetUpInput(playerIndex);
            m_Content.SetActive(true);
            m_PlayerNumberText.text = $"{PLAYER_NUMBER_PREFIX} {PlayerIndex + 1}";
            PlayAnimation(m_ShowAnimation);
            m_SelectedAbilityIndex = 0;
            FillAbilitySlotsWithAbilityData();

            if (m_CurrentSelectable != null)
            {
                m_CurrentSelectable.Dehighlight();
            }

            // set first ability as default selectable
            m_CurrentSelectable = m_AbilitySelectionSlots[0];
            m_CurrentSelectable.Highlight();
            UpdateAbilityProperties();

            GameManager.i.EnemyHealthBar.HideHealthBar();

            m_PlayerCoins = GameManager.i.PlayersManager.Players[PlayerIndex].Components.Coins;
            UpdateCoins();
        }

        public override void Close(int playerIndex)
        {
            base.Close(playerIndex);
            PlayAnimation(m_HideAnimation);
            Invoke(nameof(DisableContent), m_ShowAnimation.length);
            DisposeInput(playerIndex);
        }

        private void DisableContent()
        {
            m_Content.SetActive(false);
        }

        #region Input
        public void SetUpInput(int playerIndex)
        {
            // disable other input
            GameManager.i.InputController.EnableInput(ActionMap.Empty, playerIndex);
            GameManager.i.InputController.EnableInput(ActionMap.UI, playerIndex);

            GamePlayInput c = InputController.Controls;
            InputActionMap UIMap = InputController.GetMap(playerIndex, ActionMap.UI);
            UIMap.FindAction(c.UI.Left.name).performed += Navigation_Left;
            UIMap.FindAction(c.UI.Right.name).performed += Navigation_Right;
            UIMap.FindAction(c.UI.Up.name).performed += Navigation_Up;
            UIMap.FindAction(c.UI.Down.name).performed += Navigation_Down;
            UIMap.FindAction(c.UI.Submit.name).performed += Select;
            UIMap.FindAction(c.UI.Cancel.name).performed += Return;

            string selectButton = UIMap.FindAction(c.UI.Submit.name).GetBindingDisplayString(playerIndex);
            string returnButton = UIMap.FindAction(c.UI.Cancel.name).GetBindingDisplayString(playerIndex);
            m_InstructionsText.text = $"Select: {selectButton}\nReturn: {returnButton}";
        }

        public void DisposeInput(int playerIndex)
        {
            GameManager.i.InputController.EnableInput(ActionMap.GamePlay, playerIndex);
            GamePlayInput c = InputController.Controls;
            InputActionMap UIMap = InputController.GetMap(playerIndex, ActionMap.UI);
            UIMap.FindAction(c.UI.Left.name).performed -= Navigation_Left;
            UIMap.FindAction(c.UI.Right.name).performed -= Navigation_Right;
            UIMap.FindAction(c.UI.Up.name).performed -= Navigation_Up;
            UIMap.FindAction(c.UI.Down.name).performed -= Navigation_Down;
            UIMap.FindAction(c.UI.Submit.name).performed -= Select;
            UIMap.FindAction(c.UI.Cancel.name).performed -= Return;
        }

        #region Helper Input Methods
        private void Navigation_Left(InputAction.CallbackContext _)
        {
            Navigate(Direction.Left);
        }
        private void Navigation_Up(InputAction.CallbackContext _)
        {
            Navigate(Direction.Up);
        }
        private void Navigation_Right(InputAction.CallbackContext _)
        {
            Navigate(Direction.Right);
        }
        private void Navigation_Down(InputAction.CallbackContext _)
        {
            Navigate(Direction.Down);
        }
        private void Select(InputAction.CallbackContext _)
        {
            Select();
        }
        private void Return(InputAction.CallbackContext _)
        {
            Return();
        }
        #endregion
        #endregion

        public override void Navigate(Direction direction)
        {
            base.Navigate(direction);

            UISelectable selectable = m_CurrentSelectable.NextSelectables.Find(s => s.Direction == direction).Selectable;
            
            if(selectable == null || !selectable.IsAvailable)
            {
                return;
            }

            m_CurrentSelectable.Dehighlight();
            selectable.Highlight();
            m_CurrentSelectable = (AbilitySelectionUnit)selectable;
            UpdateAbilityProperties();
        }

        private void UpdateAbilityProperties()
        {
            Ability ability = m_CurrentSelectable.AbilityPrefab.Prefab;
            int level = GameManager.i.PlayersManager.Players[PlayerIndex].Components.Abilities.GetAbilityLevel(ability.Tag) + 1;
            m_InfoDisplayer.FillInformation(ability.Name, ability.Description);
            
            //if(level >= AbilitiesManager.MAX_LEVEL)
            //{
            //    m_InfoDisplayer.ClearProperties();
            //    return;
            //}

            // if the ability is additive i.e doesn't have upgrades then we will always use the index of 0 to get its properties
            if (ability.UpgradeType == Ability.AbilityLevelUpType.Addaditve)
            {
                level = 0;
            }

            m_CurrentSelectable.SetLevel(level);
            List<string> properties = new List<string>();

            // fill the ability properties
            for (int i = 0; i < ability.AbilityInfo.Count; i++)
            {
                string name = $"<b>{ability.AbilityInfo[i].Name}</b>";
                string value;
                string type;

                if (level >= AbilitiesManager.MAX_LEVEL)
                {
                    value = "Already maxed out.";
                    type = string.Empty;
                }
                else
                {
                    value = ability.AbilityInfo[i].Values[level].Color(ability.AbilityInfo[i].ValueColor);
                    type = ability.AbilityInfo[i].ValueType;
                }

                properties.Add($"{name}: {value} {type}");
            }

            m_InfoDisplayer.FillAbilityProperties(properties);
        }

        public override void Select()
        {
            base.Select();

            int abilityPrice = m_CurrentSelectable.AbilityPrefab.Prefab.Price;

            // check if the player has enough coins
            if (m_PlayerCoins.GetCurrentCoins() < abilityPrice)
            {
                DisplayMessage("Not enough coins");
                return;
            }

            m_PlayerCoins.AddCoins(-abilityPrice);
            UpdateCoins();

            m_SlotsMenu.SetSelectedAbility(m_CurrentSelectable.AbilityPrefab);
            Close(PlayerIndex);
            m_SlotsMenu.Open(PlayerIndex);
        }

        public override void Return()
        {
            base.Return();
            GameManager.i.ShopManager.ShopComponents.Shop.StopInteraction();
            Close(PlayerIndex);
        }

        #region Extra
        private void PlayAnimation(AnimationClip clip)
        {
            if (m_Animation.isPlaying)
            {
                m_Animation.Stop();
            }

            m_Animation.clip = clip;
            m_Animation.Play();
        }

        private void FillAbilitySlotsWithAbilityData()
        {
            m_AbilitySelectionSlots.ForEach(a => a.Deactivate());
            List<AbilityPrefab> abilities = AbilitiesManager.i.AbilitiesToDisplay[PlayerIndex].Abilities;

            for (int i = 0; i < abilities.Count; i++)
            {
                AbilityPrefab ability = abilities[i];
                m_AbilitySelectionSlots[i].ApplyAbility(ability);
            }
        }

        private void DisplayMessage(string message)
        {
            m_MessageText.enabled = true;
            m_MessageText.text = message;
            Invoke(nameof(DisableMessageText), DISPLAY_MESSAGE_DURATION);
        }

        private void DisableMessageText()
        {
            m_MessageText.enabled = false;
        }

        private void UpdateCoins()
        {
            m_CoinsText.text = m_PlayerCoins.GetCurrentCoins().ToString();
        }
        #endregion
    }
}
