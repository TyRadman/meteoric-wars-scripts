using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar.Audios
{
    [CreateAssetMenu(fileName = "Audio File", menuName = "Others/Audio File")]
    public class Audio : ScriptableObject
    {
        public List<AudioClip> Clips;
        public bool Loop;
        [Range(0f, 1f)] public float Volume;
        [Header("Pitch")]
        public PitchMode PitchMode;
        [HideInInspector] public float Pitch = 1;
        [HideInInspector] public Vector2 PitchRandomizingRange;
    }

    public enum PitchMode
    {
        None, Random, Fixed
    }
}