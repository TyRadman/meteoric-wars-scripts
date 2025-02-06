using System.Collections;
using SpaceWar.Audios;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : ScriptableObject
{
    public WeaponTag TheWeaponTag;
    public float CoolDownTime;
    public Audio Audio;
}

public enum WeaponTag
{
    Bullets, Rockets, Laser, Custom
}

public enum ShootingMode
{
    PerAllSP = 0, PerSpecificSP = 1, ThroughAllSPPerShot = 2, ThroughSpecificSPPerShot = 3, ThroughAllSPPerSet = 4, ThroughtSpecificSPPerSet = 5
}