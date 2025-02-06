using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Singlton<T> : MonoBehaviour where T : Singlton<T>
{
    public static T i;

    protected virtual void Awake()
    {
        if(i != null)
        {
            print($"Destroyed {gameObject.name} because the script already exists at {i.gameObject.name}");
            Destroy(gameObject);
            return;
        }

        i = this as T;
    }
}
