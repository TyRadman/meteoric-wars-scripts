using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] private Image m_HealthBar;
    [SerializeField] private Text m_NameText;
    [SerializeField] private Text m_HealthInNumbers;
    [SerializeField] private Gradient m_HealthBarGradient;
    [SerializeField] private float m_DisplayDuration = 3f;
    private bool m_BarIsDisplayed = false;
    [SerializeField] private Animation m_Anim;
    [SerializeField] private AnimationClip m_ShowClip;
    [SerializeField] private AnimationClip m_HideClip;
    private float m_Timer = 0f;

    private void Awake()
    {
        HideHealthBar();
    }

    private void Update()
    {
        if (m_BarIsDisplayed)
        {
            m_Timer += Time.deltaTime;

            if (m_Timer >= m_DisplayDuration)
            {
                HideHealthBar();
            }
        }
    }

    public void HideHealthBar()
    {
        m_BarIsDisplayed = false;
        m_Anim.Stop();
        m_Anim.clip = m_HideClip;
        m_Anim.Play();
        m_Timer = 0f;
    }

    public void UpdateHealthBar(float _currentHealth, float _maxHealth, string _enemyName)
    {
        m_HealthInNumbers.text = $"{_currentHealth:0} / {_maxHealth:0}";
        float t = _currentHealth / _maxHealth;
        m_HealthBar.fillAmount = t;
        m_HealthBar.color = m_HealthBarGradient.Evaluate(t);
        m_NameText.text = _enemyName;

        // if the bar is hidden 
        if (!m_BarIsDisplayed)
        {
            m_BarIsDisplayed = true;
            m_Anim.clip = m_ShowClip;
            m_Anim.Play();
        }
        // else
        else
        {
            // restart the timer
            m_Timer = 0f;
        }
    }
}
