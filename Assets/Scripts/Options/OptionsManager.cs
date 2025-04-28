using UnityEngine;

namespace Options
{
    public class OptionsManager : MonoBehaviour
    {
        [SerializeField] private CategoryButton controlesButton, graphicsButton, audioButton;
        [SerializeField] private Category controlesCategory, graphicsCategory, audioCategory;

        private void Awake()
        {
            controlesButton.AddButtonListener(controlesCategory.Show);
            graphicsButton.AddButtonListener(graphicsCategory.Show);
            audioButton.AddButtonListener(audioCategory.Show);
        }
    }
}
