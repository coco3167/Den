using System;
using System.Linq;
using DG.Tweening;
using Sirenix.Utilities;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class GameLoopManager : MonoBehaviour, IPausable
{
    [SerializeField] private AnimationClip loopAnim;
    
    private Animator m_animator;
    private Tween m_tween;
    
    private float m_gameLoopDuration;
    
    public GameLoopState currentGameLoopState { get; private set; }
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
        m_gameLoopDuration = loopAnim.length;
        Debug.Log(m_gameLoopDuration);
        
        m_tween = DOTween.To(() => Shader.GetGlobalFloat(TimeOfDay), (value) => Shader.SetGlobalFloat(TimeOfDay, value), 300, m_gameLoopDuration);
        m_tween.Pause();
        m_tween.SetEase(Ease.Linear);
        
        IGameStateListener[] gameStateListeners = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).OfType<IGameStateListener>().ToArray();
        gameStateListeners.ForEach(x => GameReady += x.OnGameReady);
        gameStateListeners.ForEach(x => GameEnded += x.OnGameEnded);
    }

    public void OnGameLoopReady()
    {
        Shader.SetGlobalFloat(TimeOfDay, 0);
        ChangeGameLoopState(GameLoopState.Morning);
        GameReady?.Invoke(null, EventArgs.Empty);
        m_tween.Play();
    }

    public void OnGameLoopEnded()
    {
        GameEnded?.Invoke(null, EventArgs.Empty);
        m_animator.SetTrigger(EndGame);
        m_tween.Kill();
        Time.timeScale = 0;
        
        Debug.Log("play the ending animation");
        FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).OfType<IReloadable>()
            .ForEach(x => x.Reload());
        Debug.Log("go back to the game with animation");
        
        Time.timeScale = 1;
        OnGameLoopReady();
    }

    public void OnGamePaused(object sender, EventArgs eventArgs)
    {
        if (GameManager.Instance.IsPaused)
        {
            m_tween.Pause();
            return;
        }
        m_tween.Play();
    }

    public void ChangeGameLoopState(GameLoopState state)
    {
        currentGameLoopState = state;
    }

    public enum GameLoopState
    {
        Morning,
        Day,
        Evening,
        Night,
    }
}
