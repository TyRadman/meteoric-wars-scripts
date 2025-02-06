using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SpaceWar
{
    public class InputController : MonoBehaviour
    {
        public class ActionMapReference
        {
            public ActionMap MapTag;
            public int PlayerIndex;
            public InputActionMap Map;
        }

        public static GamePlayInput Controls;
        public InputControlsCreator ControlsCreator;
        private static List<ActionMapReference> m_Maps = new List<ActionMapReference>();
        private List<PlayerInput> m_PlayerInputs = new List<PlayerInput>();

        private void Awake()
        {
            Controls = new GamePlayInput();
        }

        public static InputActionMap GetMap(int _playerIndex, ActionMap map)
        {
            if (m_Maps.Count == 0)
            {
                Debug.LogError($"No {map} in maps");
                return null;
            }

            InputActionMap selectedMap = m_Maps.Find(m => m.PlayerIndex == _playerIndex && m.MapTag == map).Map;

            if(selectedMap == null)
            {
                Debug.Log("No selected map in the list");
            }

            return selectedMap;
        }

        public void SetUp()
        {
            // check if players have already joined through a lobby. If not, make them manually join
            if (!GameManager.i.PlayerShipsData.HasPlayer())
            {
                // create the players
                ControlsCreator.SetUp();
            }

            Controls = new GamePlayInput();
            List<PlayerInputHandler> handlers = GameManager.i.PlayerShipsData.PlayerInputHandlers;
            FindObjectOfType<PlayerInputManager>().DisableJoining();
            m_Maps.Clear();

            for (int i = 0; i < handlers.Count; i++)
            {
                m_PlayerInputs.Add(handlers[i].Playerinputs);

                // add the maps to the static list
                for (int j = 0; j < handlers[i].Playerinputs.actions.actionMaps.Count; j++)
                {
                    ActionMapReference map = new ActionMapReference()
                    {
                        MapTag = (ActionMap)j,
                        PlayerIndex = i,
                        Map = handlers[i].Playerinputs.actions.actionMaps[j]
                    };

                    m_Maps.Add(map);
                }

                EnableGameplayInput(true, i);
            }
        }

        public void EnableGameplayInput(bool enable, int playerIndex = -1)
        {
            if (enable)
            {
                EnableInput(ActionMap.GamePlay, playerIndex);
            }

        }

        public void EnableInput(ActionMap actionMap, int playerIndex = -1)
        {
            if (playerIndex == -1)
            {
                m_PlayerInputs.ForEach(i => i.SwitchCurrentActionMap(actionMap.ToString()));
            }
            else
            {
                m_PlayerInputs[playerIndex].SwitchCurrentActionMap(actionMap.ToString());
            }
        }
    }

    public enum ActionMap
    {
        GamePlay = 0, UI = 1, Empty = 2
    }
}