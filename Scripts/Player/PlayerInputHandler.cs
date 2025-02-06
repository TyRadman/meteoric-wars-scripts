using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SpaceWar
{
    public class PlayerInputHandler : MonoBehaviour
    {
            public GamePlayInput Controls { set; get; }
            [field: SerializeField] public PlayerInput Playerinputs { set; get; }
            private static int ConfirmationNumber = 0;
            private static int MaxConfirmationNumber = 0;
            private bool _confirmed = false;
            [SerializeField] private PlayerShipsData _playersData;
            private InputActionMap _lobbyActionMap;
        }
}
