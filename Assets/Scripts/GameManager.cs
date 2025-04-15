using System;
using System.Collections.Generic;
using Sinj;
using Sirenix.OdinInspector;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField, ChildGameObjectsOnly] private EnvironmentManager environmentManager;
    [SerializeField, ChildGameObjectsOnly] private SinjManager sinjManager;
    
    [Title("Pallier")]
    private Dictionary<Emotions, WwiseMoodState> pallierMoodState = new()
    {
        { Emotions.Curiosity, WwiseMoodState.CuriosityState },
        { Emotions.Agression, WwiseMoodState.AngerState },
        { Emotions.Fear, WwiseMoodState.FearState },

    };
    private Dictionary<Emotions, int> m_currentPallier = new()
    {
        { Emotions.Curiosity , 0},
        { Emotions.Agression, 0},
        { Emotions.Fear , 0},
    };
    private const int IntervalPallier = 25;
    
    public event EventHandler GameReady, GameEnded;
    
    public static GameManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            return;
        }
        
        Destroy(gameObject);
    }
    
    public void OnGameReady()
    {
        GameReady?.Invoke(null, EventArgs.Empty);
    }

    public void OnGameEnded()
    {
        GameEnded?.Invoke(null, EventArgs.Empty);
    }

    public void HandlePallier(Emotions emotion, int value)
    {
        if (m_currentPallier[emotion] + IntervalPallier < value)
        {
            PallierReached(emotion);
        }
    }
    
    // ReSharper disable Unity.PerformanceAnalysis
    private void PallierReached(Emotions emotion)
    {
        m_currentPallier[emotion] += IntervalPallier;
        int currentPallierValue = m_currentPallier[emotion];
        WwiseStateManager.SetWwiseMoodState(pallierMoodState[emotion]);
        
        if(currentPallierValue == 100)
            OnGameEnded();
    }
}
