using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using AK.Wwise;

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

    [Header("Startup Soundbanks")]
    [SerializeField] private List<AK.Wwise.Bank> Soundbanks;

    [Header("Game State Mood Variables")]
    [SerializeField] private AK.Wwise.State Mood_Curiosity;
    [SerializeField] private AK.Wwise.State Mood_Fear;
    [SerializeField] private AK.Wwise.State Mood_Anger;
    [SerializeField] private AK.Wwise.State Mood_Neutral;
    [SerializeField] private AK.Wwise.State Mood_None;

    [SerializeField] private WwiseMoodState currentMoodState;

    [Header("Audio State Variables")]
    [SerializeField] private AK.Wwise.State Audio_StereoHeadphones;
    [SerializeField] private AK.Wwise.State Audio_StereoSpeakers;
    [SerializeField] private AK.Wwise.State Audio_Mono;
    [SerializeField] private AK.Wwise.State Audio_None;

    [SerializeField] private WwiseAudioState currentAudioState;

    [Header("Wwise Mood Switches")]
    [SerializeField] private AK.Wwise.Switch Mood_Curiosity_0;
    [SerializeField] private AK.Wwise.Switch Mood_Curiosity_1;
    [SerializeField] private AK.Wwise.Switch Mood_Curiosity_2;
    [SerializeField] private AK.Wwise.Switch Mood_Curiosity_3;
    [SerializeField] private AK.Wwise.Switch Mood_Fear_0;
    [SerializeField] private AK.Wwise.Switch Mood_Fear_1;
    [SerializeField] private AK.Wwise.Switch Mood_Fear_2;
    [SerializeField] private AK.Wwise.Switch Mood_Fear_3;
    [SerializeField] private AK.Wwise.Switch Mood_Anger_0;
    [SerializeField] private AK.Wwise.Switch Mood_Anger_1;
    [SerializeField] private AK.Wwise.Switch Mood_Anger_2;
    [SerializeField] private AK.Wwise.Switch Mood_Anger_3;

    [SerializeField] private WwiseCuriositySwitch currentCuriositySwitch;
    [SerializeField] private WwiseFearSwitch currentFearSwitch;
    [SerializeField] private WwiseAngerSwitch currentAngerSwitch;

    [Header("Wwise Game Parameters")]
    [SerializeField] private AK.Wwise.RTPC Curiosity_Value;
    [SerializeField] private AK.Wwise.RTPC Fear_Value;
    [SerializeField] private AK.Wwise.RTPC Anger_Value;
    [SerializeField] private AK.Wwise.RTPC Intensity_Value;
    [SerializeField] private AK.Wwise.RTPC Tension_Value;

    [Range(0, 100)]
    public float currentCuriosityValue;
    [Range(0, 100)]
    public float currentFearValue;
    [Range(0, 100)]
    public float currentAngerValue;
    [Range(0, 100)]
    public float currentIntensityValue;
    [Range(0, 100)]
    public float currentTensionValue;

    [Header("Wwise Events")]
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
            case WwiseMoodState.CuriosityState:
                Mood_Curiosity.SetValue();
                break;
            case WwiseMoodState.FearState:
                Mood_Fear.SetValue();
                break;
            case WwiseMoodState.AngerState:
                Mood_Anger.SetValue();
                break;
            default:
            case WwiseMoodState.NeutralState:
                Mood_Neutral.SetValue();
                break;
            case WwiseMoodState.NoneState:
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

    public void SetWwiseCuriositySwitch(WwiseCuriositySwitch switchState)
    {
        if (switchState == currentCuriositySwitch)
        {
            Debug.Log("Curiosity switch is already set to " + switchState);
            return;
        }

        int amount = 0;
        switch (switchState)
        {
            case WwiseCuriositySwitch.Curiosity_0:
                Mood_Curiosity_0.SetValue(gameObject);
                amount = 0;
                break;
            case WwiseCuriositySwitch.Curiosity_1:
                Mood_Curiosity_1.SetValue(gameObject);
                amount = 25;
                break;
            case WwiseCuriositySwitch.Curiosity_2:
                Mood_Curiosity_2.SetValue(gameObject);
                amount = 50;
                break;
            case WwiseCuriositySwitch.Curiosity_3:
                Mood_Curiosity_3.SetValue(gameObject);
                amount = 75;
                break;
        }
        SetWwiseEmotionRTPC(Sinj.Emotions.Curiosity, this.gameObject, amount, ref Anger_Value, ref currentAngerValue);
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

        int amount = 0;
        switch (switchState)
        {
            case WwiseFearSwitch.Fear_0:
                Mood_Fear_0.SetValue(gameObject);
                amount = 0;
                break;
            case WwiseFearSwitch.Fear_1:
                Mood_Fear_1.SetValue(gameObject);
                amount = 25;
                break;
            case WwiseFearSwitch.Fear_2:
                Mood_Fear_2.SetValue(gameObject);
                amount = 50;
                break;
            case WwiseFearSwitch.Fear_3:
                Mood_Fear_3.SetValue(gameObject);
                amount = 75;
                break;
        }
        SetWwiseEmotionRTPC(Sinj.Emotions.Fear, this.gameObject, amount, ref Anger_Value, ref currentAngerValue);
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

        int amount = 0;
        switch (switchState)
        {
            case WwiseAngerSwitch.Anger_0:
                Mood_Anger_0.SetValue(gameObject);
                amount = 0;
                break;
            case WwiseAngerSwitch.Anger_1:
                Mood_Anger_1.SetValue(gameObject);
                amount = 25;
                break;
            case WwiseAngerSwitch.Anger_2:
                Mood_Anger_2.SetValue(gameObject);
                amount = 50;
                break;
            case WwiseAngerSwitch.Anger_3:
                Mood_Anger_3.SetValue(gameObject);
                amount = 75;
                break;
        }
        SetWwiseEmotionRTPC(Sinj.Emotions.Agression, this.gameObject, amount, ref Anger_Value, ref currentAngerValue);

        Debug.Log("Anger switch has been set to " + switchState);
        currentAngerSwitch = switchState;
    }

    public void SetWwiseEmotionRTPC(Sinj.Emotions emotion, GameObject target, float value, ref RTPC emotionRTPC, ref float currentEmotionValue)
    {
        string emotionName = "";

        switch(emotion)
        {
            case Sinj.Emotions.Curiosity:
                emotionName = "Curiosity";
                break;
            case Sinj.Emotions.Fear:
                emotionName = "Fear";
                break;
            case Sinj.Emotions.Agression:
                emotionName = "Anger";
                break;
            case Sinj.Emotions.Tension:
                emotionName = "Tension";
                break;
        }

        if (value == currentEmotionValue)
        {
            Debug.Log($"{emotionName} value is already set to {value}");
            return;
        }
        // Ensure that the target has an AkGameObject component.
        if (target.GetComponent<AkGameObj>() == null)
        {
            target.AddComponent<AkGameObj>();
        }

        // Use 'target' instead of 'gameObject'.
        emotionRTPC.SetValue(target, value);

        Debug.Log("Anger value has been set to " + value);
        currentEmotionValue = value;
    }

}