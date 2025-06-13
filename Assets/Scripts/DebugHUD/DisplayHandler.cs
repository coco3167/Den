using System.Collections.Generic;
using UnityEngine;

namespace DebugHUD
{
    public class DisplayHandler : MonoBehaviour
    {
        [SerializeField] private List<GameObject> objectsToHide;
        [SerializeField] private List<DebugDisplay> debugDisplays;
        private bool m_shouldDisplay;
        
        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
                ShouldDisplay();
        }

        private void ShouldDisplay()
        {
            m_shouldDisplay = !m_shouldDisplay;
            objectsToHide.ForEach(x => x.SetActive(m_shouldDisplay));
            if (m_shouldDisplay)
            {
                foreach (DebugDisplay debugDisplay in debugDisplays)
                {
                    debugDisplay.Init();
                }
            }
        }
    }
}
