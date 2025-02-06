using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenFreezer : Singlton<ScreenFreezer>
{
    public enum FreezeStrength
    {
        Weak, Mild, Meduim, Strong, Epic, NONE
    }

    [System.Serializable]
    public struct FreezingStrength
    {
        public FreezeStrength Strength;
        public float Duration;
    }

    [SerializeField] private List<FreezingStrength> m_FreezingDurationValues;
    [SerializeField] private float m_FreezingScale = 0.1f;

    public void FreezeScreen(FreezeStrength _strength)
    {
        if (_strength == FreezeStrength.NONE) return;

        Time.timeScale = m_FreezingScale;
        StartCoroutine(ReleaseScreen(m_FreezingDurationValues.Find(v => v.Strength == _strength).Duration));
    }

    private IEnumerator ReleaseScreen(float _duration)
    {
        //yield return new WaitForSecondsRealtime(_duration);
        float time = 0f;
        float duraiton = _duration / 2;

        while(time < duraiton)
        {
            time += Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Lerp(1f, m_FreezingScale, time / duraiton);
            yield return null;
        }
        
        time = 0f;
        
        while (time < duraiton)
        {
            time += Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Lerp(m_FreezingScale, 1f, time / duraiton);
            yield return null;
        }

        Time.timeScale = 1f;
    }
}
