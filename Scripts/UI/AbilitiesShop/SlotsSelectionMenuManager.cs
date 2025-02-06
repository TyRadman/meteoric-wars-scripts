using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using SpaceWar.Shop;

namespace SpaceWar.UI.Shop
{
    public class SlotsSelectionMenuManager : MenuWindow, IInput
    {
        private PlayerCoins m_PlayerCoins;
        private int m_SelectedAbilityIndex = 0;
        [Header("References")]
        //[SerializeField] private ShopButton m_ReturnButton;
        [SerializeField] private List<AbilitySlotDisplay> m_AbilitySlots;
        private UISelectable m_CurrentSelectable;
        [SerializeField] private GameObject m_Content;
        [Header("Animation")]
        [SerializeField] private Animation m_Animation;
        [SerializeField] private AnimationClip m_ShowAnimation;
        [SerializeField] private AnimationClip m_HideAnimation;
        private AbilityPrefab m_SelectedAbility;

        public void SetUp()
        {
            m_Content.SetActive(false);
        }

        public void SetSelectedAbility(AbilityPrefab abilityPrefab)
        {
            m_SelectedAbility = abilityPrefab;
        }

        public override void Open(int playerIndex)
        {
            base.Open(playerIndex);

            m_Content.SetActive(true);
            PlayerIndex = playerIndex;
            SetUpInput(playerIndex);
            PlayAnimation(m_ShowAnimation);
            FillSlots();
        }

        public override void Close(int playerIndex)
        {
            base.Close(playerIndex);
            PlayAnimation(m_HideAnimation);
            Invoke(nameof(DisableContent), m_ShowAnimation.length);
            DisposeInput(playerIndex);
            GameManager.i.ShopManager.ShopComponents.Shop.StopInteraction();
        }

        private void DisableContent()
        {
            m_Content.SetActive(false);
        }

        #region Input
        public void SetUpInput(int playerIndex)
        {
            GameManager.i.InputController.EnableInput(ActionMap.UI, playerIndex);
            GamePlayInput c = InputController.Controls;
            InputActionMap UIMap = InputController.GetMap(playerIndex, ActionMap.UI);
            UIMap.FindAction(c.UI.Ability_1.name).performed += SelectFirstSlot;
            UIMap.FindAction(c.UI.Ability_2.name).performed += SelectSecondSlot;
            UIMap.FindAction(c.UI.Ability_3.name).performed += SelectThirdSlot;
            UIMap.FindAction(c.UI.Ability_4.name).performed += SelectFourthSlot;

            string abilityOne = UIMap.FindAction(c.UI.Ability_1.name).GetBindingDisplayString(playerIndex);
            m_AbilitySlots[0].SetKeyText(abilityOne);
            string abilityTwo = UIMap.FindAction(c.UI.Ability_2.name).GetBindingDisplayString(playerIndex);
            m_AbilitySlots[1].SetKeyText(abilityTwo);
            string abilityThree = UIMap.FindAction(c.UI.Ability_3.name).GetBindingDisplayString(playerIndex);
            m_AbilitySlots[2].SetKeyText(abilityThree);
            string abilityFour = UIMap.FindAction(c.UI.Ability_4.name).GetBindingDisplayString(playerIndex);
            m_AbilitySlots[3].SetKeyText(abilityFour);
        }

        public void DisposeInput(int playerIndex)
        {
            GameManager.i.InputController.EnableInput(ActionMap.GamePlay, playerIndex);
            GamePlayInput c = InputController.Controls;
            InputActionMap UIMap = InputController.GetMap(playerIndex, ActionMap.UI);
            UIMap.FindAction(c.UI.Ability_1.name).performed -= SelectFirstSlot;
            UIMap.FindAction(c.UI.Ability_2.name).performed -= SelectSecondSlot;
            UIMap.FindAction(c.UI.Ability_3.name).performed -= SelectThirdSlot;
            UIMap.FindAction(c.UI.Ability_4.name).performed -= SelectFourthSlot;
        }

        #region Input Helper Methods
        public void SelectFirstSlot(InputAction.CallbackContext _)
        {
            SelectSlot(0);
        }
        public void SelectSecondSlot(InputAction.CallbackContext _)
        {
            SelectSlot(1);
        }
        public void SelectThirdSlot(InputAction.CallbackContext _)
        {
            SelectSlot(2);
        }
        public void SelectFourthSlot(InputAction.CallbackContext _)
        {
            SelectSlot(3);
        }
        #endregion
        #endregion

        private void FillSlots()
        {
            PlayerAbilities player = GameManager.i.PlayersManager.GetPlayer(PlayerIndex).Abilities;
            m_AbilitySlots.ForEach(s => s.SetUpSlot(player, m_SelectedAbility));
        }

        private void SelectSlot(int slotIndex)
        {
            m_AbilitySlots[slotIndex].ApplyAbility();
            Close(PlayerIndex);
        }

        private void PlayAnimation(AnimationClip clip)
        {
            if (m_Animation.isPlaying)
            {
                m_Animation.Stop();
            }

            m_Animation.clip = clip;
            m_Animation.Play();
        }
    }
}
