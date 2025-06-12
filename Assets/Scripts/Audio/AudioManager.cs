using System;
using System.Collections.Generic;
using System.Linq;
using AK.Wwise;
using AYellowpaper.SerializedCollections;
using Sirenix.OdinInspector;
using SmartObjects_AI.Agent;
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
    public enum WwiseMixPreset
    {
        Regular,
        Gameplay,
        Contemplative,
        None
    }
    public enum WwiseEmotionStateRTPC
    {
        Curiosity,
        Fear,
        Anger,
        Intensity,
        Tension,
        Neutral,
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
    public class AudioManager : MonoBehaviour, IGameStateListener, IReloadable
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

        [Title("Mix Preset Variables")][ReadOnly, SerializeField, HideLabel] private bool mixPresetSeparator;
        [SerializeField] public SerializedDictionary<WwiseMixPreset, State> mixPresets;
        [SerializeField, ReadOnly] public WwiseMixPreset currentMixPreset;
        public WwiseMixPreset CurrentMixPreset => currentMixPreset;
        public int CurrentMixPresetIndex => (int)currentMixPreset;

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
        [SerializeField] private AK.Wwise.RTPC CursorSpeed;
        private Dictionary<AgentDynamicParameter, int> emotionPalliers = new()
        {
            { AgentDynamicParameter.Curiosity, 0 },
            { AgentDynamicParameter.Aggression, 0 },
            { AgentDynamicParameter.Fear, 0 },
        };
        private uint cursorMovePlayingId = 0;
        [SerializeField] private GameObject mouseManifestation;
        public GameObject MouseManifestation => mouseManifestation;

        [Title("Wwise Events")]
        [FoldoutGroup("Wwise UI Events")]
        [SerializeField] public AK.Wwise.Event FaderTick;
        [FoldoutGroup("Wwise UI Events")]
        [SerializeField] public AK.Wwise.Event Box;
        [FoldoutGroup("Wwise UI Events")]
        [SerializeField] public AK.Wwise.Event UIMove;
        [FoldoutGroup("Wwise UI Events")]
        [SerializeField] public AK.Wwise.Event UIAccept;
        [FoldoutGroup("Wwise UI Events")]
        [SerializeField] public AK.Wwise.Event UIBack;
        [FoldoutGroup("Wwise UI Events")]
        [SerializeField] public AK.Wwise.Event UIMenu;
        [FoldoutGroup("Wwise UI Events")]
        [SerializeField] public AK.Wwise.Event MasterTest;
        [FoldoutGroup("Wwise UI Events")]
        [SerializeField] public AK.Wwise.Event MusicTest;
        [FoldoutGroup("Wwise UI Events")]
        [SerializeField] public AK.Wwise.Event AmbienceTest;
        [FoldoutGroup("Wwise UI Events")]
        [SerializeField] public AK.Wwise.Event SFXTest;
        [FoldoutGroup("Wwise UI Events")]
        [SerializeField] public AK.Wwise.Event Menu;
        [FoldoutGroup("Wwise UI Events")]
        [SerializeField] public AK.Wwise.Event Back;
        [FoldoutGroup("Wwise UI Events")]
        [SerializeField] public AK.Wwise.Event Accept;
        [FoldoutGroup("Wwise UI Events")]
        [SerializeField] public AK.Wwise.Event Move;
        [FoldoutGroup("Wwise Move Events")]
        [SerializeField] public AK.Wwise.Event FootstepWalk;
        [FoldoutGroup("Wwise Move Events")]
        [SerializeField] public AK.Wwise.Event FootstepRun;
        [FoldoutGroup("Wwise Move Events")]
        [SerializeField] public AK.Wwise.Event Eat;
        [FoldoutGroup("Wwise Move Events")]
        [SerializeField] public AK.Wwise.Event Scratch;
        [FoldoutGroup("Wwise Move Events")]
        [SerializeField] public AK.Wwise.Event Sit;
        [FoldoutGroup("Wwise Move Events")]
        [SerializeField] public AK.Wwise.Event Stand;
        [FoldoutGroup("Wwise Move Events")]
        [SerializeField] public AK.Wwise.Event Tube;




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
                {WwiseEmotionStateRTPC.Neutral, 0f},
            };

            ResetAmbience.Post(this.gameObject);
        }
        void Start()
        {
            WwiseStateManager.SetWwiseMoodState(WwiseMoodState.NeutralState);
            WwiseStateManager.SetWwiseAudioState(WwiseAudioState.StereoHeadphones);

            StartAmbience();
        }
        private void FixedUpdate()
        {
            // Call cursor sound
            float speed = GameManager.Instance.GetMouseManager().MouseVelocity();
            AudioManager.Instance.UpdateCursorSpeed(speed, mouseManifestation);
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
        public void RestartAmbience()
        {
            ResetAmbience.Post(this.gameObject);
        }
        public void PauseAmbience()
        {
            PlayAmbience?.ExecuteAction(this.gameObject, AkActionOnEventType.AkActionOnEventType_Pause, 450, AkCurveInterpolation.AkCurveInterpolation_Linear);
        }
        public void ResumeAmbience()
        {
            PlayAmbience?.ExecuteAction(this.gameObject, AkActionOnEventType.AkActionOnEventType_Resume, 250, AkCurveInterpolation.AkCurveInterpolation_Linear);
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
            // Clamp value to Wwise RTPC range
            float clampedValue = value;

            WwiseEmotionStateRTPC emotionStateRtpc = TranslateSinjAgentEmotionToAudioManagerEmotion(parameter);
            RTPC emotionRTPC = gameParameters[emotionStateRtpc];

            if (Mathf.Approximately(clampedValue, currentGameParametersValues[emotionStateRtpc]))
            {
                Debug.Log($"{emotionStateRtpc.ToString()} value is already set to {clampedValue}");
                return;
            }
            if (!target.GetComponent<AkGameObj>())
            {
                target.AddComponent<AkGameObj>();
            }

            emotionRTPC.SetValue(target, clampedValue);

            Debug.Log($"{emotionStateRtpc} value has been set to {clampedValue}");
            currentGameParametersValues[emotionStateRtpc] = clampedValue;
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
                case AgentDynamicParameter.Neutral:
                    return WwiseEmotionStateRTPC.Neutral;
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
            Debug.Log($"[AudioManager] Dominant mood: {dominantMood}, palliers: Curiosity={emotionPalliers[AgentDynamicParameter.Curiosity]}, Anger={emotionPalliers[AgentDynamicParameter.Aggression]}, Fear={emotionPalliers[AgentDynamicParameter.Fear]}");

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
                // pas de *25 ici aussi
                SetWwiseEmotionRTPC(kv.Key, gameObject, kv.Value);
            }

            // Set Neutral RTPC to the highest pallier reached (times interval)
            int maxPallier = emotionPalliers.Values.Max();
            float neutralValue = maxPallier; // pas *25 ici
            SetWwiseEmotionRTPC(AgentDynamicParameter.Neutral, gameObject, neutralValue);
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
            CursorSpeed.SetValue(target, clampedSpeed);
        }

        #endregion Cursor

        #region Cycles
        public void OnGameReady(object sender, EventArgs eventArgs)
        {
            StartCursorMoveSound(mouseManifestation);
        }

        public void OnGameEnded(object sender, EventArgs eventArgs)
        {
            StopCursorMoveSound(mouseManifestation);
        }

        public void Reload()
        {
            // Reset all emotion palliers to 0
            foreach (var key in emotionPalliers.Keys.ToList())
            {
                emotionPalliers[key] = 0;
            }

            // Update RTPCs and mood state to reflect the reset
            UpdateMoodAndRTPCs();

            // Reset mood state and ambience as before
            WwiseStateManager.SetWwiseMoodState(WwiseMoodState.NeutralState);
            RestartAmbience();
            GameManager.Instance.Reload();
        }
        #endregion Cycles
    }
}