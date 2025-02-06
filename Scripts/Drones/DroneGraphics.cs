using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneGraphics : MonoBehaviour
{
    [SerializeField] private GameObject m_Graphics;
    // because Invoke takes no arguments, so we cache the enable bool
    private bool m_Enable;

    public void EnableGraphics(bool _enable, float _delay = 0)
    {
        m_Enable = _enable;

        if (_delay > 0)
        {
            Invoke(nameof(EnableGraphicsProcess), _delay);
        }
        else
        {
            EnableGraphicsProcess();
        }
    }

    private void EnableGraphicsProcess()
    {
        m_Graphics.SetActive(m_Enable);
    }
}
