using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SpaceWar.MainMenu
{
    public class PlayerMainMenuController : MonoBehaviour
    {
        private PlayerNewInputSystem m_Input;
        [SerializeField] private int m_Index;

        private void Awake()
        {
            m_Input = new PlayerNewInputSystem();
            m_Input.Enable();

            if (m_Index == 0)
            {
                m_Input.MainMenuP1.Enable();
                m_Input.MainMenuP1.Navigation.performed += SwitchMenu;
                m_Input.MainMenuP1.Select.performed += SelectButton;
                m_Input.MainMenuP1.Select.performed += SelectButton;
                m_Input.MainMenuP1.Return.performed += ReturnButton;
            }
            else
            {
                m_Input.MainMenuP2.Enable();
                m_Input.MainMenuP2.Navigation.performed += SwitchMenu;
                m_Input.MainMenuP2.Select.performed += SelectButton;
                m_Input.MainMenuP2.Return.performed += ReturnButton;
            }
        }

        private void Start()
        {
            MenuController.i.SwitchInput(MainMenuAnimations.AnimationType.MainMenu);
            SetKeyActionsForText();
        }

        private void SwitchMenu(InputAction.CallbackContext context)
        {
            if (MenuController.i.Actions[m_Index].NavigationAction == null)
            {
                print("Why 1");
                return;
            }

            MenuController.i.Actions[m_Index].NavigationAction(context.ReadValue<Vector2>(), m_Index);
        }

        private void SelectButton(InputAction.CallbackContext context)
        {
            if (MenuController.i.Actions[m_Index].SelectionAction == null)
            {
                print("Why 2");
                return;
            }

            MenuController.i.Actions[m_Index].SelectionAction(m_Index);
        }

        private void ReturnButton(InputAction.CallbackContext context)
        {
            if (MenuController.i.Actions[m_Index].ReturnAction == null)
            {
                print("Why 3");
                return;
            }

            //print(MenuController.Instance.Actions[m_Index].ReturnAction.GetInvocationList()[0].Method.Name + $". Player {m_Index} at {Time.time}");
            MenuController.i.Actions[m_Index].ReturnAction(m_Index);
        }

        private void SetKeyActionsForText()
        {
            string selectKey = m_Index == 0 ? m_Input.MainMenuP1.Select.GetBindingDisplayString(options: InputBinding.DisplayStringOptions.DontIncludeInteractions) :
                m_Input.MainMenuP2.Select.GetBindingDisplayString(options: InputBinding.DisplayStringOptions.DontIncludeInteractions);
            string returnKey = m_Index == 0 ? m_Input.MainMenuP1.Return.GetBindingDisplayString(options: InputBinding.DisplayStringOptions.DontIncludeInteractions) :
                m_Input.MainMenuP2.Return.GetBindingDisplayString(options: InputBinding.DisplayStringOptions.DontIncludeInteractions);
            PlayerSelectionControl.i.SetActionTexts(selectKey, returnKey, m_Index);
        }

        public void DisableInputs()
        {
            m_Input.Disable();
            m_Input.Dispose();
        }
    }
}