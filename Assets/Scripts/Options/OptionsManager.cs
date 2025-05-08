using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

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

        
        private void Awake()
        {
            generalButton.AddButtonListener(generalCategory.Show);
            controlesButton.AddButtonListener(controlesCategory.Show);
            graphicsButton.AddButtonListener(graphicsCategory.Show);
            audioButton.AddButtonListener(audioCategory.Show);

            if (GameManager.Instance == null)
                return;
            
            GameManager.Instance.GamePaused += OnPaused;
            ShowOptions(false);
        }

        private void OnPaused(object obj, GameManager.GamePausedEventArgs e)
        {
            ShowOptions(e.IsPaused);
        }

        private void ShowOptions(bool isPaused)
        {
            gameObjectsToHide.ForEach(x => x.SetActive(isPaused));

            if (isPaused)
            {
                generalButton.ClickOnButton();
                EventSystem.current.SetSelectedGameObject(generalButton.gameObject);
            }
        }
    }
}
