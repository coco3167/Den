using System;
using System.Linq;
using DG.Tweening;
using Sirenix.Utilities;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class GameLoopManager : MonoBehaviour
{
    [SerializeField] private Skybox skybox;
    [SerializeField] private float gameLoopDuration;
    
    private Animator m_animator;
    private Tween m_skyboxTween;
    public event EventHandler GameReady, GameEnded;

    public static GameLoopManager Instance;
    private static readonly int EndGame = Animator.StringToHash("EndGame");
    private static readonly int TimeOfDay = Shader.PropertyToID("_TimeOfDay");

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        
        m_animator = GetComponent<Animator>();
        m_skyboxTween = skybox.material.DOFloat(300, TimeOfDay, gameLoopDuration);
        m_skyboxTween.Pause();
        m_skyboxTween.SetEase(Ease.Linear);
        
        IGameStateListener[] gameStateListeners = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).OfType<IGameStateListener>().ToArray();
        gameStateListeners.ForEach(x => GameReady += x.OnGameReady);
        gameStateListeners.ForEach(x => GameEnded += x.OnGameEnded);

        
    }

    public void OnGameLoopReady()
    {
        skybox.material.SetFloat(TimeOfDay, 0);
        Debug.Log(skybox.material.GetFloat(TimeOfDay));
        GameReady?.Invoke(null, EventArgs.Empty);
        m_skyboxTween.Play();
    }

    public void OnGameLoopEnded()
    {
        GameEnded?.Invoke(null, EventArgs.Empty);
        m_animator.SetTrigger(EndGame);
        m_skyboxTween.Kill();
        Time.timeScale = 0;
        
        Debug.Log("play the ending animation");
        FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).OfType<IReloadable>()
            .ForEach(x => x.Reload());
        Debug.Log("go back to the game with animation");
        
        Time.timeScale = 1;
        OnGameLoopReady();
    }
}
