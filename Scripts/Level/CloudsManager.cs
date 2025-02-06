using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudsManager : MonoBehaviour
{
    [System.Serializable]
    public class StarsParticles
    {
        public ParticleSystem Particle;
        [HideInInspector] public float MaxSpeed;
        [HideInInspector] public float MaxEmission;
        [HideInInspector] public float SpeedMinValueT;
        [HideInInspector] public float EmissionMinValueT;

        public void SetValues()
        {
            MaxSpeed = Particle.main.startSpeed.constantMax;
            MaxEmission = Particle.emission.rateOverTime.constantMax;
            SpeedMinValueT = Particle.main.startSpeed.constantMin / MaxSpeed;
            EmissionMinValueT = Particle.emission.rateOverTime.constantMin / MaxEmission;
        }

        public void SetSpeeds(float _speed)
        {
            var main = Particle.main;
            float maxSpeed = _speed * MaxSpeed;
            float minSpeed = maxSpeed * SpeedMinValueT;
            main.startSpeed = new ParticleSystem.MinMaxCurve(minSpeed, maxSpeed);
            var emission = Particle.emission;
            float maxEmission = _speed * MaxEmission;
            float minEmission = maxEmission * EmissionMinValueT;
            emission.rateOverTime = new ParticleSystem.MinMaxCurve(minEmission, maxEmission);
        }
    }

    [SerializeField] private float m_CloudsRatio;
    [SerializeField] private float m_SpeedY = 1f;
    private float m_OriginalSpeed;
    [SerializeField] private float m_SpeedX = 0.1f;
    [SerializeField] private List<StarsParticles> m_Stars;
    [SerializeField] private Material[] m_CloudMaterials;


    private void Awake()
    {
        m_OriginalSpeed = m_SpeedY;
        SetCloudsSpeed();
        m_Stars.ForEach(s => s.SetValues());
    }

    private void Update()
    {
        SetCloudsSpeed();
        SetStarsValue();
    }

    private void SetStarsValue()
    {
        m_Stars.ForEach(s => s.SetSpeeds(m_SpeedY / m_OriginalSpeed * 2));
    }

    private void SetCloudsSpeed()
    {
        for (int i = 0; i < m_CloudMaterials.Length; i++)
        {
            float speed = m_SpeedY - m_CloudsRatio * i;
            m_CloudMaterials[i].SetVector("_Speed", new Vector2(m_SpeedX, speed));
        }
    }
}
