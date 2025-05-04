using Audio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Options.Categories
{
    public class AudioOptions : MonoBehaviour
    {
        [SerializeField] private Slider musicSlider, sfxSlider, ambianceSlider;
        [SerializeField] private TMP_Dropdown monoDropdown;

        private void Awake()
        {
            // Link ui to wwise parameters
            musicSlider.onValueChanged.AddListener(delegate(float value)
            {
                AudioManager.Instance.ChangeAudioVolume(VolumeType.Music, value);
            });
            sfxSlider.onValueChanged.AddListener(delegate(float value)
            {
                AudioManager.Instance.ChangeAudioVolume(VolumeType.SFX, value);
            });
            ambianceSlider.onValueChanged.AddListener(delegate(float value)
            {
                AudioManager.Instance.ChangeAudioVolume(VolumeType.Ambiance, value);
            });

            monoDropdown.onValueChanged.AddListener(delegate(int value)
            {
                WwiseStateManager.SetWwiseAudioState((WwiseAudioState)value);
            });
        }
    }
}