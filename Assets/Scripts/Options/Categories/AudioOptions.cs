
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
        public AK.Wwise.Event sliderChangeEvent;

        [Tooltip("Default value for this RTPC/slider")]
        public float defaultValue = 8f;
        [Tooltip("Minimum value for this RTPC/slider")]
        public float minValue = 0f;
        [Tooltip("Maximum value for this RTPC/slider")]
        public float maxValue = 10f;
        [Tooltip("If true, slider uses whole numbers")]
        public bool wholeNumbers = true;

        public void AddListener()
        {
            _slider.onValueChanged.AddListener(delegate
            {
                float floatValue = _slider.value;
                AudioManager.SetGlobalRTPCValue(rtpc, floatValue);
                SaveVolume(floatValue);

                sliderChangeEvent?.Post(_slider.gameObject);
            });
        }

        public void LoadVolume()
        {
            float stored = PlayerPrefs.GetFloat(rtpc.Name, defaultValue);
            _slider.wholeNumbers = wholeNumbers;
            _slider.minValue = minValue;
            _slider.maxValue = maxValue;
            _slider.SetValueWithoutNotify(stored);
        }

        public void ApplyVolumeToWwise()
        {
            float floatValue = _slider.value;
            AudioManager.SetGlobalRTPCValue(rtpc, floatValue);
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

            // Add the component if it doesn't exist
            var openListener = mixDropdown.GetComponent<DropdownOnPointerClick>();
            if (openListener == null)
                openListener = mixDropdown.gameObject.AddComponent<DropdownOnPointerClick>();

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
