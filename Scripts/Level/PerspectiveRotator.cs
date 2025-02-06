using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerspectiveRotator : MonoBehaviour
{
    private Transform m_Cam;
    [SerializeField] private float m_RotationDuration = 2f;
    [SerializeField] private float m_RotationAmount = 90f;
    [SerializeField] private float m_BetweenFlipsTime = 20f;

    private void Awake()
    {
        m_Cam = Camera.main.transform;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            RotateCamera();
        }
    }

    private void RotateCamera()
    {
        StartCoroutine(rotate());
    }

    private IEnumerator rotate()
    {
        float time = 0f;
        float startingRotation = m_Cam.eulerAngles.z;
        float endingRotation = startingRotation + m_RotationAmount;

        while(time < m_RotationDuration)
        {
            time += Time.deltaTime;
            float t = time / m_RotationDuration;
            m_Cam.eulerAngles = new Vector3(m_Cam.eulerAngles.x, m_Cam.eulerAngles.y, Mathf.Lerp(startingRotation, endingRotation, t));
            yield return null;
        }

        yield return new WaitForSeconds(m_BetweenFlipsTime);
        StartCoroutine(rotate());
    }
}
