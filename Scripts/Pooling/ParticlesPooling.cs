using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    [CreateAssetMenu(menuName = "Others/Particle Pool")]
    public class ParticlesPooling : ScriptableObject
    {
        public ParticlePoolTag Tag;
        public GameObject Prefab;
        /// <summary>
        /// Determines whether the color overrides the original colors of the particle system
        /// </summary>
        public bool UseColor = true;
        public Color ParticleColor;
        public int InitialNumber;
        [HideInInspector] public List<ParticleSystem> Objects;
        [HideInInspector] public Transform Parent;
    }
}