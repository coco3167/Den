
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

        // Reference to the Wwise event for slider change
        public AK.Wwise.Event sliderChangeEvent;

        public void AddListener()
        {
            _slider.onValueChanged.AddListener(delegate
            {
                int intValue = Mathf.RoundToInt(_slider.value);
                AudioManager.SetGlobalRTPCValue(rtpc, intValue);
                SaveVolume(intValue);

                // Play Wwise event for slider change
                sliderChangeEvent?.Post(_slider.gameObject);
            });
        }

        public void LoadVolume()
        {
            int stored = PlayerPrefs.GetInt(rtpc.Name, 8);
            _slider.wholeNumbers = true;
            _slider.minValue = 0;
            _slider.maxValue = 10;
            _slider.SetValueWithoutNotify(stored);
        }
        public void ApplyVolumeToWwise()
        {
            int intValue = Mathf.RoundToInt(_slider.value);
            AudioManager.SetGlobalRTPCValue(rtpc, intValue);
        }
        public void RemoveListener()
        {
            _slider.onValueChanged.RemoveAllListeners();
        }

        private void SaveVolume(int value)
        {
            PlayerPrefs.SetInt(rtpc.Name, value);
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

        // Wwise events for dropdown actions
        [SerializeField] private AK.Wwise.Event dropdownOpenEvent;
        [SerializeField] private AK.Wwise.Event dropdownSelectEvent;

        private void Awake()
        {
            mixDropdown.ClearOptions();
            var options = new List<string>();
            foreach (WwiseAudioState state in System.Enum.GetValues(typeof(WwiseAudioState)))
                options.Add(state.ToString());
            mixDropdown.AddOptions(options);

            // Set value before adding the listener
            mixDropdown.SetValueWithoutNotify(AudioManager.Instance.CurrentAudioStateIndex);
            mixDropdown.RefreshShownValue();

            // Add value changed listener for option selection
            mixDropdown.onValueChanged.AddListener(OnAudioStateSelected);

            // Add listener for dropdown open (pointer click)
            mixDropdown.DropdownOnPointerClick += OnDropdownOpened;

            var openListener = mixDropdown.gameObject.AddComponent<DropdownOnPointerClick>();
            openListener.onDropdownOpened += OnDropdownOpened;
        }

        private void Start()
        {
            foreach (var bus in buses)
            {
                bus.LoadVolume();
                bus.ApplyVolumeToWwise();
                bus.AddListener();
            }
        }

        private void OnAudioStateSelected(int value)
        {
            WwiseStateManager.SetWwiseAudioState((WwiseAudioState)value);

            // Play Wwise event for dropdown option selection
            dropdownSelectEvent?.Post(mixDropdown.gameObject);
        }

        // Called when the dropdown is opened
        private void OnDropdownOpened(UnityEngine.EventSystems.PointerEventData eventData)
        {
            dropdownOpenEvent?.Post(mixDropdown.gameObject);
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
