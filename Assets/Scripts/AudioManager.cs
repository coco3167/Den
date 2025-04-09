using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using AK.Wwise;
using AYellowpaper.SerializedCollections;

public enum WwiseMoodState
{
    CuriosityState,
    FearState,
    AngerState,
    NeutralState,
    NoneState
}
public enum WwiseAudioState
{
    StereoHeadphones,
    StereoSpeakers,
    Mono,
    None
}

public enum WwiseEmotionStateRTPC
{
    Curiosity,
    Fear,
    Anger,
    Intensity,
    Tension
}

public enum WwiseCuriositySwitch 
{
    Curiosity_0,
    Curiosity_1,
    Curiosity_2,
    Curiosity_3,
}
public enum WwiseFearSwitch
{
    Fear_0,
    Fear_1,
    Fear_2,
    Fear_3,
}
public enum WwiseAngerSwitch
{
    Anger_0,
    Anger_1,
    Anger_2,
    Anger_3,
}

[RequireComponent(typeof(AkGameObj))]
public class AudioManager : MonoBehaviour
{

    public static AudioManager Instance;

    private bool bIsInitialized = false;

    [Title("Startup Soundbanks")]
    [SerializeField] private List<Bank> Soundbanks;

    [Title("Game State Mood Variables")] [ReadOnly, SerializeField, HideLabel] private bool stateMoodSeparator;
    [SerializeField] private SerializedDictionary<WwiseMoodState, State> gameStateMoodVisualization;
    [SerializeField, ReadOnly] private WwiseMoodState currentMoodState;

    [Title("Audio State Variables")] [ReadOnly, SerializeField, HideLabel] private bool audioStateSeparator;
    [SerializeField] private SerializedDictionary<WwiseAudioState, State> audioState;
    [SerializeField, ReadOnly] private WwiseAudioState currentAudioState;

    [Title("Wwise Mood Switches")] [ReadOnly, SerializeField, HideLabel] private bool moodSwitchSeparator;
    [SerializeField] private SerializedDictionary<WwiseCuriositySwitch, Switch> moodCuriosity;
    [SerializeField] private SerializedDictionary<WwiseFearSwitch, Switch> moodFear;
    [SerializeField] private SerializedDictionary<WwiseAngerSwitch, Switch> moodAnger;

    [SerializeField, ReadOnly] private WwiseCuriositySwitch currentCuriositySwitch;
    [SerializeField, ReadOnly] private WwiseFearSwitch currentFearSwitch;
    [SerializeField, ReadOnly] private WwiseAngerSwitch currentAngerSwitch;

    [Title("Wwise Game Parameters")] [ReadOnly, SerializeField, HideLabel] private bool parametersSeparators;
    [SerializeField] private SerializedDictionary<WwiseEmotionStateRTPC, RTPC> gameParameters;

    [SerializeField, ReadOnly] private SerializedDictionary<WwiseEmotionStateRTPC, float> currentGameParametersValues;

    [Title("Wwise Events")]
    [SerializeField] private AK.Wwise.Event AmbienceTest;
    [SerializeField] private AK.Wwise.Event MusicTest;
    [SerializeField] private AK.Wwise.Event PlayNeutrakBarks;
    [SerializeField] private AK.Wwise.Event PlayMoodBarks;
    [SerializeField] private AK.Wwise.Event PlayAngerStep;
    [SerializeField] private AK.Wwise.Event PlayCuriousStep;
    [SerializeField] private AK.Wwise.Event PlayFearStep;

    [SerializeField] private GameObject BarkerTest;

    private void Awake()
    {
        Initialize();
        currentGameParametersValues = new()
        {
            {WwiseEmotionStateRTPC.Curiosity, 0f},
            {WwiseEmotionStateRTPC.Fear, 0f},
            {WwiseEmotionStateRTPC.Anger, 0f},
            {WwiseEmotionStateRTPC.Intensity, 0f},
            {WwiseEmotionStateRTPC.Tension, 0f},
        
        };
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetWwiseAudioState(WwiseAudioState.StereoHeadphones);
        SetWwiseMoodState(WwiseMoodState.NeutralState);

        //AmbienceTest.Post(gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            SetWwiseMoodState(WwiseMoodState.NeutralState);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            SetWwiseMoodState(WwiseMoodState.CuriosityState);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            SetWwiseMoodState(WwiseMoodState.AngerState);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            SetWwiseMoodState(WwiseMoodState.FearState);
        }
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

