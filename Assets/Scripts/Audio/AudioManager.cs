using System.Collections.Generic;
using System.Linq;
using AK.Wwise;
using AYellowpaper.SerializedCollections;
using Sinj;
using Sirenix.OdinInspector;
using SmartObjects_AI.Agent;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

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
    public enum WwiseTODState
    {
        Morning,
        Day,
        Evening,
        Night
    }
    [RequireComponent(typeof(AkGameObj))]
    public class AudioManager : MonoBehaviour
    {

        public static AudioManager Instance;

        private bool bIsInitialized = false;

        [Title("Startup Soundbanks")]
        [SerializeField] private List<Bank> Soundbanks;

        [Title("Game State Mood Variables")][ReadOnly, SerializeField, HideLabel] private bool stateMoodSeparator;
        [SerializeField] public SerializedDictionary<WwiseMoodState, State> gameStateMoodVisualization;
        [SerializeField, ReadOnly] private WwiseMoodState currentMoodState;

        [Title("Audio State Variables")][ReadOnly, SerializeField, HideLabel] private bool audioStateSeparator;
        [SerializeField] public SerializedDictionary<WwiseAudioState, State> audioState;
        [SerializeField, ReadOnly] public WwiseAudioState currentAudioState;
        public WwiseAudioState CurrentAudioState => currentAudioState;
        public int CurrentAudioStateIndex => (int)currentAudioState;

        [Title("Wwise Mood Switches")][ReadOnly, SerializeField, HideLabel] private bool moodSwitchSeparator;
        [SerializeField] private SerializedDictionary<WwiseCuriositySwitch, Switch> moodCuriosity;
        [SerializeField] private SerializedDictionary<WwiseFearSwitch, Switch> moodFear;
        [SerializeField] private SerializedDictionary<WwiseAngerSwitch, Switch> moodAnger;

        [SerializeField, ReadOnly] private WwiseCuriositySwitch currentCuriositySwitch;
        [SerializeField, ReadOnly] private WwiseFearSwitch currentFearSwitch;
        [SerializeField, ReadOnly] private WwiseAngerSwitch currentAngerSwitch;

        [Title("Game State Mood Variables")][ReadOnly, SerializeField, HideLabel] private bool switchReactionMoodSeparator;
        [SerializeField] public SerializedDictionary<WwiseReactionMoodSwitch, Switch> switchReactionMood;
        [SerializeField, ReadOnly] private WwiseReactionMoodSwitch currentSwitchReactionMood;

        [Title("Wwise Game Parameters")][ReadOnly, SerializeField, HideLabel] private bool parametersSeparators;
        [SerializeField] private SerializedDictionary<WwiseEmotionStateRTPC, RTPC> gameParameters;
        [SerializeField, ReadOnly] private SerializedDictionary<WwiseEmotionStateRTPC, float> currentGameParametersValues;

        [Title("Wwise Volume Paramters")]
        [SerializeField] public AK.Wwise.RTPC MasterVolume;
        [SerializeField] public AK.Wwise.RTPC MusicVolume;
        [SerializeField] public AK.Wwise.RTPC SFXVolume;
        [SerializeField] public AK.Wwise.RTPC AmbienceVolume;

        [Title("Wwise Events")]
        [SerializeField] private AK.Wwise.Event PlayIntroMusic;
        [SerializeField] private AK.Wwise.Event PlayAmbience;
        [SerializeField] private AK.Wwise.Event ResetAmbience;

        [Title("Wwise Stinger Events")]
        [SerializeField] private AK.Wwise.Event CuriosityStinger;
        [SerializeField] private AK.Wwise.Event AngerStinger;
        [SerializeField] private AK.Wwise.Event FearStinger;

        [Title("Wwise Time of Day States")]
        [SerializeField]
        public SerializedDictionary<WwiseTODState, State> timeOfDayStates;

        [Title("Wwise Cursor Movement")]
        [SerializeField] private AK.Wwise.Event CursorMoveEvent;
        [SerializeField] private AK.Wwise.RTPC DEN_GP_CursorSpeed;

        private Dictionary<AgentDynamicParameter, int> emotionPalliers = new()
        {
            { AgentDynamicParameter.Curiosity, 0 },
            { AgentDynamicParameter.Aggression, 0 },
            { AgentDynamicParameter.Fear, 0 },
        };
        private uint cursorMovePlayingId = 0;

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

            ResetAmbience.Post(this.gameObject);
        }
        void Start()
        {
            WwiseStateManager.SetWwiseMoodState(WwiseMoodState.NeutralState);
            WwiseStateManager.SetWwiseAudioState(WwiseAudioState.StereoHeadphones);

            StartAmbience();
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

        public void PlayEmotionSteps(AgentDynamicParameter parameter, int pallierReached)
        {
            emotionPalliers[parameter] = pallierReached;
            UpdateMoodAndRTPCs();

            // Trigger the stinger event for the emotion
            switch (parameter)
            {
                case AgentDynamicParameter.Curiosity:
                    CuriosityStinger?.Post(this.gameObject);
                    break;
                case AgentDynamicParameter.Aggression:
                    AngerStinger?.Post(this.gameObject);
                    break;
                case AgentDynamicParameter.Fear:
                    FearStinger?.Post(this.gameObject);
                    break;
            }
        }
        private void StartAmbience()
        {
            PlayAmbience.Post(this.gameObject);
        }

        #region Tools
        private void Initialize()
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
        private void LoadSoundbanks()
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
        #endregion Tools

        #region RTPC
        public void SetWwiseEmotionRTPC(AgentDynamicParameter parameter, GameObject target, float value)
        {
            WwiseEmotionStateRTPC emotionStateRtpc = TranslateSinjAgentEmotionToAudioManagerEmotion(parameter);
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
        private WwiseEmotionStateRTPC TranslateSinjAgentEmotionToAudioManagerEmotion(AgentDynamicParameter parameter)
        {
            switch (parameter)
            {
                case AgentDynamicParameter.Curiosity:
                    return WwiseEmotionStateRTPC.Curiosity;
                case AgentDynamicParameter.Fear:
                    return WwiseEmotionStateRTPC.Fear;
                case AgentDynamicParameter.Aggression:
                    return WwiseEmotionStateRTPC.Anger;
                case AgentDynamicParameter.Tension:
                    return WwiseEmotionStateRTPC.Tension;
                default:
                    return WwiseEmotionStateRTPC.Tension;
            }
        }
        public static void SetGlobalRTPCValue(RTPC rtpc, float value)
        {
            rtpc.SetGlobalValue(rtpc.Name, value);
        }

        #endregion RTPC

        #region Mood 
        private AgentDynamicParameter GetDominantMood()
        {
            // Returns the emotion with the highest pallier
            return emotionPalliers.OrderByDescending(kv => kv.Value).First().Key;
        }
        public void UpdateMoodAndRTPCs()
        {
            AgentDynamicParameter dominantMood = GetDominantMood();

            // Set the mood state
            WwiseMoodState moodState = dominantMood switch
            {
                AgentDynamicParameter.Curiosity => WwiseMoodState.CuriosityState,
                AgentDynamicParameter.Fear => WwiseMoodState.FearState,
                AgentDynamicParameter.Aggression => WwiseMoodState.AngerState,
                _ => WwiseMoodState.NeutralState
            };
            WwiseStateManager.SetWwiseMoodState(moodState);

            // Set RTPCs for each emotion
            foreach (var kv in emotionPalliers)
            {
                float value = kv.Value * GameManager.IntervalPallier; // Each pallier is 25
                SetWwiseEmotionRTPC(kv.Key, gameObject, value);
            }
        }
        #endregion Mood

        #region TOD
        public void SetWwiseTODState(WwiseTODState state)
        {
            if (timeOfDayStates.TryGetValue(state, out var wwiseState))
            {
                wwiseState.SetValue();
            }
            else
            {
                Debug.LogWarning($"No Wwise state mapped for {state}");
            }
        }
        public static WwiseTODState ToWwiseTODState(GameLoopManager.GameLoopState state)
        {
            return state switch
            {
                GameLoopManager.GameLoopState.Morning => WwiseTODState.Morning,
                GameLoopManager.GameLoopState.Day => WwiseTODState.Day,
                GameLoopManager.GameLoopState.Evening => WwiseTODState.Evening,
                GameLoopManager.GameLoopState.Night => WwiseTODState.Night,
                _ => WwiseTODState.Morning
            };
        }

        #endregion TOD

        #region Cursor
        public void StartCursorMoveSound(GameObject target)
        {
            if (cursorMovePlayingId == 0)
            {
                if (!target.GetComponent<AkGameObj>())
                    target.AddComponent<AkGameObj>();
                cursorMovePlayingId = CursorMoveEvent.Post(target);
                Debug.Log($"CursorMoveEvent posted, playingId: {cursorMovePlayingId}");
            }
        }
        public void StopCursorMoveSound(GameObject target)
        {
            if (cursorMovePlayingId != 0)
            {
                CursorMoveEvent.Stop(target);
                cursorMovePlayingId = 0;
            }
        }
        public void UpdateCursorSpeed(float speed, GameObject target)
        {
            float clampedSpeed = Mathf.Clamp(speed, 0f, 100f);
            if (!target.GetComponent<AkGameObj>())
                target.AddComponent<AkGameObj>();
            DEN_GP_CursorSpeed.SetValue(target, clampedSpeed);
            Debug.Log($"Cursor speed updated to {clampedSpeed}");
        }
        #endregion Cursor
    }
}