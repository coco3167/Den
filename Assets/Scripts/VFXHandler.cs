using System.Collections.Generic;
using SmartObjects_AI.Agent;
using UnityEngine;

public class VFXHandler : MonoBehaviour
{
    [SerializeField] private AgentDynamicParameter selfParameter;
    [SerializeField] private List<ParticleSystem> particleSystems;
    [SerializeField] private List<float> ratesValues;
    
    private void Awake()
    {
        GameManager.Instance.pallierReached.AddListener(StartParticleSystem);
    }

    private void StartParticleSystem(AgentDynamicParameter parameter, int pallier)
    {
        if(selfParameter != parameter)
            return;
        
        Debug.Log("here");  
        
        ChangeParticlePower(pallier/GameManager.IntervalPallier-1);
        
        foreach (ParticleSystem p in particleSystems)
        {
            p.Play();
        }
    }

    private void ChangeParticlePower(int index)
    {
        foreach (ParticleSystem p in particleSystems)
        {
            ParticleSystem.EmissionModule emission = p.emission;
            emission.rateOverTime = ratesValues[index];
        }
    }
}