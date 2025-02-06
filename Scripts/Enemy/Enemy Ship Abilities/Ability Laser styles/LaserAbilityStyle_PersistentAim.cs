using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    [CreateAssetMenu(menuName = "Enemy Abilities/Laser/Persistent Aim")]
    public class LaserAbilityStyle_PersistentAim : EnemyLaserAbilityStyle
    {
        [SerializeField] private float m_WarningMovementSpeed = 5f;
        [SerializeField] private float m_ShootingMovementSpeed = 2f;
        private EnemyShipMovement m_Movement;

        public override void SetUp(ShipComponenets components)
        {

        }

        public override void SetUp(float _difficulty, RedLaserAbility _ability, Transform _ship)
        {
            base.SetUp(_difficulty, _ability, _ship);
            m_Movement = _ship.GetComponent<EnemyShipMovement>();
        }

        public override void Perform(LaserIndicator _indicator, RedLaserAbility _ability, EnemyShipAbilities _holder, Transform _ship)
        {
            base.Perform(_indicator, _ability, _holder, _ship);
            ActionCoroutine = GameManager.i.GeneralValues.StartCoroutine(PerformanceProcess(_indicator, _ability, _holder, _ship));
        }

        private IEnumerator PerformanceProcess(LaserIndicator _indicator, RedLaserAbility _ability, EnemyShipAbilities _holder, Transform _ship)
        {
            // start the indicator first
            Indicator.StartIndicator(WarningTime);
            float time = 0;

            while (time < WarningTime)
            {
                time += Time.deltaTime;
                m_Movement.MoveToDirection(GameManager.i.PlayersManager.GetClosestPlayerToTransform(_ability.transform.position), m_WarningMovementSpeed);
                Indicator.transform.position = _ship.transform.position;
                yield return null;
            }

            float duration = _ability.GetWeapon().ShotDuration + _ability.GetWeapon().ResizingTime * 2;
            _ability.Activate();
            time = 0;

            while (time < duration)
            {
                time += Time.deltaTime;
                m_Movement.MoveToDirection(GameManager.i.PlayersManager.GetClosestPlayerToTransform(_ability.transform.position), m_ShootingMovementSpeed);
                yield return null;
            }

            m_Movement.StopMovementToDirection();
            yield return new WaitForSeconds(1f);
            _holder.ResumeMovement();
            _holder.ResumeShooting();
            _holder.PerformAbilityAgain();
        }
    }
}