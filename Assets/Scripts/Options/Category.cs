using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Options
{
    public class Category : MonoBehaviour
    {
        private static List<Category> _categories = new List<Category>();

        [SerializeField, ChildGameObjectsOnly] private GameObject mainChild;

        private void Awake()
        {
            _categories.Add(this);
        }

        public void Show()
        {
            mainChild.SetActive(true);
            HideOtherCategories();
        }

        public void Hide()
        {
            mainChild.SetActive(false);
        }
        
        private void HideOtherCategories()
        {
            foreach (Category category in _categories)
            {
                if (category != this)
                {
                    category.Hide();
                }
            }
        }

        private void OnDestroy()
        {
            _categories.Remove(this);
        }
    }
}
