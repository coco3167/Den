using UnityEngine;
using UnityEngine.UI;

namespace Options.Categories
{
    public class Controles : MonoBehaviour
    {
        [SerializeField] private Slider mouseSensitivity;
        [SerializeField] private Slider joystickSensitivity;

        private void Awake()
        {
            mouseSensitivity.onValueChanged.AddListener(OnMouseSensitivityChanged);
            joystickSensitivity.onValueChanged.AddListener(OnJoystickSensitivityChanged);
        }

        private void OnMouseSensitivityChanged(float value)
        {
            GameParameters.MouseSensitivity = value;
        }
        private void OnJoystickSensitivityChanged(float value)
        {
            GameParameters.JoystickSensitivity = value;
        }
    }
}
