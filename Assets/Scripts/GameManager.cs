using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Audio;
using AYellowpaper.SerializedCollections;
using Options;
using Sinj;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using SmartObjects_AI;
using SmartObjects_AI.Agent;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Windows.WebCam;

[RequireComponent(typeof(PlayerInput))]
public class GameManager : MonoBehaviour, IGameStateListener
{

    [SerializeField, ChildGameObjectsOnly] private Camera mainCamera;

    [SerializeField, ChildGameObjectsOnly] private EnvironmentManager environmentManager;
    [SerializeField, ChildGameObjectsOnly] private SinjManager sinjManager;
    [SerializeField] private MouseManager mouseManager;

    [SerializeField] public UnityEvent<AgentDynamicParameter, int> pallierReached;

    [SerializeField] public WorldParameters worldParameters;

    [SerializeField] private SerializedDictionary<AgentDynamicParameter, float> timeBeforeRealEnd;

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

    public event EventHandler GamePaused;
    public bool IsPaused { get; private set; }

    //public event EventHandler<GamePausedEventArgs> GamePaused;
    //public GamePausedEventArgs PausedEventArgs { get; private set; }

    private PlayerInput m_playerInput;

    public static GameManager Instance;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        m_playerInput = GetComponent<PlayerInput>();

        worldParameters = new(mouseManager);

        pallierReached.AddListener(OnPallierReached);

        SceneManager.LoadScene(1, LoadSceneMode.Additive);
        // PausedEventArgs = new GamePausedEventArgs();

        FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).OfType<IPausable>()
            .ForEach(x => GamePaused += x.OnGamePaused);
        //IsPaused = true;

        //m_playerInput.enabled = false;

        Cursor.visible = false;
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    public void OnGameReady(object sender, EventArgs eventArgs)
    {
        IsPaused = false;
        m_playerInput.enabled = true;
    }

    public void OnGameEnded(object sender, EventArgs eventArgs)
    {
        //IsPaused = true;
        //m_playerInput.enabled = false;
        worldParameters.Reload();
    }


    public void OnPause(InputAction.CallbackContext callbackContext)
    {
        if (!callbackContext.started)
            return;
        IsPaused = !IsPaused;
        Cursor.lockState = IsPaused ? CursorLockMode.None : CursorLockMode.Locked;
        Time.timeScale = IsPaused ? 0 : Options.GameParameters.TimeScale;

        // Pause or resume ambience
        if (IsPaused)
        {
            AudioManager.Instance.PauseAmbience();
            AudioManager.Instance.StopCursorMoveSound(mouseManager.GetMouseAura());
        }
        else
        {
            AudioManager.Instance.ResumeAmbience();
            AudioManager.Instance.StartCursorMoveSound(mouseManager.GetMouseAura());
        }

        GamePaused?.Invoke(this, EventArgs.Empty);
    }

    public void Reload()
    {
        foreach (var key in m_currentPalier.Keys.ToList())
        {
            m_currentPalier[key] = 0;
        }
    }

    public void OnManualGameEnded(InputAction.CallbackContext callbackContext)
    {
        if (!callbackContext.started)
            return;
        if (IsPaused)
        {
            OnPause(callbackContext);
        }
        GameLoopManager.Instance.OnGameLoopEnded(AgentDynamicParameter.Curiosity, true);
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
        if (IsPaused)
            return;
        mouseManager.OnMouseMoved(callbackContext.ReadValue<Vector2>());
    }

    public void OnOtherMoved(InputAction.CallbackContext callbackContext)
    {
        if (IsPaused)
            return;

        if (callbackContext.performed)
            mouseManager.OnOtherMoveStart(callbackContext.ReadValue<Vector2>());
        else if (callbackContext.canceled)
            mouseManager.OnOtherMoveEnd();
    }

	public void OnSensiChanged(InputAction.CallbackContext callbackContext)
    {
        if(!callbackContext.performed)
            return;
        
        GameParameters.SensitivityChange(callbackContext.ReadValue<float>());
    }

    public void InfluencedByMouse(bool value)
    {
        mouseManager.IsUsed = value;
        sinjManager.InfluencedByMouse(value);
    }

    public void ResetEmotionPalliers()
    {
        foreach (var key in m_currentPalier.Keys.ToList())
            m_currentPalier[key] = 0;
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
        AudioManager.Instance.PlayEmotionSteps(parameter, nextPallier);

        if (m_currentPalier[parameter] >= 100)
        {
            InfluencedByMouse(false);
            StartCoroutine(CallGameLoopEnd(parameter));
            AudioManager.Instance.PlayEndMusic();
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private IEnumerator CallGameLoopEnd(AgentDynamicParameter parameter)
    {
        yield return new WaitForSeconds(timeBeforeRealEnd[parameter]);
        GameLoopManager.Instance.OnGameLoopEnded(parameter);
    }

    // #region EventArgs
    // public class GamePausedEventArgs : EventArgs
    // {
    //     public bool IsPaused;
    // }
    // #endregion

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
    public void OnGamePaused(object sender, EventArgs eventArgs);
}