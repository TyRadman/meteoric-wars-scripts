using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SpaceWar.Unit
{
    public class PlayerInteraction : MonoBehaviour, IController, IInput
    {
        private PlayerComponents m_Components;
        private bool m_HasInteractable = false;
        private IInteractable m_CurrentInteractable;
        [SerializeField] private Constraints m_OnInInteractiveAreaConstraints;

        public void SetUp(IController components)
        {
            m_Components = (PlayerComponents)components;
        }

        #region Input
        public void SetUpInput(int playerIndex)
        {
            GamePlayInput c = InputController.Controls;
            InputActionMap playerMap = InputController.GetMap(playerIndex, ActionMap.GamePlay);
            playerMap.FindAction(c.Gameplay.Interact.name).performed += Interact;
            playerMap.FindAction(c.Gameplay.Return.name).performed += Skip;
        }

        public void DisposeInput(int playerIndex)
        {
            GamePlayInput c = InputController.Controls;
            InputActionMap playerMap = InputController.GetMap(playerIndex, ActionMap.GamePlay);
            playerMap.FindAction(c.Gameplay.Interact.name).performed -= Interact;
            playerMap.FindAction(c.Gameplay.Return.name).performed -= Skip;
        }
        #endregion

        private void Interact(InputAction.CallbackContext context)
        {
            if (!m_HasInteractable)
            {
                return;
            }

            m_CurrentInteractable.Interact(m_Components.PlayerIndex);
        }

        private void Skip(InputAction.CallbackContext _)
        {
            if (!m_HasInteractable)
            {
                return;
            }

            m_CurrentInteractable.CancelInteraction();
        }

        public void OnInteractableAreaEntered(IInteractable interactable)
        {
            m_Components.Constraints.ApplyConstraints(true, m_OnInInteractiveAreaConstraints);
            m_HasInteractable = true;
            m_CurrentInteractable = interactable;
        }

        public void OnInteractableAreaExited()
        {
            m_Components.Constraints.ApplyConstraints(false, m_OnInInteractiveAreaConstraints);
            m_HasInteractable = false;
            m_CurrentInteractable = null;
        }

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
    }
}
