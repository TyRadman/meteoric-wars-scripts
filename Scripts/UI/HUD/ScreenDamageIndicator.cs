using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    public class ScreenDamageIndicator : MonoBehaviour
    {
        [SerializeField] private Animation m_Animation;

        public void PlayDamageAnimation()
        {
            m_Animation.Play();
        }
    }
}
