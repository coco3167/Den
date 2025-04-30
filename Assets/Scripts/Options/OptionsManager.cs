using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Options
{
    public class OptionsManager : MonoBehaviour
    {
        [Title("General")]
        [SerializeField] private CategoryButton generalButton;
        [SerializeField] private Category generalCategory;
        
        [Title("Controles")]
        [SerializeField] private CategoryButton controlesButton;
        [SerializeField] private Category controlesCategory;
        
        [Title("Graphics")]
        [SerializeField] private CategoryButton graphicsButton;
        [SerializeField] private Category graphicsCategory;
        
        [Title("Audio")]
        [SerializeField] private CategoryButton audioButton;
        [SerializeField] private Category audioCategory;
        
        private void Awake()
        {
            generalButton.AddButtonListener(generalCategory.Show);
            controlesButton.AddButtonListener(controlesCategory.Show);
            graphicsButton.AddButtonListener(graphicsCategory.Show);
            audioButton.AddButtonListener(audioCategory.Show);
            
            EventSystem.current.SetSelectedGameObject(generalButton.gameObject);
        }
    }
}
