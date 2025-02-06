using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar.Audios
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private AudioSourcePooler m_Pooler;

        public void SetUp()
        {
            m_Pooler.CreatePools();
        }

        public void PlayAudio(Audio _audio)
        {
            if (_audio == null)
            {
                print("No audio passed");
                return;
            }

            AudioSource source = m_Pooler.GetAudioSource();

            source.pitch = _audio.Pitch;
            source.loop = _audio.Loop;

            if (_audio.PitchMode == PitchMode.Random)
            {
                source.pitch = 1 + _audio.PitchRandomizingRange.RandomValue();
            }

            source.PlayOneShot(_audio.Clips[0], _audio.Volume);
        }

        public AudioSource PlayAudioAndGetSource(Audio _audio)
        {
            if (_audio == null)
            {
                print("No audio passed");
                return null;
            }

            AudioSource source = m_Pooler.GetAudioSource();

            source.pitch = _audio.Pitch;
            source.loop = _audio.Loop;

            if (_audio.PitchMode == PitchMode.Random)
            {
                source.pitch = 1 + _audio.PitchRandomizingRange.RandomValue();
            }

            if (source.loop)
            {
                source.volume = _audio.Volume;
                source.clip = _audio.Clips[0];
                source.Play();
            }
            else
            {
                source.PlayOneShot(_audio.Clips[0], _audio.Volume);
            }

            return source;
        }
    }
}