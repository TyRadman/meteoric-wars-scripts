using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar.Audios
{
    public class AudioSourcePooler : MonoBehaviour, IPooler
    {
        [SerializeField] private AudioSource m_SourcePrefab;
        [SerializeField] private int m_StartCount = 2;
        private List<AudioSource> m_Sources = new List<AudioSource>();

        public void CreatePools()
        {
            for (int i = 0; i < m_StartCount; i++)
            {
                AddItemToPool();
            }
        }

        public AudioSource GetAudioSource()
        {
            List<AudioSource> availableSources = m_Sources.FindAll(s => !s.isPlaying);

            if(availableSources.Count == 0)
            {
                AddItemToPool();
                availableSources = m_Sources.FindAll(s => !s.isPlaying);
            }

            return availableSources[0];
        }

        public void AddItemToPool()
        {
            AudioSource source = Instantiate(m_SourcePrefab, transform);
            m_Sources.Add(source);
        }
    }
}
