using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy Values/Sniper Machine Gun Values")]
public class SniperMachineGunValuesObject : ScriptableObject
{
    public Vector2 Speed;
    public Vector2Int NumberOfShots;
    public Vector2Int BulletsPerShot;
    public Vector2 TimeBetweenShots;
    public Vector2 Angle;
    public Vector2 CoolDownTime;
}
