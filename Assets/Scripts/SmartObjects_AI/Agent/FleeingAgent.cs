using UnityEngine;

namespace SmartObjects_AI.Agent
{
    public class FleeingAgent : MonoBehaviour
    {
        [SerializeReference] private BaseScoreCalcul scoreCalcul;
        [SerializeField] private MouseAgent mouseAgent;
        [SerializeField] private SmartAgent smartAgent;

        private float m_distance;
        private MouseManager m_mouseManager;
        private SmartObject m_smartObject;

        private void Awake()
        {
            m_mouseManager = GameManager.Instance.GetMouseManager();
            m_smartObject = GetComponentInChildren<SmartObject>();
        }

        private void FixedUpdate()
        {
            m_distance = scoreCalcul.CalculateScore(smartAgent, m_smartObject);
            transform.position = mouseAgent.transform.position + m_distance * (mouseAgent.transform.position - m_mouseManager.GetRawWorldMousePosition()).normalized;
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(transform.position, Vector3.one);
        }
    }
}