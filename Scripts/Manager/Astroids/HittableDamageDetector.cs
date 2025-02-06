using UnityEngine;

namespace SpaceWar
{
    public class HittableDamageDetector : DamageDetector
    {
        [SerializeField] private AstroidDamageRenderer m_DamageRenderer;

        public override void GotShot(float _damage, Vector3 _impactPoint, int _playerIndex = -1)
        {
            base.GotShot(_damage, _impactPoint, _playerIndex);
            m_DamageRenderer.TakeDamageColorChanging();
        }
    }
}