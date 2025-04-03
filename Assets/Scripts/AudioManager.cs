using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
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

    [SerializeField] private WwiseCuriositySwitch currentCuriositySWitch;
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
    [SerializeField] private AK.Wwise.Event CreatureTest;

    private void Awake()
    {
        Initialize();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetWwiseAudioState(WwiseAudioState.StereoHeadphones);
        SetWwiseMoodState(WwiseMoodState.Neutral);
        SetWwiseAngerRTPC(0);

        MusicTest.Post(gameObject);
        AmbienceTest.Post(gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetWwiseMoodState(WwiseMoodState.Curiosity);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetWwiseMoodState(WwiseMoodState.Fear);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SetWwiseMoodState(WwiseMoodState.Anger);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SetWwiseMoodState(WwiseMoodState.Neutral);
        }
        if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            SetWwiseAngerRTPC(0);
        }
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            SetWwiseAngerRTPC(26);
        }
        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            SetWwiseAngerRTPC(51);
        }
        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            SetWwiseAngerRTPC(76);
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

    public void SetWwiseCuriositySwitch(WwiseCuriositySwitch switchState)
    {
        if (switchState == currentCuriositySWitch)
        {
            Debug.Log("Curiosity switch is already set to " + switchState);
            return;
        }
        switch (switchState)
        {
            case WwiseCuriositySwitch.Curiosity_0:
                Mood_Curiosity_0.SetValue(gameObject);
                SetWwiseCuriosityRTPC(0);
                break;
            case WwiseCuriositySwitch.Curiosity_1:
                Mood_Curiosity_1.SetValue(gameObject);
                SetWwiseCuriosityRTPC(26);
                break;
            case WwiseCuriositySwitch.Curiosity_2:
                Mood_Curiosity_2.SetValue(gameObject);
                SetWwiseCuriosityRTPC(51);
                break;
            case WwiseCuriositySwitch.Curiosity_3:
                Mood_Curiosity_3.SetValue(gameObject);
                SetWwiseCuriosityRTPC(76);
                break;
        }
        Debug.Log("Curiosity switch has been set to " + switchState);
        currentCuriositySWitch = switchState;
    }
    public void SetWwiseFearSwitch(WwiseFearSwitch switchState)
    {
        if (switchState == currentFearSwitch)
        {
            Debug.Log("Fear switch is already set to " + switchState);
            return;
        }
        switch (switchState)
        {
            case WwiseFearSwitch.Fear_0:
                Mood_Fear_0.SetValue(gameObject);
                SetWwiseFearRTPC(0);
                break;
            case WwiseFearSwitch.Fear_1:
                Mood_Fear_1.SetValue(gameObject);
                SetWwiseFearRTPC(26);
                break;
            case WwiseFearSwitch.Fear_2:
                Mood_Fear_2.SetValue(gameObject);
                SetWwiseFearRTPC(51);
                break;
            case WwiseFearSwitch.Fear_3:
                Mood_Fear_3.SetValue(gameObject);
                SetWwiseFearRTPC(76);
                break;
        }
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
        switch (switchState)
        {
            case WwiseAngerSwitch.Anger_0:
                Mood_Anger_0.SetValue(gameObject);
                SetWwiseAngerRTPC(0);
                break;
            case WwiseAngerSwitch.Anger_1:
                Mood_Anger_1.SetValue(gameObject);
                SetWwiseAngerRTPC(26);
                break;
            case WwiseAngerSwitch.Anger_2:
                Mood_Anger_2.SetValue(gameObject);
                SetWwiseAngerRTPC(51);
                break;
            case WwiseAngerSwitch.Anger_3:
                Mood_Anger_3.SetValue(gameObject);
                SetWwiseAngerRTPC(76);
                break;
        }
        Debug.Log("Anger switch has been set to " + switchState);
        currentAngerSwitch = switchState;
    }

    public void SetWwiseCuriosityRTPC(float value)
    {
        if (value == currentCuriosityValue)
        {
            Debug.Log("Curiosity value is already set to " + value);
            return;
        }
        Curiosity_Value.SetValue(gameObject, value);
        Debug.Log("Curiosity value has been set to " + value);
        currentCuriosityValue = value;
    }
    public void SetWwiseFearRTPC(float value)
    {
        if (value == currentFearValue)
        {
            Debug.Log("Fear value is already set to " + value);
            return;
        }
        Fear_Value.SetValue(gameObject, value);
        Debug.Log("Fear value has been set to " + value);
        currentFearValue = value;
    }
    public void SetWwiseAngerRTPC(float value)
    {
        if (value == currentAngerValue)
        {
            Debug.Log("Anger value is already set to " + value);
            return;
        }
        Anger_Value.SetValue(gameObject, value);
        Debug.Log("Anger value has been set to " + value);
        currentAngerValue = value;
    }
    public void SetWwiseIntensityRTPC(float value)
    {
        if (value == currentIntensityValue)
        {
            Debug.Log("Intensity value is already set to " + value);
            return;
        }
        Intensity_Value.SetValue(gameObject, value);
        Debug.Log("Intensity value has been set to " + value);
        currentIntensityValue = value;
    }
    public void SetWwiseTensionRTPC(float value)
    {
        if (value == currentTensionValue)
        {
            Debug.Log("Tension value is already set to " + value);
            return;
        }
        Tension_Value.SetValue(gameObject, value);
        Debug.Log("Tension value has been set to " + value);
        currentTensionValue = value;
    }
}
