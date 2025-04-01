using System.Collections.Generic;
using UnityEngine;

public enum WwiseSwitchMood
{
    Curiosity,
    Fear,
    Anger,
    MNeutral,
    None
}
public enum WwiseAudioState
{
    StereoHeadphones,
    StereoSpeakers,
    Mono,
    None
}
public enum WwiseMusicState
{
    LevelStart,
    LevelPauseMenu,
    None
}
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    private bool bIsInitialized = false;

    [Header("Startup Soundbanks")]
    [SerializeField] private List<AK.Wwise.Bank> Soundbanks;

    [Header("Game Switch Variables")]
    [SerializeField] private AK.Wwise.Switch Mood_Curiosity;
    [SerializeField] private AK.Wwise.Switch Mood_Fear;
    [SerializeField] private AK.Wwise.Switch Mood_Anger;
    [SerializeField] private AK.Wwise.Switch Mood_Neutral;
    [SerializeField] private AK.Wwise.Switch Mood_None;

    [Header("Game State Variables")]
    [SerializeField] private AK.Wwise.State Audio_StereoHeadphones;
    [SerializeField] private AK.Wwise.State Audio_StereoSpeakers;
    [SerializeField] private AK.Wwise.State Audio_Mono;
    [SerializeField] private AK.Wwise.State Audio_None;

    [Header("Music State Variables")]
    [SerializeField] private AK.Wwise.State Music_LevelStart;
    [SerializeField] private AK.Wwise.State Music_LevelPauseMenu;
    [SerializeField] private AK.Wwise.State Music_None;


    private void Awake()
    {
        Initialize();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

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

    void SetWwiseSwitchmood(WwiseSwitchMood switchMood)
    {
        switch (switchMood)
        {
            case WwiseSwitchMood.Curiosity:
                Mood_Curiosity.SetValue();
                break;
            case WwiseSwitchMood.Fear:
                Mood_Fear.SetValue();
                break;
            case WwiseSwitchMood.Anger:
                Mood_Anger.SetValue();
                break;
            case WwiseSwitchMood.MNeutral:
                Mood_Neutral.SetValue();
                break;
            case WwiseSwitchMood.None:
                Mood_None.SetValue();
                break;
        }
    }
}
