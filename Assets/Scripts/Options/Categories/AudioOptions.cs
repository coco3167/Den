
    using Audio;
    using TMPro;
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using AK.Wwise;

    namespace Options.Categories
    {
        [Serializable]
    public class WwiseBus
    {
        public Slider _slider;
        public RTPC rtpc;

        public void AddListener()
        {
            _slider.onValueChanged.AddListener(delegate
            {
                AudioManager.SetGlobalRTPCValue(rtpc, _slider.value);
                SaveVolume(_slider.value);
            });
        }

        public void LoadVolume()
        {
            float stored = PlayerPrefs.GetFloat(rtpc.Name, 0.8f);
            _slider.value = stored;
        }
        public void ApplyVolumeToWwise()
        {
            AudioManager.SetGlobalRTPCValue(rtpc, _slider.value);
        }
        public void RemoveListener()
        {
            _slider.onValueChanged.RemoveAllListeners();
        }

        private void SaveVolume(float value)
        {
            PlayerPrefs.SetFloat(rtpc.Name, value);
        }

        public void DeleteData()
        {
            PlayerPrefs.DeleteKey(rtpc.Name);
        }
    }

        public class AudioOptions : MonoBehaviour
        {
            [SerializeField] private List<WwiseBus> buses;
            [SerializeField] private TMP_Dropdown mixDropdown;

            private void Awake()
        {

            // 1) Hook up the dropdown listener *first*
            mixDropdown.onValueChanged.AddListener(OnAudioStateSelected);

            // 2) Make sure the dropdown has one option per enum value
            mixDropdown.ClearOptions();
            var options = new List<string>();
            foreach (WwiseAudioState state in System.Enum.GetValues(typeof(WwiseAudioState)))
                options.Add(state.ToString());
            mixDropdown.AddOptions(options);

            // 3) Expose the currentAudioState publicly in AudioManager
            //    (see note below) and then read it back immediately:
            mixDropdown.value = AudioManager.Instance.CurrentAudioStateIndex;
            mixDropdown.RefreshShownValue();

            // 4) Finally, actually apply it in Wwise
            //    (because TMP_Dropdown.value assignment doesn’t invoke the callback)
            OnAudioStateSelected(mixDropdown.value);
        }
        private void Start()
        {
            foreach (var bus in buses)
            {
                bus.LoadVolume();
                // Apply to Wwise globally right away:
                AudioManager.SetGlobalRTPCValue(bus.rtpc, bus._slider.value);
                bus.AddListener();
            }
        }
        private void OnAudioStateSelected(int value)
        {
            WwiseStateManager.SetWwiseAudioState((WwiseAudioState)value);
        }

        private void OnDestroy()
            {
                foreach (var bus in buses)
                {
                    bus.RemoveListener();
                }
            }

            public void DeleteUserData()
            {
                foreach (var bus in buses)
                {
                    bus.DeleteData();
                    bus.LoadVolume();
                }
            }
        }
    }
