using System;
using Sinj;
using Sirenix.OdinInspector;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField, ChildGameObjectsOnly] private EnvironmentManager environmentManager;
    [SerializeField, ChildGameObjectsOnly] private SinjManager sinjManager;

    public static event EventHandler GameReady;

    public static void OnGameReady()
    {
        GameReady?.Invoke(null, EventArgs.Empty);
    }
}
