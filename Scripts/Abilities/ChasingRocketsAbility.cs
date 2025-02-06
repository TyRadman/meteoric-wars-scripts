using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    public class ChasingRocketsAbility : Ability
    {
        [Header("Special Variables")]
        [SerializeField] private BulletPoolingObject m_BulletPoolingRequest;
        [SerializeField] private ParticlesPooling m_MuzzlePoolingRequest;
        [SerializeField] private ParticlesPooling m_ImpactPoolingRequest;
        [SerializeField] private BulletWeapon m_Weapon;
        private PlayerShooting m_PlayerShooting;

        public override void SetUp(Transform _ship = null)
        {
            base.SetUp(_ship);

            m_PlayerShooting = _ship.GetComponent<PlayerShooting>();
            m_PlayerShooting.SetAbilitiesBulletWeapon(m_Weapon);
            PoolingSystem.Instance.CreateAdditionalBulletPools(m_BulletPoolingRequest);
            PoolingSystem.Instance.CreateAdditionalParticlePools(m_MuzzlePoolingRequest);
            PoolingSystem.Instance.CreateAdditionalParticlePools(m_ImpactPoolingRequest);
            Slot.SetAmount(CurrentAmount.ToString());
        }

        public override void Activate()
        {
            base.Activate();

            if (!IsAvailable)
            {
                return;
            }

            IsAvailable = false;
            AddAmount(-1f);

            if (CurrentAmount > 0)
            {
                Slot.StartAbilityUsageCountdown(LeastUsageTime);
                Invoke(nameof(RechargeAbility), LeastUsageTime);
            }

            for (int i = 0; i < 5; i++)
            {
                m_PlayerShooting.AbilityBulletShoot();
            }
        }
    }
}