using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Options.Categories
{
    public class GraphicOptions : MonoBehaviour
    {
        [SerializeField] private Toggle fullscreenButton;
        [SerializeField] private Toggle outlineToggle;

        private void Awake()
        {
            fullscreenButton.onValueChanged.AddListener(Fullscreen);
            outlineToggle.onValueChanged.AddListener(Outline);
        }

        private void Fullscreen(bool value)
        {
            #if UNITY_EDITOR
                EditorWindow.mouseOverWindow.maximized = value;
            #else
                Screen.fullScreen = value;
            #endif
        }
        
        private void Outline(bool value)
        {
            GameParameters.IsOutline = value;
        }
    }
}
