using System.Collections.Generic;
using UnityEngine;

namespace DebugHUD
{
    public class DisplayHandler : MonoBehaviour
    {
        [SerializeField] private List<GameObject> objectsToHide;
        private bool m_shouldDisplay;
        
        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))    
                m_shouldDisplay = !m_shouldDisplay;

            objectsToHide.ForEach(x => x.SetActive(m_shouldDisplay));
        }
    }
}
