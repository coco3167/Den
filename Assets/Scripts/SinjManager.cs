using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class SinjManager : MonoBehaviour
{
    [Header("Sinjs")]
    [SerializeField, AssetsOnly, AssetSelector(Paths = "Assets/Prefab")] private GameObject sinjPrefab;
    [SerializeField, Range(1,10)] private int sinjCount;
    [SerializeField, ReadOnly] private List<Sinj> sinjs = new();

    [Header("Navigation")]
    [SerializeField] private EnvironmentManager environmentManager;
    
    [Header("Mouse Input")]
    [SerializeField] private MouseManager mouseManager;

    private void Awake()
    {
        sinjs.Clear();
        for (int loop = 0; loop < sinjCount; loop++)
        {
            sinjs.Add(Instantiate(sinjPrefab, transform).GetComponent<Sinj>());
            sinjs[loop].Init();
        }
    }

    private void Update()
    {
        foreach (Sinj sinj in sinjs)
        {
            sinj.ReactToMouseDistance(mouseManager.ObjectDistanceToMouse(sinj.transform.position), mouseManager.GetRawWorldMousePosition());
        }
    }
}