using SpaceWar.Audios;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapons/Laser Weapon")]
public class LaserWeapon : Weapon
{
    public float DamagePerSecond;
    public float Width;
    public float ShotDuration;
    public float ResizingTime;
    [Tooltip("How fast will the laser flicker")]
    public float Frequency;
    [Tooltip("How much will the laser grow in size on top of its original width")]
    public float Amplitude;
    public Gradient Color;

    public void CopyWeaponValues(LaserWeapon _weapon)
    {
        _weapon.DamagePerSecond = DamagePerSecond;
        _weapon.Width = Width;
        _weapon.ShotDuration = ShotDuration;
        _weapon.ResizingTime = ResizingTime;
        _weapon.Frequency = Frequency;
        _weapon.Amplitude = Amplitude;
        _weapon.Color = Color;
        _weapon.Audio = Audio;
    }
}
