using System.Collections.Generic;
using DebugHUD;
using Sirenix.OdinInspector;
using UnityEngine;

public class SinjManager : MonoBehaviour, IDebugDisplayAble
{
    [Header("Sinjs")]
    [SerializeField, AssetsOnly, AssetSelector(Paths = "Assets/Prefab")] private GameObject sinjPrefab;
    [SerializeField, Range(1,10)] private int sinjCount;
    [SerializeField, ReadOnly] private List<Sinj> sinjs = new();

    [Header("Navigation")]
    [SerializeField] private EnvironmentManager environmentManager;
    
    [Header("Mouse Input")]
    [SerializeField] private MouseManager mouseManager;
    
    private List<DebugParameter> debugParameters = new();

    private void Awake()
    {
        sinjs.Clear();
        for (int loop = 0; loop < sinjCount; loop++)
        {
            sinjs.Add(Instantiate(sinjPrefab, transform).GetComponent<Sinj>());
            sinjs[loop].Init(mouseManager);
        }
        
        DebugParameter debugParameter = new DebugParameter();
        debugParameter.Name = "Health";
        debugParameter.Value = 0;
        debugParameters.Add(debugParameter);
    }

    private void Update()
    {
        foreach (Sinj sinj in sinjs)
        {
            sinj.HandleStimuli();
        }
    }

    public int GetParameterCount()
    {
        return debugParameters.Count;
    }

    public DebugParameter GetParameter(int index)
    {
        return debugParameters[index];
    }
}