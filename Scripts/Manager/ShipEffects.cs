using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    public class ShipEffects : ObjectDamageRenderer
    {
        private const float FLICKER_DURATION = 8f;

        public enum ThrustSpeedTypes
        {
            Fast, Normal, Slow
        }

        [System.Serializable]
        public struct ThrustSpeeds
        {
            public ThrustSpeedTypes Type;
            public float RateOverDistance;
            public float RateOverTime;
            public int BurstCount;
        }

        [System.Serializable]
        public class ThrustSpeedsPerThruster
        {
            public ParticleSystem Thruster;
            public List<ThrustSpeeds> ThrusterSpeeds;
        }

        private ThrustSpeedTypes m_CurrentThrusterSpeed;
        [SerializeField] private List<ThrustSpeedsPerThruster> m_ThrustersSpeedsInfo;
        public ParticlePoolTag MuzzleEffectTag = ParticlePoolTag.StandardMuzzle;
        public List<ParticleSystem> Thrusters;

        public void ShootingMuzzle(Vector3 _position, Quaternion _rotation)
        {
            PoolingSystem.Instance.UseParticles(MuzzleEffectTag, _position, _rotation);
        }

        public void ShootingMuzzle(Vector3 _position, Quaternion _rotation, ParticlePoolTag _tag)
        {
            PoolingSystem.Instance.UseParticles(_tag, _position, _rotation);
        }

        public List<SpriteRenderer> GetSprites()
        {
            List<SpriteRenderer> sprites = new List<SpriteRenderer>();
            DamageRenderers.ForEach(d => sprites.Add(d.PartRenderer));
            return sprites;
        }

        public void SetThrusterSpeed(ThrustSpeedTypes _type)
        {
            // new method
            if (m_CurrentThrusterSpeed == _type) return;

            m_CurrentThrusterSpeed = _type;

            for (int i = 0; i < m_ThrustersSpeedsInfo.Count; i++)
            {
                var thruster = m_ThrustersSpeedsInfo[i];

                // if there is a particles system for this thruster
                if (thruster.Thruster != null)
                {
                    var speed = thruster.ThrusterSpeeds.Find(s => s.Type == _type);
                    ParticleSystem.EmissionModule emission = thruster.Thruster.emission;
                    emission.rateOverDistance = speed.RateOverDistance;
                    emission.rateOverTime = speed.RateOverTime;
                    emission.burstCount = speed.BurstCount;
                }
            }
        }

        public void AddThrusterToExistingsSpeeds(ParticleSystem _particle, int _index)
        {
            m_ThrustersSpeedsInfo[_index].Thruster = _particle;
        }

        public void SetAvailability(bool _available)
        {
            Available = _available;
        }

        public void ShipSpriteFlicker(float _duration)
        {
            StartCoroutine(FlickeringProcess(_duration));
        }

        private IEnumerator FlickeringProcess(float _duration)
        {
            float time = 0f;

            while (time < _duration)
            {
                time += Time.deltaTime;

                foreach (DamageRenderer renderer in DamageRenderers)
                {
                    Color col = renderer.PartRenderer.color;
                    col.a = Mathf.Abs(Mathf.Sin(time * FLICKER_DURATION));
                    renderer.PartRenderer.color = col;
                }

                yield return null;
            }

            // return the alpha value of all sprites back to one
            foreach (DamageRenderer renderer in DamageRenderers)
            {
                Color col = renderer.PartRenderer.color;
                col.a = 1f;
                renderer.PartRenderer.color = col;
            }
        }
    }
}