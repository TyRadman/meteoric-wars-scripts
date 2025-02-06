using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SpaceWar
{
    public class RocketBulletEvent : RocketBullet
    {
        private UnityAction<Vector2> m_OnDeathAction;

        public override void OnTargetHit()
        {
            base.OnTargetHit();
            // if there is an event, then invoke it
            m_OnDeathAction?.Invoke(transform.position);
        }

        public void SetAction(UnityAction<Vector2> _action)
        {
            m_OnDeathAction = _action;
        }
    }
}