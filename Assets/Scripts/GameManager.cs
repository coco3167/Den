using System;
using System.Collections.Generic;
using Audio;
using Sinj;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    [SerializeField, ChildGameObjectsOnly] private EnvironmentManager environmentManager;
    [SerializeField, ChildGameObjectsOnly] private SinjManager sinjManager;
    [SerializeField] private MouseManager mouseManager;
    
    [SerializeField] private UnityEvent<Emotions> pallierReached;
    
    // Palier
    private readonly Dictionary<Emotions, WwiseMoodState> m_palierMoodState = new()
    {
        { Emotions.Curiosity, WwiseMoodState.CuriosityState },
        { Emotions.Agression, WwiseMoodState.AngerState },
        { Emotions.Fear, WwiseMoodState.FearState },

    };
    private readonly Dictionary<Emotions, int> m_currentPalier = new()
    {
        { Emotions.Curiosity , 0},
        { Emotions.Agression, 0},
        { Emotions.Fear , 0},
    };
    private const int IntervalPallier = 25;

    // Events
    public event EventHandler GameReady, GameEnded;
    public event EventHandler<GamePausedEventArgs> GamePaused;
    private GamePausedEventArgs m_pausedEventArgs;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            pallierReached.AddListener(OnPallierReached);
            SceneManager.LoadScene(1, LoadSceneMode.Additive);
            m_pausedEventArgs = new GamePausedEventArgs();
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
        if (m_currentPalier[emotion] + IntervalPallier < value)
        {
            pallierReached.Invoke(emotion);
        }
    }

    public void OnMouseMoved(InputAction.CallbackContext callbackContext)
    {
        if(m_pausedEventArgs.IsPaused)
            return;
        mouseManager.OnMouseMoved(callbackContext.ReadValue<Vector2>());
    }

    public void OnOtherMoved(InputAction.CallbackContext callbackContext)
    {
        if(m_pausedEventArgs.IsPaused)
            return;
        
        if(callbackContext.performed)
            mouseManager.OnOtherMoveStart(callbackContext.ReadValue<Vector2>());
        else if(callbackContext.canceled)
            mouseManager.OnOtherMoveEnd();
    }

    public void OnPause(InputAction.CallbackContext callbackContext)
    {
        m_pausedEventArgs.IsPaused = !m_pausedEventArgs.IsPaused;
        Cursor.lockState = m_pausedEventArgs.IsPaused ? CursorLockMode.None : CursorLockMode.Locked;
        Time.timeScale = m_pausedEventArgs.IsPaused ? 0 : 1;
        GamePaused?.Invoke(this, m_pausedEventArgs);
    }
    
    // ReSharper disable Unity.PerformanceAnalysis
    private void OnPallierReached(Emotions emotion)
    {
        m_currentPalier[emotion] += IntervalPallier;
        int currentPalierValue = m_currentPalier[emotion];
        WwiseStateManager.SetWwiseMoodState(m_palierMoodState[emotion]);
        
        if(currentPalierValue == 100)
            OnGameEnded();
    }

    #region EventArgs
    public class GamePausedEventArgs : EventArgs
    {
        public bool IsPaused;
    }
    #endregion
    
}