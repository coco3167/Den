using System;
using System.Linq;
using Sirenix.Utilities;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class GameLoopManager : MonoBehaviour
{
    private Animator m_animator;
    
    public event EventHandler GameReady, GameEnded;

    public static GameLoopManager Instance;
    private static readonly int EndGame = Animator.StringToHash("EndGame");

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        
        m_animator = GetComponent<Animator>();
        
        IGameStateListener[] gameStateListeners = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).OfType<IGameStateListener>().ToArray();
        gameStateListeners.ForEach(x => GameReady += x.OnGameReady);
        gameStateListeners.ForEach(x => GameEnded += x.OnGameEnded);

        
    }

    public void OnGameLoopReady()
    {
        GameReady?.Invoke(null, EventArgs.Empty);
    }

    public void OnGameLoopEnded()
    {
        GameEnded?.Invoke(null, EventArgs.Empty);
        m_animator.SetTrigger(EndGame);
        Time.timeScale = 0;
        
        Debug.Log("play the ending animation");
        FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).OfType<IReloadable>()
            .ForEach(x => x.Reload());
        Debug.Log("go back to the game with animation");
        
        Time.timeScale = 1;
        OnGameLoopReady();
    }
}
