using TMPro;
using UnityEngine;

namespace DebugHUD
{
    public class ParameterVisualisation : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI textField;

        public void Display(DebugParameter parameter)
        {
            textField.text = $"{parameter.Name} : {parameter.Value}";
        }
    }
}