        SetWwiseMoodState(WwiseMoodState.NoneState);
        SetWwiseAudioState(WwiseAudioState.None);

        bIsInitialized = true;
    }

    void LoadSoundbanks()
    {
        if (Soundbanks.Count > 0)
        {
            foreach (Bank bank in Soundbanks)
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

        gameStateMoodVisualization[stateMood].SetValue();

        Debug.Log("Mood has been set to " + stateMood);
        currentMoodState = stateMood;
    }
    public void SetWwiseAudioState(WwiseAudioState newAudioState)
    {
        if (newAudioState == currentAudioState)
        {
            Debug.Log("Mood is already set to " + audioState);
            return;
        }
        
        audioState[newAudioState].SetValue();
        
        Debug.Log("Audio state has been set to " + newAudioState);
        currentAudioState = newAudioState;
    }

    public void SetWwiseCuriositySwitch(WwiseCuriositySwitch switchState)
    {
        if (switchState == currentCuriositySwitch)
        {
            Debug.Log("Curiosity switch is already set to " + switchState);
            return;
        }
        
        moodCuriosity[switchState].SetValue(gameObject);
        int amount = (int)switchState * 25;
        
        SetWwiseEmotionRTPC(Sinj.Emotions.Curiosity, gameObject, amount);
        Debug.Log("Curiosity switch has been set to " + switchState);
        currentCuriositySwitch = switchState;
    }
    public void SetWwiseFearSwitch(WwiseFearSwitch switchState)
    {
        if (switchState == currentFearSwitch)
        {
            Debug.Log("Fear switch is already set to " + switchState);
            return;
        }
        
        moodFear[switchState].SetValue(gameObject);
        int amount = (int)switchState * 25;
        
        SetWwiseEmotionRTPC(Sinj.Emotions.Fear, gameObject, amount);
        Debug.Log("Fear switch has been set to " + switchState);
        currentFearSwitch = switchState;
    }
    public void SetWwiseAngerSwitch(WwiseAngerSwitch switchState)
    {
        if (switchState == currentAngerSwitch)
        {
            Debug.Log("Anger switch is already set to " + switchState);
            return;
        }
        
        moodAnger[switchState].SetValue(gameObject);
        int amount = (int)switchState * 25;
        
        SetWwiseEmotionRTPC(Sinj.Emotions.Agression, gameObject, amount);
        
        Debug.Log("Anger switch has been set to " + switchState);
        currentAngerSwitch = switchState;
    }

    public void SetWwiseEmotionRTPC(Sinj.Emotions emotion, GameObject target, float value)
    {
        WwiseEmotionStateRTPC emotionStateRtpc = TranslateSinjAgentEmotionToAudioManagerEmotion(emotion);
        RTPC emotionRTPC = gameParameters[emotionStateRtpc];
        
        if (Mathf.Approximately(value, currentGameParametersValues[emotionStateRtpc]))
        {
            Debug.Log($"{emotionStateRtpc.ToString()} value is already set to {value}");
            return;
        }
        // Ensure that the target has an AkGameObject component.
        if (!target.GetComponent<AkGameObj>())
        {
            target.AddComponent<AkGameObj>();
        }

        // Use 'target' instead of 'gameObject'.
        emotionRTPC.SetValue(target, value);

        Debug.Log("Anger value has been set to " + value);
        currentGameParametersValues[emotionStateRtpc] = value;
    }

    private WwiseEmotionStateRTPC TranslateSinjAgentEmotionToAudioManagerEmotion(Sinj.Emotions emotion)
    {
        switch(emotion)
        {
            case Sinj.Emotions.Curiosity:
                return WwiseEmotionStateRTPC.Curiosity;
            case Sinj.Emotions.Fear:
                return WwiseEmotionStateRTPC.Fear;
            case Sinj.Emotions.Agression:
                return WwiseEmotionStateRTPC.Anger;
            case Sinj.Emotions.Tension:
                return WwiseEmotionStateRTPC.Tension;
            default:
                return WwiseEmotionStateRTPC.Intensity;
        }
    }

}