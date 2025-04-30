using System.Collections.Generic;
using AK.Wwise;
using AYellowpaper.SerializedCollections;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Audio
{
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
    public enum WwiseReactionMoodSwitch
    {
        CuriositySwitch,
        FearSwitch,
        AngerSwitch,
        None,
    }

    public enum VolumeType
    {
        Music,
        SFX,
        Ambiance,
    }

    [RequireComponent(typeof(AkGameObj))]
    public class AudioManager : MonoBehaviour
    {

        public static AudioManager Instance;

        private bool bIsInitialized = false;

        [Title("Startup Soundbanks")]
        [SerializeField] private List<Bank> Soundbanks;

        [Title("Game State Mood Variables")] [ReadOnly, SerializeField, HideLabel] private bool stateMoodSeparator;
        [SerializeField] public SerializedDictionary<WwiseMoodState, State> gameStateMoodVisualization;
        [SerializeField, ReadOnly] private WwiseMoodState currentMoodState;

        [Title("Audio State Variables")] [ReadOnly, SerializeField, HideLabel] private bool audioStateSeparator;
        [SerializeField] public SerializedDictionary<WwiseAudioState, State> audioState;
        [SerializeField, ReadOnly] private WwiseAudioState currentAudioState;
        [Title("Mono State Toggle")]
        [SerializeField, ToggleLeft] private bool enableMonoState = false;

        [Title("Wwise Mood Switches")] [ReadOnly, SerializeField, HideLabel] private bool moodSwitchSeparator;
        [SerializeField] private SerializedDictionary<WwiseCuriositySwitch, Switch> moodCuriosity;
        [SerializeField] private SerializedDictionary<WwiseFearSwitch, Switch> moodFear;
        [SerializeField] private SerializedDictionary<WwiseAngerSwitch, Switch> moodAnger;

        [SerializeField, ReadOnly] private WwiseCuriositySwitch currentCuriositySwitch;
        [SerializeField, ReadOnly] private WwiseFearSwitch currentFearSwitch;
        [SerializeField, ReadOnly] private WwiseAngerSwitch currentAngerSwitch;

        [Title("Game State Mood Variables")][ReadOnly, SerializeField, HideLabel] private bool switchReactionMoodSeparator;
        [SerializeField] public SerializedDictionary<WwiseReactionMoodSwitch, Switch> switchReactionMood;
        [SerializeField, ReadOnly] private WwiseReactionMoodSwitch currentSwitchReactionMood;

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
        [SerializeField] private AK.Wwise.Event PlayAmbience;


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
            if (enableMonoState)
            {
                WwiseStateManager.SetWwiseAudioState(WwiseAudioState.Mono);
            }
            else
                WwiseStateManager.SetWwiseAudioState(WwiseAudioState.StereoHeadphones);
            WwiseStateManager.SetWwiseMoodState(WwiseMoodState.NeutralState);

            PlayAmbience.Post(this.gameObject);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Keypad1))
            {
                WwiseStateManager.SetWwiseMoodState(WwiseMoodState.NeutralState);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad2))
            {
                WwiseStateManager.SetWwiseMoodState(WwiseMoodState.CuriosityState);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad3))
            {
                WwiseStateManager.SetWwiseMoodState(WwiseMoodState.FearState);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad4))
            {
                WwiseStateManager.SetWwiseMoodState(WwiseMoodState.AngerState);
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

            WwiseStateManager.SetWwiseMoodState(WwiseMoodState.NoneState);
            WwiseStateManager.SetWwiseAudioState(WwiseAudioState.None);

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
        public void UnloadSoundbanks(List<string> bankNames)
        {
            if (bankNames == null || bankNames.Count == 0)
            {
                Debug.LogError("Bank names list is empty or null! Please provide valid bank names to unload.");
                return;
            }

            foreach (string bankName in bankNames)
            {
                Bank bankToUnload = Soundbanks.Find(bank => bank.Name == bankName);
                if (bankToUnload != null)
                {
                    bankToUnload.Unload();
                    Debug.Log($"Soundbank '{bankName}' has been unloaded.");
                }
                else
                {
                    Debug.LogWarning($"Soundbank '{bankName}' not found in the assigned Soundbanks list.");
                }
            }
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
                case Sinj.Emotions.Intensity:
                    return WwiseEmotionStateRTPC.Intensity;
                default:
                    return WwiseEmotionStateRTPC.Tension;
            }
        }

        public void ChangeAudioVolume(VolumeType volumeType, float value)
        {
            //TODO Jules implement here volume change depending on volumeType (use switch)
        }
    }
}