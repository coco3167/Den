using UnityEngine;

namespace SmartObjects_AI
{
    public class FleePointPositionning : MonoBehaviour
    {
        [SerializeField] private float distance;
        
        private Transform m_agent;
        private MouseManager m_mouseManager;
        
        private void Awake()
        {
            m_agent = transform.parent;
            m_mouseManager = GameManager.Instance.GetMouseManager();
        }

        private void FixedUpdate()
        {
            Vector3 mousePos = m_mouseManager.GetRawWorldMousePosition();
            mousePos.y = m_agent.position.y;
            Vector3 fleeVector = (m_agent.position - mousePos).normalized;
            if (fleeVector == Vector3.zero)
                fleeVector = new Vector3(Random.Range(0, 1), 0, Random.Range(0, 1)).normalized;
            transform.position = distance * fleeVector + mousePos;
        }
    }
}
