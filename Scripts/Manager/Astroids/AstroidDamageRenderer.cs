using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    public class AstroidDamageRenderer : ObjectDamageRenderer
    {
        public void SetDeathEffect(ParticlesPooling _deathParticle)
        {
            DeathParticles = _deathParticle;
        }
    }
}