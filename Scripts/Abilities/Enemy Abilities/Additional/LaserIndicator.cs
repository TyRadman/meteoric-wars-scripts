using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserIndicator : MonoBehaviour
{
    private Gradient m_Color;
    [SerializeField] private int m_NumberOfFlashesPerSecond;
    [SerializeField] private Vector2 m_AlphaLimits;
    [SerializeField] private LineRenderer m_Line;
    private Coroutine m_IndicationProcess;

    private void Awake()
    {
        m_Color = m_Line.colorGradient;
    }

    public void StartIndicator(float _time)
    {
        if (m_IndicationProcess != null) return;

        m_Line.enabled = true;
        m_IndicationProcess = StartCoroutine(IndicationProcess(_time));
    }

    private IEnumerator IndicationProcess(float _duration)
    {
        float time;
        int totalFlashes = Mathf.RoundToInt(_duration * m_NumberOfFlashesPerSecond);
        float halfAFlash = (_duration / totalFlashes) / 2;

        // in the case the duration is shorter than the smallest number it can be rounded up to which 1 resulted by 0.5, then we make it one flash
        if(_duration <= 0.5f)
        {
            totalFlashes = 1;
        }

        for (int i = 0; i < totalFlashes; i++)
        {
            time = 0f;

            while (time < halfAFlash)
            {
                time += Time.deltaTime;
                float alpha = Mathf.Lerp(m_AlphaLimits.x, m_AlphaLimits.y, time / halfAFlash);
                GradientAlphaKey key = new GradientAlphaKey(alpha, 0f);
                m_Color.SetKeys(m_Color.colorKeys, new GradientAlphaKey[] { key, key });
                m_Line.colorGradient = m_Color;
                yield return null;
            }

            time = 0f;

            while (time < halfAFlash)
            {
                time += Time.deltaTime;
                float alpha = Mathf.Lerp(m_AlphaLimits.y, m_AlphaLimits.x, time / halfAFlash);
                GradientAlphaKey key = new GradientAlphaKey(alpha, 0f);
                m_Color.SetKeys(m_Color.colorKeys, new GradientAlphaKey[] { key, key });
                m_Line.colorGradient = m_Color;
                yield return null;
            }
        }

        m_IndicationProcess = null;
    }

    public void StopIndicator()
    {
        m_Line.enabled = false;
        StopAllCoroutines();
    }
}
