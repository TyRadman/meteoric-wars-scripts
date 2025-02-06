using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    public abstract class ObjectDamageRenderer : MonoBehaviour
    {
        [System.Serializable]
        public class DamageRenderer
        {
            public SpriteRenderer PartRenderer;
            public Color OriginalColor;

            public DamageRenderer(SpriteRenderer _sprite, Color _col)
            {
                PartRenderer = _sprite;
                OriginalColor = _col;
            }
        }

        /// <summary>
        /// The parts that will be colored upon taking damage
        /// </summary>
        public List<DamageRenderer> DamageRenderers = new List<DamageRenderer>();
        protected bool Available = true;
        [SerializeField] protected ParticlesPooling DeathParticles;
        [SerializeField] protected ParticlesPooling ImpactParticle;

        public void AddDamageRenderer(List<ShipPart> _sprites)
        {
            for (int i = 0; i < _sprites.Count; i++)
            {
                DamageRenderer newOne = new DamageRenderer(_sprites[i].transform.GetChild(0).GetComponent<SpriteRenderer>(),
                    _sprites[i].transform.GetChild(0).GetComponent<SpriteRenderer>().color);
                DamageRenderers.Add(newOne);
            }
        }

        public void AddDamageRenderer(SpriteRenderer _sprite)
        {
            DamageRenderers.Add(new DamageRenderer(_sprite, _sprite.color));
        }

        public void TakeDamageColorChanging()
        {
            if (!Available)
            {
                return;
            }

            Available = false;
            DamageRenderers.ForEach(d => d.PartRenderer.color = GameManager.i.GeneralValues.DamageColor);

            if (ImpactParticle != null)
            {
                PoolingSystem.Instance.UseParticles(ImpactParticle, transform.position, transform.localScale.x);
            }

            Invoke(nameof(ResetColor), GameManager.i.GeneralValues.DamageColorDuration);
        }

        public void PlayDeathParticles()
        {
            if (DeathParticles != null)
            {
                PoolingSystem.Instance.UseParticles(DeathParticles, transform.position);
            }
        }

        private void ResetColor()
        {
            DamageRenderers.ForEach(d => d.PartRenderer.color = d.OriginalColor);
            Available = true;
        }
    }
}