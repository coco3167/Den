using System;
using System.Collections.Generic;
using System.Linq;
using Audio;
using Sinj;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using SmartObjects_AI;
using SmartObjects_AI.Agent;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    [SerializeField, ChildGameObjectsOnly] private Camera mainCamera;

    [SerializeField, ChildGameObjectsOnly] private EnvironmentManager environmentManager;
    [SerializeField, ChildGameObjectsOnly] private SinjManager sinjManager;
    [SerializeField] private MouseManager mouseManager;

    [SerializeField] private UnityEvent<AgentDynamicParameter, int> pallierReached;

    [SerializeField] public WorldParameters worldParameters;

    // Palier
    private readonly Dictionary<AgentDynamicParameter, WwiseMoodState> m_palierMoodState = new()
    {
        { AgentDynamicParameter.Curiosity, WwiseMoodState.CuriosityState },
        { AgentDynamicParameter.Aggression, WwiseMoodState.AngerState },
        { AgentDynamicParameter.Fear, WwiseMoodState.FearState },

    };
    private readonly Dictionary<AgentDynamicParameter, int> m_currentPalier = new()
    {
        { AgentDynamicParameter.Curiosity , 0},
        { AgentDynamicParameter.Aggression, 0},
        { AgentDynamicParameter.Fear , 0},
    };
    public const int IntervalPallier = 25;

    public event EventHandler GameReady, GameEnded;
    public event EventHandler<GamePausedEventArgs> GamePaused;
    private GamePausedEventArgs m_pausedEventArgs;

    public static GameManager Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        worldParameters = new(mouseManager);
        
        pallierReached.AddListener(OnPallierReached);
        
        SceneManager.LoadScene(1, LoadSceneMode.Additive);
        m_pausedEventArgs = new GamePausedEventArgs();
        
        IGameStateListener[] gameStateListeners = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).OfType<IGameStateListener>().ToArray();
        gameStateListeners.ForEach(x => GameReady += x.OnGameReady);
        gameStateListeners.ForEach(x => GameEnded += x.OnGameEnded);

        FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).OfType<IPausable>()
            .ForEach(x => GamePaused += x.OnGamePaused);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
            OnGameEnded();
    }

    public void OnGameReady()
    {
        GameReady?.Invoke(null, EventArgs.Empty);
    }

    public void OnGameEnded()
    {
        GameEnded?.Invoke(null, EventArgs.Empty);
        Time.timeScale = 0;
        Debug.Log("play the ending animation");
        FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).OfType<IReloadable>()
            .ForEach(x => x.Reload());
        Debug.Log("go back to the game with animation");
        Time.timeScale = 1;
        OnGameReady();
    }

    public void HandlePallier(AgentDynamicParameter parameter, int value)
    {
        if (m_currentPalier[parameter] + IntervalPallier < value)
        {
            pallierReached.Invoke(parameter, m_currentPalier[parameter] + IntervalPallier);
        }
    }

    public void OnMouseMoved(InputAction.CallbackContext callbackContext)
    {
        if (m_pausedEventArgs.IsPaused)
            return;
        mouseManager.OnMouseMoved(callbackContext.ReadValue<Vector2>());
    }

    public void OnOtherMoved(InputAction.CallbackContext callbackContext)
    {
        if (m_pausedEventArgs.IsPaused)
            return;

        if (callbackContext.performed)
            mouseManager.OnOtherMoveStart(callbackContext.ReadValue<Vector2>());
        else if (callbackContext.canceled)
            mouseManager.OnOtherMoveEnd();
    }

    public void OnPause(InputAction.CallbackContext callbackContext)
    {
        m_pausedEventArgs.IsPaused = !m_pausedEventArgs.IsPaused;
        Cursor.lockState = m_pausedEventArgs.IsPaused ? CursorLockMode.None : CursorLockMode.Locked;
        Time.timeScale = m_pausedEventArgs.IsPaused ? 0 : 1;
        GamePaused?.Invoke(this, m_pausedEventArgs);
    }

    public Camera GetCamera()
    {
        return mainCamera;
    }

    public MouseManager GetMouseManager()
    {
        return mouseManager;
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void OnPallierReached(AgentDynamicParameter parameter, int nextPallier)
    {
        m_currentPalier[parameter] = nextPallier;
        WwiseStateManager.SetWwiseMoodState(m_palierMoodState[parameter]);

        if (m_currentPalier[parameter] >= 100)
            OnGameEnded();
    }

    #region EventArgs
    public class GamePausedEventArgs : EventArgs
    {
        public bool IsPaused;
    }
    #endregion

}

public interface IReloadable
{
    public void Reload();
}

public interface IGameStateListener
{
    public void OnGameReady(object sender, EventArgs eventArgs);
    public void OnGameEnded(object sender, EventArgs eventArgs);
}

public interface IPausable
{
    public void OnGamePaused(object sender, GameManager.GamePausedEventArgs eventArgs);
}