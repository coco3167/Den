using UnityEngine;

public class VFXHandler : MonoBehaviour
{
    private ParticleSystem m_particleSystem;
    
    private void Awake()
    {
        m_particleSystem = GetComponent<ParticleSystem>();
        m_particleSystem.Stop();
    }

    public void StartParticleSystem()
    {
        m_particleSystem.Play();
    }

    public void ChangeParticlePower(float power)
    {
        ParticleSystem.EmissionModule emission = m_particleSystem.emission;
        emission.rateOverDistanceMultiplier = power;
    }
}