using Sirenix.OdinInspector;
using UnityEngine;

namespace Options
{
    public class SelectedFlowers : MonoBehaviour
    {
        [SerializeField, ChildGameObjectsOnly] private GameObject[] flowers;

        private void Awake()
        {
            OnSelect(false);
        }

        public void OnSelect(bool selected)
        {
            foreach (GameObject flower in flowers)
            {
                flower.SetActive(selected);
            }
        }
    }
}
