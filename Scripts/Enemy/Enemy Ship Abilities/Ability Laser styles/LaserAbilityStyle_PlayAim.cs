using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    [CreateAssetMenu(menuName = "Enemy Abilities/Laser/Play Aim")]
    public class LaserAbilityStyle_PlayAim : EnemyLaserAbilityStyle
    {
        [SerializeField] private float m_MovementSpeed = 5f;

        public override void Perform(LaserIndicator _indicator, RedLaserAbility _ability, EnemyShipAbilities _holder, Transform _ship)
        {
            base.Perform(_indicator, _ability, _holder, _ship);
            ActionCoroutine = GameManager.i.GeneralValues.StartCoroutine(performanceProcess(_indicator, _ability, _holder, _ship));
        }

        private IEnumerator performanceProcess(LaserIndicator _indicator, RedLaserAbility _ability, EnemyShipAbilities _holder, Transform _ship)
        {
            // locate the player
            float xPosition = GameManager.i.PlayersManager.GetClosestPlayerToTransform(_ability.transform.position).x;
            // move to the player
            Vector2 newPosition = new Vector2(xPosition, _ship.position.y);
            Vector2 oldPosition = _ship.position;
            float time = 0f;
            float speed = Helper.GetTrueSpeed(m_MovementSpeed, Vector2.Distance(newPosition, oldPosition));

            while (time < speed)
            {
                time += Time.deltaTime;
                float t = time / speed;
                _ship.position = Vector2.Lerp(oldPosition, newPosition, t);
                yield return null;
            }

            // repeat the same normal 
            Indicator.transform.localPosition = Vector2.zero;
            Indicator.StartIndicator(WarningTime);
            yield return new WaitForSeconds(WarningTime);
            _ability.Activate();
            yield return new WaitForSeconds(_ability.GetWeapon().ShotDuration + 1f);
            _holder.ResumeMovement();
            _holder.ResumeShooting();
            _holder.PerformAbilityAgain();
        }
    }
}