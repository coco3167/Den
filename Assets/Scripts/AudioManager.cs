using System.Collections.Generic;
using UnityEngine;

public enum WwiseMoodState
{
    Curiosity,
    Fear,
    Anger,
    Neutral,
    None
}
public enum WwiseAudioState
{
    StereoHeadphones,
    StereoSpeakers,
    Mono,
    None
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    private bool bIsInitialized = false;

    [Header("Startup Soundbanks")]
    [SerializeField] private List<AK.Wwise.Bank> Soundbanks;

    [Header("Game State Mood Variables")]
    [SerializeField] private AK.Wwise.State Mood_Curiosity;
    [SerializeField] private AK.Wwise.State Mood_Fear;
    [SerializeField] private AK.Wwise.State Mood_Anger;
    [SerializeField] private AK.Wwise.State Mood_Neutral;
    [SerializeField] private AK.Wwise.State Mood_None;

    private WwiseMoodState currentMoodState;

    [Header("Audio State Variables")]
    [SerializeField] private AK.Wwise.State Audio_StereoHeadphones;
    [SerializeField] private AK.Wwise.State Audio_StereoSpeakers;
    [SerializeField] private AK.Wwise.State Audio_Mono;
    [SerializeField] private AK.Wwise.State Audio_None;

    private WwiseAudioState currentAudioState;

    [Header("Wwise Music Events")]
    [SerializeField] private AK.Wwise.Event MainMusic_Play;
    [SerializeField] private AK.Wwise.Event MainMusic_Stop;

    private void Awake()
    {
        Initialize();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetWwiseAudioState(WwiseAudioState.StereoHeadphones);
        SetWwiseMoodState(WwiseMoodState.Neutral);

        MainMusic_Play.Post(gameObject);
    }


    void Initialize()
    {
        //Singleton logic
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Debug.Log("Destroying duplicate AudioManager");
            Destroy(this);
        }
        if (!bIsInitialized)
        {
            LoadSoundbanks();
        }

        SetWwiseMoodState(WwiseMoodState.None);
        SetWwiseAudioState(WwiseAudioState.None);

        bIsInitialized = true;
    }

    void LoadSoundbanks()
    {
        if (Soundbanks.Count > 0)
        {
            foreach (AK.Wwise.Bank bank in Soundbanks)
            {
                bank.Load();
                Debug.Log("Soundbanks have been loaded.");
            }
        }
        else
            Debug.LogError("Soundbanks list is empty ! Are the banks assigned to the AudioManager?");
    }

    public void SetWwiseMoodState(WwiseMoodState stateMood)
    {
        if (stateMood == currentMoodState)
        {
            Debug.Log("Mood is already set to " + stateMood);
            return;
        }

        switch (stateMood)
        {
            case WwiseMoodState.Curiosity:
                Mood_Curiosity.SetValue();
                break;
            case WwiseMoodState.Fear:
                Mood_Fear.SetValue();
                break;
            case WwiseMoodState.Anger:
                Mood_Anger.SetValue();
                break;
            default:
            case WwiseMoodState.Neutral:
                Mood_Neutral.SetValue();
                break;
            case WwiseMoodState.None:
                Mood_None.SetValue();
                break;
        }

        Debug.Log("Mood has been set to " + stateMood);
        currentMoodState = stateMood;
    }

    public void SetWwiseAudioState(WwiseAudioState audioState)
    {
        if (audioState == currentAudioState)
        {
            Debug.Log("Mood is already set to " + audioState);
            return;
        }

        switch (audioState)
        {
            default:
            case WwiseAudioState.StereoHeadphones:
                Audio_StereoHeadphones.SetValue();
                break;
            case WwiseAudioState.StereoSpeakers:
                Audio_StereoSpeakers.SetValue();
                break;
            case WwiseAudioState.Mono:
                Audio_Mono.SetValue();
                break;
            case WwiseAudioState.None:
                Audio_None.SetValue();
                break;
        }

        Debug.Log("Audio state has been set to " + audioState);
        currentAudioState = audioState;
    }
}
