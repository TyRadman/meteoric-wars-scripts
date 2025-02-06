using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SpaceWar.Shop
{
    public class OffensiveShop : MonoBehaviour
    {
        [SerializeField] private ShopComponents m_Shop;
        [SerializeField] private EnemyShipMovement m_Movement;
        [SerializeField] private EnemyHealth m_Health;
        [SerializeField] private EnemyComponents m_Components;
        [Header("Movement up")]
        [SerializeField] private float m_MovementToTopDuration = 4f;
        [SerializeField] private float m_UpDistance = 7f;
        public bool CheckForHealth = true;

        private void Update()
        {
            CheckHealth();
        }

        private void CheckHealth()
        {
            if (!CheckForHealth) return;

            if (m_Health.GetHealthT() <= 0.8f)
            {
                CheckForHealth = false;
                StartCoroutine(MovementBackToTopProcess());
            }
        }

        private IEnumerator MovementBackToTopProcess()
        {
            // disable shop related functionality
            m_Shop.Shop.StopShopMode();

            // movement process
            float time = 0f;
            Vector2 startPosition = transform.position;
            Vector2 endPosition = (Vector2)transform.position + Vector2.up * m_UpDistance;
            AnimationCurve curve = GameManager.i.GeneralValues.EntranceMovementSpeedCurve;

            while (time < m_MovementToTopDuration)
            {
                time += Time.deltaTime;
                float t = time / m_MovementToTopDuration;
                transform.position = Vector2.Lerp(startPosition, endPosition, curve.Evaluate(t));

                yield return null;
            }

            // show weapons
            m_Shop.Animation.PlayWeaponAnimation(true);
            yield return new WaitForSeconds(m_Shop.Animation.GetWeaponsAnimationLength(true));

            // activate crazy shop
            m_Movement.PerformMovement(transform);

            // set the shooting
            if (m_Components.ShootingMethod.Components == null)
            {
                m_Components.ShootingMethod.Components = m_Components;
            }

            // start shooting
            m_Components.ShootingMethod.SetUp(m_Components);
            m_Components.ShootingMethod.PerformShooting();
            // start activating the abilities
            m_Components.Abilities.ActivateShip();
        }

        public void SetMaxHealth()
        {
            m_Health.SetCurrentHealthToMaxHealth();
        }

        public void StopAggressiveMode()
        {
            // stop shooting
            m_Components.ShootingMethod.StopShooting();
            // stop movement
            m_Components.Movement.StopMovement();
            // stop abilities
            m_Components.Abilities.StopSpecialAttackQueue();
            // animation
            m_Shop.Animation.PlayWeaponAnimation(false);
        }
    }
}