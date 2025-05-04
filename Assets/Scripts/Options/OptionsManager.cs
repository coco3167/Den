using System;
using System.Collections.Generic;
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

        [Title("GameObject to Hide")] 
        [SerializeField, ChildGameObjectsOnly] private List<GameObject> gameObjectsToHide;

        private bool m_isShowed = true;
        
        private void Awake()
        {
            generalButton.AddButtonListener(generalCategory.Show);
            controlesButton.AddButtonListener(controlesCategory.Show);
            graphicsButton.AddButtonListener(graphicsCategory.Show);
            audioButton.AddButtonListener(audioCategory.Show);
            
            ShowOptions();
        }

        private void Update()
        {
            // TODO refactor cette dégeulasserie
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                ShowOptions();
            }
        }

        public void ShowOptions()
        {
            m_isShowed = !m_isShowed;
            
            gameObjectsToHide.ForEach(x => x.SetActive(m_isShowed));
            
            if(m_isShowed)
                EventSystem.current.SetSelectedGameObject(generalButton.gameObject);
        }
    }
}
