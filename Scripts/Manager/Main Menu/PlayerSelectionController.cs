using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSelectionController : Singlton<PlayerSelectionController>
{
    [SerializeField] private List<PlayerInput> m_PlayersInput;

    protected override void Awake()
    {
        base.Awake();
    }

    public void SwitchShips(InputAction.CallbackContext context)
    {
        Vector2 values = context.ReadValue<Vector2>();
        
        if(values.x > 0)
        {

        }
        else if(values.x < 0)
        {

        }
    }
}
