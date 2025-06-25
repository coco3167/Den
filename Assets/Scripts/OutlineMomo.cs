using System;
using System.Collections.Generic;
using Sirenix.Utilities;
using SmartObjects_AI.Agent;
using UnityEngine;

public class OutlineMomo : MonoBehaviour, IPausable, IGameStateListener
{
    [SerializeField] private Material baseMaterial, outlineMaterial;

    private List<SkinnedMeshRenderer> m_skinnedMeshRenderers = new();

    public void OnGameReady(object sender, EventArgs eventArgs)
    {
        GetComponentsInChildren<AnimationAgent>().ForEach(x => m_skinnedMeshRenderers.Add(x.skinnedMeshRenderer));
    }

    public void OnGameEnded(object sender, EventArgs eventArgs)
    {
        //
    }

    public void OnGamePaused(object sender, EventArgs eventArgs)
    {
        if(GameManager.Instance.IsPaused)
            return;
        
        foreach (SkinnedMeshRenderer skinnedMeshRenderer in m_skinnedMeshRenderers)
        {
            skinnedMeshRenderer.material = Options.GameParameters.IsOutline ? outlineMaterial : baseMaterial;
        }
    }
}
