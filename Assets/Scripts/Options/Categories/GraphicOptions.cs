using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Options.Categories
{
    public class GraphicOptions : MonoBehaviour
    {
        [SerializeField] private Toggle fullscreenButton;

        private void Awake()
        {
            fullscreenButton.onValueChanged.AddListener(Fullscreen);
        }

        private void Fullscreen(bool value)
        {
            #if UNITY_EDITOR
                EditorWindow.mouseOverWindow.maximized = value;
            #else
                Screen.fullScreen = value;
            #endif
        }
    }
}
