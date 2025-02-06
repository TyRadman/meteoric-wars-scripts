using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    public class EnemyComponents : ShipComponenets
    {
        public ShipRank Rank;
        public bool HasAbility = false;
        public EnemyShipMovement Movement;
        [field: SerializeField] public EnemyShooting ShootingMethod { get; private set; }
        [field: SerializeField] public EnemyStats EnemyStats { get; private set; }
        [field: SerializeField] public EnemyShipAbilities Abilities { get; set; }
        public float OffenceValue;
        public float DefenceValue;
        public float MobilityValue;
        public new EnemyHealth Health;

        public bool IsInWave = false;
        public bool SpawnCollectables = true;
        public PolygonCollider2D Collider;
        public string EnemyName;

        public override void SetUp(IController components)
        {
            base.SetUp(components);

            Collider = GetComponent<PolygonCollider2D>();
            IsInWave = true;
            EnemyName = gameObject.name;
            Effects.MuzzleEffectTag = ParticlePoolTag.StandardMuzzle;

            ShootingMethod.SetUp(this);
            EnemyStats.SetUp(this);

            if (Abilities != null)
            {
                Abilities.SetUp(this);
            }
        }

        public void SetValues(ShipRankValues _values, float _offensiveDifficulty, float _defensiveDifficulty)
        {
            Health.SetValues(_values, _defensiveDifficulty);

            EnemyStats.SetValues(_values, _offensiveDifficulty, _defensiveDifficulty);
        }

        public override void OnDeath()
        {
            base.OnDeath();

            EnemyStats.OnDeath();

            // play the explosion particles using the method overload that takes size into account to resize the explosion
            PoolingSystem.Instance.UseParticles(ParticlePoolTag.StandardExplosion, transform.position, Quaternion.identity, transform.localScale.x);

            // play ship's death particles
            SpawnAndKillManager.i.ShipDeath(Effects.GetSprites(), transform, true);

            ReportDeathToEnemiesManager();

            DisableDamageDetectors();

            // stops the enemy from shooting (a ghost keeps shooting if this line wasn't there, trust me)
            ShootingMethod.StopShooting();

            Movement.StopMovement();

            // if the ship has an ability, then stop it from looping (remember the mother ship that wouldn't stop spawning minions?
            if (HasAbility)
            {
                Abilities.StopSpecialAttackQueue();
            }

            // freeze the screen (an effect that can be moved somewhere else later)
            ScreenFreezer.i.FreezeScreen(GameManager.i.GeneralValues.RankValues.Find(v => v.Rank == Rank).FreezeStrength);

            SpawnCollectable();
        }

        private void ReportDeathToEnemiesManager()
        {
            if (IsInWave)
            {
                // so that the wave manager starts spawning if all enemies are down
                WavesManager.i.EnemyDestroyed(transform);
            }
            else
            {
                // if the ship is fighting on the player's side, then it's not a part of the wave therefore, we don't notify the waves manager
                if (ShipStats.UserTag == BulletUser.Enemy)
                {
                    WavesManager.i.EnemyDestroyed(transform);
                }

                GetComponent<SummonedShip>().OnDestroyed(ShipStats.UserTag, transform);
            }
        }

        public void SpawnCollectable()
        {
            if (SpawnCollectables)
            {
                CollectableSpawner.i.SpawnCollectable(transform.position, Rank, Collider.GetAverageArea());
            }
        }

        public void SetShootingMethod(EnemyShooting shootingMethod)
        {
            ShootingMethod = shootingMethod;
        }

        #region
        public override void Activate()
        {
            base.Activate();
            Abilities.Activate();
        }

        public override void Deactivate()
        {
            base.Deactivate();
            Abilities.Deactivate();
        }

        public override void Dispose()
        {
            base.Dispose();
            Abilities.Dispose();
        }
        #endregion
    }
}
