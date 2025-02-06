using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SpaceWar
{
    public class PlayerAbilityUIElement : MonoBehaviour
    {
        public enum AbilityIconState
        {
            Idle, Usage, Charging
        }

        private AbilityIconState m_State = AbilityIconState.Idle;
        public Image IconImage;
        public bool HasAbility;
        private string m_ActionKey;
        [SerializeField] private int m_PlayerIndex;
        public int SlotIndex;
        public PlayerAbilities PlayerAbility;
        [SerializeField] private Text m_KeyText;
        [SerializeField] private Image m_CoverImage;
        [SerializeField] private Text m_CountDownText;
        [SerializeField] private Text m_LevelText;
        [SerializeField] private Text m_AmountText;
        [SerializeField] private Animation m_Anim;
        [SerializeField] private AnimationClip m_ActivationClip;
        [SerializeField] private AnimationClip m_DeactivationClip;
        [SerializeField] private Animation m_KeyDisplayAnimation;
        [SerializeField] private GameObject m_EmptyCover;

        private void Awake()
        {
            m_KeyDisplayAnimation.Play();
            m_CountDownText.text = string.Empty;
            m_KeyText.text = string.Empty;
            m_AmountText.text = string.Empty;
            IconImage.color = Color.grey;
            ActivateSlot(false, 0, Ability.AbilityUsage.None, Ability.AbilityLevelUpType.None);
        }

        public void SetUp()
        {
            PlayerAbility = GameManager.i.PlayersManager.Players[m_PlayerIndex].Components.Abilities;
        }

        public void ActivateSlot(bool _activate, int _level, Ability.AbilityUsage _usageType, Ability.AbilityLevelUpType _levelType, string _amount = "")
        {
            HasAbility = _activate;
            m_EmptyCover.SetActive(!_activate);

            if (!_activate)
            {
                IconImage.color = Color.grey;
                m_LevelText.text = string.Empty;
                m_KeyText.text = string.Empty;
            }
            else
            {
                IconImage.color = Color.white;

                if (_levelType == Ability.AbilityLevelUpType.Upgradable)
                {
                    m_LevelText.text = $"Lvl {(_level + 1):0}";
                    m_AmountText.text = string.Empty;
                }
                else if (_levelType == Ability.AbilityLevelUpType.Addaditve)
                {
                    m_LevelText.text = string.Empty;
                    m_AmountText.text = _amount;
                }

                // without this, the activation key is displayed when the ability of the icon is upgraded even if the ability was still charging which is not supposed to happen
                if (m_State == AbilityIconState.Idle)
                {
                    m_KeyText.text = m_ActionKey;
                }

                if (_usageType == Ability.AbilityUsage.Effect)
                {
                    m_KeyText.text = string.Empty;
                }
            }
        }

        public void ActivateAbility(float _coolDownTime)
        {
            m_Anim.clip = m_DeactivationClip;
            m_Anim.Play();
            StartCoroutine(countDownProcess(_coolDownTime));
        }

        public void StartAbilityUsageCountdown(float _time)
        {
            m_KeyText.text = string.Empty;
            m_Anim.clip = m_ActivationClip;
            m_Anim.Play();
            // we add one so that the counter ends at zero not after zero (players are not programmers)
            StartCoroutine(usageProcess(_time));
        }

        public void TurnOffSlot()
        {
            m_LevelText.text = string.Empty;
            m_KeyText.text = string.Empty;
            HasAbility = false;
            IconImage.sprite = null;
            IconImage.color = Color.grey;
        }

        public void ClearSlot()
        {
            TurnOffSlot();
            StopAllCoroutines();
            m_CoverImage.fillAmount = 0f;
            m_CountDownText.text = string.Empty;
            m_State = AbilityIconState.Idle;
        }

        public void SetAmount(string _amount)
        {
            m_AmountText.text = _amount;
        }

        private IEnumerator usageProcess(float _time)
        {
            float time = 0f;
            m_State = AbilityIconState.Usage;

            while (time < _time)
            {
                time += Time.deltaTime;
                float t = time / _time;
                m_CoverImage.fillAmount = t;
                m_CountDownText.text = $"{_time - time:0.0} s";
                yield return null;
            }
        }

        private IEnumerator countDownProcess(float _countDownTime)
        {
            float time = 0f;
            m_State = AbilityIconState.Charging;

            while (time < _countDownTime)
            {
                time += Time.deltaTime;
                float t = time / _countDownTime;
                m_CoverImage.fillAmount = 1 - t;
                m_CountDownText.text = $"{_countDownTime - time:0.0} s";
                yield return null;
            }

            m_State = AbilityIconState.Idle;
            m_CountDownText.text = string.Empty;
            m_KeyText.text = m_ActionKey;
        }

        public void SetKeyText(string _text)
        {
            m_ActionKey = _text;
        }
    }
}