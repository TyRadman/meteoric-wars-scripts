using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A base class for type shooter that are not bullets, rockets or laser. This has no standard types, it's different from ship to ship
/// </summary>
public class CustomShooter : MonoBehaviour
{
    public virtual void Shoot()
    {

    }

    public virtual void Upgrade(Weapon _newWeapon, int _level)
    {

    }
}
