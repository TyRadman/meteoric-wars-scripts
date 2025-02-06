using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScreenTitlesManager : MonoBehaviour
{
    public static ScreenTitlesManager Instance;
    [Header("Wave Variables")]
    [SerializeField] private TextMeshProUGUI m_WavesText;
    [SerializeField] private TextMeshProUGUI m_AdviceText;
    [SerializeField] private Image m_WaveLaserImage;
    [SerializeField] private Color m_WaveTextColor;
    [SerializeField] private Color m_VictoryColor;
    [TextArea(2, 5)][SerializeField] private string[] m_WaveAdvice;
    [SerializeField] private Animation m_WaveTextAnimation;
    [SerializeField] private Animation m_ScreenTextAnimation;
    [SerializeField] private List<AnimationInfo> m_AnimationInfo;
    [SerializeField] private CanvasGroup m_Stats;

    public enum AnimationTag
    {
        GameOver, Wave
    }

    [System.Serializable]
    public struct AnimationInfo
    {
        public AnimationTag Tag;
        public AnimationClip Clip;
    }

    private void Awake()
    {
        Instance = this;
    }

    public void ShowNextWave(int _waveNumber)
    {
        // set color
        m_WavesText.color = m_WaveTextColor;
        m_AdviceText.color = m_WaveTextColor;
        m_WaveLaserImage.color = m_WaveTextColor;
        // set text
        m_WavesText.text = $"Wave {_waveNumber + 1}";
        m_AdviceText.text = m_WaveAdvice[Random.Range(0, m_WaveAdvice.Length)];
        // play animation
        PlayAnimation(AnimationTag.Wave);
    }

    public void ShowLevelComplete()
    {
        m_WavesText.color = m_VictoryColor;
        m_WaveLaserImage.color = m_VictoryColor;
        m_WavesText.text = $"Victory";
        m_AdviceText.text = string.Empty;
        // for now
        PlayAnimation(AnimationTag.Wave);
    }

    public void PlayAnimation(AnimationTag _tag)
    {
        m_ScreenTextAnimation.clip = m_AnimationInfo.Find(a => a.Tag == _tag).Clip;
        m_ScreenTextAnimation.Play();
    }

    public float GetAnimationLength(AnimationTag _tag)
    {
        return m_AnimationInfo.Find(a => a.Tag == _tag).Clip.length;
    }

    public void ShowStats(bool _show)
    {
        StartCoroutine(ShowStatsProcess(_show));
    }

    private IEnumerator ShowStatsProcess(bool _show)
    {
        float time = 0f;

        while(time < 0.5f)
        {
            time += Time.deltaTime;
            float t = time / 0.5f;
            m_Stats.alpha = _show? Mathf.Lerp(0f, 1f, t) : Mathf.Lerp(1f, 0f, t);
            yield return null;
        }
    }
}
