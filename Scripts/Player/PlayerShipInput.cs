using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SpaceWar
{
    public class PlayerShipInput : MonoBehaviour
    {

        [SerializeField] private GamePlayInput _input;
        [SerializeField] private List<KeyAction> m_KeyActions;
        [SerializeField] private PlayerComponents m_Components;
        [SerializeField] private PlayerControllerInput m_ControllerInput;
        public bool Active = false;

        #region Set Up
        public void SetUpButtons(KeysSet _set)
        {
            for (int i = 0; i < _set.Keys.Count; i++)
            {
                m_KeyActions[i].Button = _set.Keys.Find(k => k.Tag == m_KeyActions[i].Tag).TheButton;
            }
        }
        #endregion


        #region Additional Methods
        public void EnableKeyUsage(KeyTag _tag, bool _enable)
        {
            m_KeyActions.Find(k => k.Tag == _tag).CanBeUsed = _enable;
        }

        public void Activate(bool _enable)
        {
            Active = _enable;
        }
        #endregion
    }

    [System.Serializable]
    public class KeyAction
    {
        public KeyTag Tag;
        public KeyCode Button;
        public bool HasDownAction;
        public bool HasUpAction;
        public bool HasPressingAction;
        public bool CanBeUsed = true;
        [HideInInspector] public UnityAction DownAction;
        [HideInInspector] public UnityAction PressingAction;
        [HideInInspector] public UnityAction UpAction;
    }

    public enum KeyTag
    {
        Left = 0, Right = 1, Up = 2, Down = 3, Ability1 = 4, Ability2 = 5, Ability3 = 6, Ability4 = 7, NormalShot = 8, SuperShot = 9, WeaponSwitch = 10
    }

    [System.Serializable]
    public struct KeySet
    {
        public KeyTag Tag;
        public KeyCode TheButton;
    }
}