using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    public class PlayerShipWeaponInfo : MonoBehaviour
    {
        private Shooter m_Shooter;
        private LaserShooter m_LaserShooter;
        public List<Weapon> Weapons;
        [SerializeField] private BulletPoolingObject m_BulletPoolingRequest;
        [SerializeField] private List<BulletPoolingObject> m_BulletPoolingRequests;
        [SerializeField] private ParticlesPooling m_MuzzlePoolingRequest;
        [SerializeField] private ParticlesPooling m_ImpactPoolingRequest;
        [SerializeField] private List<ParticlesPooling> m_ParticlePoolingRequests;

        private void Awake()
        {
            m_Shooter = GetComponent<Shooter>();
            m_LaserShooter = GetComponent<LaserShooter>();

            // create bullet objects
            // set the tag of the bullets' impact tag as the tag used for the impacts requested by the player ship
            m_BulletPoolingRequest.ImpactPoolTag = m_ImpactPoolingRequest.Tag;
            PoolingSystem.Instance.CreateAdditionalBulletPools(m_BulletPoolingRequest);
            PoolingSystem.Instance.CreateAdditionalParticlePools(m_MuzzlePoolingRequest);
            PoolingSystem.Instance.CreateAdditionalParticlePools(m_ImpactPoolingRequest);

            GetComponent<ShipEffects>().MuzzleEffectTag = m_MuzzlePoolingRequest.Tag;

            for (int i = 0; i < m_BulletPoolingRequests.Count; i++)
            {
                m_BulletPoolingRequests[i].ImpactPoolTag = m_BulletPoolingRequests[i].ImpactPoolTag;
                PoolingSystem.Instance.CreateAdditionalBulletPools(m_BulletPoolingRequests[i]);
            }

            for (int i = 0; i < m_ParticlePoolingRequests.Count; i++)
            {
                PoolingSystem.Instance.CreateAdditionalParticlePools(m_ParticlePoolingRequests[i]);
            }
        }

        private void Start()
        {
            SwitchWeapon();
        }


        public void SwitchWeapon()
        {
            if (Weapons.Count == 0)
            {
                return;
            }

            if (Weapons[0].TheWeaponTag == WeaponTag.Bullets)
            {
                m_Shooter.SetUpWeapon(Weapons[0] as BulletWeapon);
            }
            else if (Weapons[0].TheWeaponTag == WeaponTag.Rockets)
            {
                m_Shooter.SetUpWeapon(Weapons[0] as BulletWeapon);
            }
            else if (Weapons[0].TheWeaponTag == WeaponTag.Laser)
            {
                m_LaserShooter.SetWeapon(Weapons[0] as LaserWeapon);
            }
        }
    }
}