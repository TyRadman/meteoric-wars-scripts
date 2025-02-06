using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

namespace SpaceWar.Shop
{
    public class Shop : MonoBehaviour, IInteractable
    {
        [SerializeField] private Vector2 m_ColliderSizeRange;
        [SerializeField] private float m_MovementDuration = 4f;
        [SerializeField] private float m_WaitingDuration = 10f;
        [SerializeField] private AnimationCurve m_MovementAwayAnimation;
        [SerializeField] private AnimationCurve m_MovementAnimation;
        [SerializeField] private Gradient m_TimerColor;
        [Header("References")]
        [SerializeField] private CircleCollider2D m_Collider;
        [SerializeField] private Transform m_Timer;
        [SerializeField] private SpriteRenderer m_TimerSprite;
        [SerializeField] private ShopComponents m_Shop;
        [SerializeField] private EnemyComponents m_Components;
        [SerializeField] private TextMeshPro m_InteractionText;
        public bool IsPresent = false;
        private bool m_CanInteract = false;
        private int m_CurrentPlayerIndex = -1;
        public bool UsingShop = false;

        #region Shop Spawning
        public void SpawnShop()
        {
            // don't spawn again if already spawning
            if (IsPresent)
            {
                return;
            }

            m_Components.SetUp(null);

            m_InteractionText.enabled = false;
            IsPresent = true;
            m_Shop.OffensiveShop.CheckForHealth = true;
            m_Collider.enabled = false;
            float xPosition = Random.Range(-LevelDimensions.Instance.HalfWidth + 2, LevelDimensions.Instance.HalfWidth - 2);
            float yPosition = LevelDimensions.Instance.LevelHeight + 5f;
            transform.position = new Vector2(xPosition, yPosition);
            // set the shop's health to max
            m_Shop.OffensiveShop.SetMaxHealth();

            StartCoroutine(MovementProcess());

        }

        private IEnumerator MovementProcess()
        {
            float time = 0f;
            Vector2 startPosition = transform.position;
            Vector2 endPosition = startPosition;
            endPosition.y = -2f;

            while (time < m_MovementDuration)
            {
                time += Time.deltaTime;
                float t = time / m_MovementDuration;
                float tt = m_MovementAnimation.Evaluate(t);
                transform.position = Vector2.Lerp(startPosition, endPosition, tt);

                yield return null;
            }

            StartCoroutine(TimerProcess());
        }

        private IEnumerator TimerProcess()
        {
            // start a dialogue
            m_Shop.Dialogue.TypeMessage();
            // play the screen animation
            m_Shop.Animation.PlayAnimation(ShopAnimation.Animations.OpenShop);
            yield return new WaitForSeconds(m_Shop.Animation.GetAnimationLength(0));
            m_Collider.enabled = true;
            float time = 0f;

            while (time < m_WaitingDuration)
            {
                if (!UsingShop)
                {
                    time += Time.deltaTime;
                    float t = time / m_WaitingDuration;
                    Vector2 scale = m_Timer.localScale;
                    scale.x = Mathf.Lerp(1f, 0f, t);
                    m_Timer.localScale = scale;
                    m_TimerSprite.color = m_TimerColor.Evaluate(t);
                }

                yield return null;
            }

            MoveShopAway();
        }

        public void MoveShopAway()
        {
            // disable checking for health so that the shop doesn't get offensive when it's about to leave
            m_Shop.OffensiveShop.CheckForHealth = false;
            StartCoroutine(MovementAwayProcess());
        }

        private IEnumerator MovementAwayProcess()
        {
            if (m_CanInteract)
            {
                m_Shop.Animation.PlayAnimation(ShopAnimation.Animations.HideInteraction);
            }

            m_Collider.enabled = false;
            m_Shop.Animation.PlayAnimation(ShopAnimation.Animations.CloseShop);

            float time = 0f;
            Vector2 startPosition = transform.position;
            Vector2 endPosition = startPosition;
            endPosition.y = -LevelDimensions.Instance.LevelHeight - 5f;

            while (time < m_MovementDuration)
            {
                time += Time.deltaTime;
                float t = time / m_MovementDuration;
                float tt = m_MovementAwayAnimation.Evaluate(t);
                transform.position = Vector2.Lerp(startPosition, endPosition, tt);

                yield return null;
            }

            IsPresent = false;
            gameObject.SetActive(false);
        }
        #endregion

        #region Interaction
        public void CancelInteraction()
        {
            StopAllCoroutines();
            MoveShopAway();
        }

        public void Interact(int playerIndex)
        {
            if (!m_CanInteract)
            {
                return;
            }

            UsingShop = true;
            GameManager.i.AbilitiesShop.Open(m_CurrentPlayerIndex);
        }

        public void StopInteraction()
        {
            UsingShop = false;
        }

        public void OnPlayerEnter(int _playerIndex)
        {
            m_CurrentPlayerIndex = _playerIndex;

            GameManager.i.PlayersManager.Players[m_CurrentPlayerIndex].Components.
                Interactions.OnInteractableAreaEntered(this);

            m_Collider.radius = m_ColliderSizeRange.y;
            m_Shop.Animation.PlayAnimation(ShopAnimation.Animations.ShowInteraction);
            m_CanInteract = true;
            UpdateInteractionText();
        }

        private void UpdateInteractionText()
        {
            m_InteractionText.enabled = true;
            GamePlayInput c = InputController.Controls;
            InputActionMap UIMap = InputController.GetMap(m_CurrentPlayerIndex, ActionMap.GamePlay);
            string interactText = UIMap.FindAction(c.Gameplay.Interact.name).GetBindingDisplayString(m_CurrentPlayerIndex);
            string skipText = UIMap.FindAction(c.Gameplay.Return.name).GetBindingDisplayString(m_CurrentPlayerIndex);
            m_InteractionText.text = $"Interact: [{interactText}]\nSkip: [{skipText}]";
        }

        public void OnPlayerExit(int _playerIndex)
        {
            m_CurrentPlayerIndex = -1;
            GameManager.i.PlayersManager.Players[_playerIndex].Components.
                Interactions.OnInteractableAreaExited();

            m_Collider.radius = m_ColliderSizeRange.x;
            m_Shop.Animation.PlayAnimation(ShopAnimation.Animations.HideInteraction);
            m_CanInteract = false;
            m_InteractionText.enabled = false;
        }
        #endregion

        #region Stop Shop Functionality
        public void StopShopMode()
        {
            // stop the timer
            StopAllCoroutines();

            // play the turn off ship animation
            if (m_CanInteract)
            {
                m_Shop.Animation.PlayAnimation(ShopAnimation.Animations.HideInteraction);
            }

            m_Collider.enabled = false;
            m_Shop.Animation.PlayAnimation(ShopAnimation.Animations.CloseShop);
        }
        #endregion
    }
}
