using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SpaceWar
{
    public class EnemyStats : Stats
    {
        public ShipRank Rank;
        public EnemyComponents Components;
        public float ScoreValue;
        public float Difficulty = 1f;
        [field: SerializeField]
        // TODO: must be removed
        public virtual UnityAction OnDeathAction { set; get; }

        public override void OnDeath()
        {
            base.OnDeath();

            // invoke any events subscribed to the enemy's death
            OnDeathAction?.Invoke();


            // TODO: scores must be handled differently
            // add score to the player
            //AddScoreToPlayer(_playerIndex);
        }

        public override void SetDamage(float _damage)
        {
            base.SetDamage(_damage);
            Components.ShipShooter.Damage = _damage;
        }

        private void AddScoreToPlayer(int _playerIndex)
        {
            if (_playerIndex >= 0)
            {
                GameManager.i.PlayersManager.Players[_playerIndex].Components.Stats.AddScore(ScoreValue);
            }
        }

        internal void SetValues(ShipRankValues _values, float _offensiveDifficulty, float _defensiveDifficulty)
        {
            Rank = _values.Rank;
            ScoreValue = _values.Score;

            SetUserTag(BulletUser.Enemy);

            SetDamage(_values.DamageRange.Lerp(_offensiveDifficulty));
        }

        public void SetUserTag(BulletUser _tag)
        {
            UserTag = _tag;
            Components.ShipShooter.UserTag = _tag;
            Components.ShipShooter.SetWeaponShooting();
            transform.tag = _tag == BulletUser.Player ? GeneralValues.PlayerTag : GeneralValues.EnemyTag;
            gameObject.layer = _tag == BulletUser.Player ? GeneralValues.PlayerLayerInt : GeneralValues.EnemyLayerInt;
        }
    }
}