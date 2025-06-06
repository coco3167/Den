using System.Collections.Generic;
using UnityEngine;

public class VFXHandler : MonoBehaviour
{
    [SerializeField] private List<ParticleSystem> m_particleSystem;
    [SerializeField] private List<float> m_ratesValues;
    
    private void Awake()
    {
        foreach (ParticleSystem p in m_particleSystem)
        {
            p.Stop();
        }
    }

    public void StartParticleSystem()
    {
        foreach (ParticleSystem p in m_particleSystem)
        {
            p.Play();
        }
    }

    public void ChangeParticlePower(int index)
    {
        foreach (ParticleSystem p in m_particleSystem)
        {
            ParticleSystem.EmissionModule emission = p.emission;
            emission.rateOverTime = m_ratesValues[index];
        }
        
    }
}