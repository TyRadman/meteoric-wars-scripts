using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BuildTest : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_FPSText;

    private void Start()
    {
        DisplayFPS();
    }

    private void DisplayFPS()
    {
        m_FPSText.text = $"FPS: {(1f / Time.deltaTime).ToString("00")}";
        Invoke(nameof(DisplayFPS), 0.2f);
    }
}
