using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    public class PlayerCharacterMenuController : MonoBehaviour
    {
        [SerializeField] private List<ParticleSystem> _thrusters;

        public void PlayThrusters()
        {
            _thrusters.ForEach(t => t.Play());
        }
    }
}
