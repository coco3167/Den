using System;
using System.Collections;
using System.Linq;
using Audio;
using DG.Tweening;
using Sirenix.Utilities;
using SmartObjects_AI.Agent;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class GameLoopManager : MonoBehaviour, IPausable
{
    [SerializeField] private AnimationClip loopAnim;
    [SerializeField] private IntroManager introManager;
    [SerializeField] private RawImage blackBackground;
    [SerializeField] private SymbolManager symbolManager;

    private Animator m_animator;
    private Tween m_tween;

    private float m_gameLoopDuration;

    public GameLoopState currentGameLoopState { get; private set; }
    public event EventHandler GameReady, GameEnded, Reload;

    public static GameLoopManager Instance;

    // Animator cached id
    private static readonly int EndGame = Animator.StringToHash("EndGame");

    // Shader cached id
    private static readonly int TimeOfDay = Shader.PropertyToID("_TimeOfDay");
    private static readonly int LoopState = Shader.PropertyToID("_GameLoopState");
    private static readonly int Restart = Animator.StringToHash("Restart");
    private static readonly int StartGame = Animator.StringToHash("StartGame");

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

        m_tween = DOTween.To(() => Shader.GetGlobalFloat(TimeOfDay), (value) => Shader.SetGlobalFloat(TimeOfDay, value), 300, m_gameLoopDuration);
        m_tween.Pause();
        m_tween.SetEase(Ease.Linear);

        IGameStateListener[] gameStateListeners = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).OfType<IGameStateListener>().ToArray();
        gameStateListeners.ForEach(x => GameReady += x.OnGameReady);
        gameStateListeners.ForEach(x => GameEnded += x.OnGameEnded);
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    public void OnGameLoopReady()
    {
        m_animator.SetTrigger(StartGame);
        Shader.SetGlobalFloat(TimeOfDay, 0);
        ChangeGameLoopState(GameLoopState.Morning);
        GameReady?.Invoke(null, EventArgs.Empty);
        m_tween.Play();
    }

    public void OnGameLoopEnded(AgentDynamicParameter parameter, bool manual = false)
    {
        if (manual)
            m_animator.SetTrigger(EndGame);
        
        GameEnded?.Invoke(null, EventArgs.Empty);
        m_tween.Kill();

        blackBackground.gameObject.SetActive(true);
        AudioManager.Instance.ResetEndMusic();
        blackBackground.DOColor(Color.white, 5).OnComplete(() => symbolManager.LastAppearance(parameter)).Play();
        
        AudioManager.Instance.StopCursorMoveSound(GameManager.Instance.GetMouseManager().GetMouseAura());

        //introManager.LoopReset();
        //StartCoroutine(RestartCoroutine());
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

    private IEnumerator RestartCoroutine()
    {
        WaitForEndOfFrame endOfFrame = new WaitForEndOfFrame();

        while (introManager.step < 3)
        {
            yield return endOfFrame;
        }

        FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).OfType<IReloadable>()
            .ForEach(x => x.Reload());
        m_animator.SetTrigger(Restart);
    }

    public void ChangeGameLoopState(GameLoopState state)
    {
        currentGameLoopState = state;
        Shader.SetGlobalInteger(LoopState, (int)state);
        AudioManager.Instance.SetWwiseTODState(AudioManager.ToWwiseTODState(state));
    }

    public enum GameLoopState
    {
        Morning,
        Day,
        Evening,
        Night,
    }
}
