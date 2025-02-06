using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    public class CRTransformation : Ability
    {
        private PlayerShooting m_PlayerShooter;
        private Shooter m_OldShooter;
        private Shooter m_NewShooter;
        private ShipEffects m_Effects;
        private Vector2[] m_OldColliderPath;
        private Vector2[] m_NewColliderPath;
        private PolygonCollider2D m_Collider;
        [SerializeField] private List<CRTransformationLevel> m_Levels;
        [Header("Special References")]
        [SerializeField] private GameObject m_BigShipPrefab;
        private ParticleSystem[] m_NewThrusters;
        [Header("Animation Variables")]
        [SerializeField] private AnimationClip[] m_AnimationClips;
        private Animation m_SmallShipAnimation;
        private Animation m_BigShipAnimation;
        private PlayerStats m_PlayerStats;
        private float m_UsageDuration = 10f;
        private float m_Resilience = 0.3f;
        private PlayerMovement m_ShipMovement;
        private WaitForSeconds m_WaitingTime;

        [System.Serializable]
        public struct CRTransformationLevel
        {
            public float UsageDuration;
            public float Resilience;
            public float SpeedReduction;
        }

        public override void SetLevelValues(int _levelNumber)
        {
            base.SetLevelValues(_levelNumber);

            CRTransformationLevel level = m_Levels[_levelNumber];
            m_UsageDuration = level.UsageDuration;
            m_Resilience = level.Resilience;
        }

        public override void SetUp(Transform _ship = null)
        {
            base.SetUp(_ship);
            // cache the ship's original graphics object
            m_Effects = _ship.GetComponent<ShipEffects>();
            m_ShipMovement = _ship.GetComponent<PlayerMovement>();
            var bigShip = Instantiate(m_BigShipPrefab, _ship);
            m_WaitingTime = new WaitForSeconds(m_AnimationClips[0].length * 0.5f);
            m_BigShipAnimation = bigShip.transform.GetChild(2).GetComponent<Animation>();
            m_BigShipAnimation.GetComponent<SpriteRenderer>().enabled = false;
            m_SmallShipAnimation = m_Effects.DamageRenderers[0].PartRenderer.GetComponent<Animation>();
            m_NewThrusters = new ParticleSystem[bigShip.transform.GetChild(1).childCount];

            // add new thrusters
            for (int i = 0; i < m_NewThrusters.Length; i++)
            {
                m_NewThrusters[i] = bigShip.transform.GetChild(1).GetChild(i).GetComponent<ParticleSystem>();
                // add the thrusters to the original ship effects
                _ship.GetComponent<ShipEffects>().AddThrusterToExistingsSpeeds(m_NewThrusters[i], i + 2);
            }


            // cache the shooter and the shooting points
            m_PlayerShooter = _ship.GetComponent<PlayerShooting>();
            m_PlayerStats = _ship.GetComponent<PlayerStats>();
            m_OldShooter = _ship.GetComponent<Shooter>();
            m_NewShooter = bigShip.GetComponent<Shooter>();
            m_NewShooter.UserTag = m_OldShooter.UserTag;
            m_Collider = _ship.GetComponent<PolygonCollider2D>();
            m_OldColliderPath = m_Collider.GetPath(0);
            m_NewColliderPath = bigShip.GetComponent<PolygonCollider2D>().GetPath(0);
            m_NewShooter.UserIndex = m_OldShooter.UserIndex;
            m_NewShooter.Effects = m_OldShooter.Effects;
            m_NewShooter.SetWeaponShooting();
        }

        public override void Activate()
        {
            base.Activate();

            if (!IsAvailable)
            {
                return;
            }

            IsAvailable = false;

            // change the graphics
            StartCoroutine(transformShip());
        }

        public override void RechargeAbility()
        {
            base.RechargeAbility();
            StartCoroutine(wearOffTransformation());
        }

        private IEnumerator wearOffTransformation()
        {
            m_ShipMovement.Enable(false);
            m_PlayerShooter.CanShoot = false;
            m_SmallShipAnimation.clip = m_AnimationClips[1];
            m_BigShipAnimation.clip = m_AnimationClips[0];
            m_BigShipAnimation.Play();
            // wait until half the animation is done to start the other one
            yield return m_WaitingTime;
            m_SmallShipAnimation.Play();

            foreach (ParticleSystem thruster in m_NewThrusters)
            {
                thruster.Stop();
            }

            // wait for the entire animation to finish and then set values
            yield return m_WaitingTime;
            yield return m_WaitingTime;
            m_PlayerShooter.SetShooter(m_OldShooter);
            m_PlayerShooter.CanShoot = true;
            m_OldShooter.Effects.Thrusters.ForEach(t => t.Play());
            m_Collider.SetPath(0, m_OldColliderPath);
            m_PlayerStats.Resilience = 0f;
            m_ShipMovement.Enable(true);
        }

        private IEnumerator transformShip()
        {
            // disable shooting
            m_PlayerShooter.CanShoot = false;
            // disable movement
            m_ShipMovement.Enable(false);
            // set up animations
            m_SmallShipAnimation.clip = m_AnimationClips[0];
            m_BigShipAnimation.clip = m_AnimationClips[1];
            m_SmallShipAnimation.Play();
            // wait until half the animation is done to start the other one
            yield return m_WaitingTime;
            m_BigShipAnimation.Play();
            // wait for the entire animation to finish and then set values
            yield return m_WaitingTime;
            yield return m_WaitingTime;

            // enable shooting
            m_PlayerShooter.CanShoot = true;
            // change the shooter
            m_PlayerShooter.SetShooter(m_NewShooter);
            // set the player's resilience
            m_PlayerStats.Resilience = m_Resilience;

            // switch thrusters
            foreach (ParticleSystem thruster in m_NewThrusters)
            {
                thruster.Play();
            }

            m_OldShooter.Effects.Thrusters.ForEach(t => t.Stop());
            // set the big ship's collider
            m_Collider.SetPath(0, m_NewColliderPath);
            // show usage remaining time
            Slot.StartAbilityUsageCountdown(m_UsageDuration);
            // start counting down
            Invoke(nameof(RechargeAbility), m_UsageDuration);
            // enable movement
            m_ShipMovement.Enable(true);
        }
    }
}