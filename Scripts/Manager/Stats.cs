using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SpaceWar
{
    [RequireComponent(typeof(ShipEffects))]
    public class Stats : MonoBehaviour, IController
    {
        public bool IsInWave = false;
        [HideInInspector] public float DamagePerShot = 0f;
        public BulletUser UserTag;

        public virtual void SetUp(IController components)
        {

        }

        public virtual void OnDeath()
        {
            GameManager.i.AudioManager.PlayAudio(GameManager.i.GeneralValues.DeathAudio);
        }

        public virtual void SetDamage(float _damage)
        {
            DamagePerShot = _damage;
        }

        public BulletUser GetUserTag()
        {
            return UserTag;
        }

        #region IController
        public virtual void Activate()
        {

        }

        public virtual void Deactivate()
        {

        }

        public virtual void Dispose()
        {

        }
        #endregion
    }
}