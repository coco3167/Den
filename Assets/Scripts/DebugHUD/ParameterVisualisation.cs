using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DebugHUD
{
    public class ParameterVisualisation : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI textField;
        [SerializeField] private Image image;

        public void Display(DebugParameter parameter)
        {
            textField.text = $"{parameter.Name} : {parameter.Value}";
            image.color = parameter.IsSpecial ? Color.red : new Color(0, 0, 0, .3f);
        }
    }
}
